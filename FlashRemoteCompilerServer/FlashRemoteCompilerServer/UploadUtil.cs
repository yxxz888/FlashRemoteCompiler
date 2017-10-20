using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace FlashRemoteCompilerServer
{
    class UploadUtil
    {
        public delegate void UploadFinishedCallback(Boolean isSuc,String message);

        private UploadFinishedCallback onUploadFinished;

        private FlaItem[] uploadList;

        public void uploadFile(String[] fileList,UploadFinishedCallback callback)
        {
            this.onUploadFinished += callback;

            uploadList = getFlaItemList(fileList);

            if (compressSwc())
            {
                Tuple<Boolean, String> result = copyClientFile();
                onUploadFinished(result.Item1,result.Item2);
            }
            else
                onUploadFinished(false, "压缩swc错误！");
        }

        private Boolean compressSwc()
        {
            Process proc = null;
            try
            {
                proc = new Process();
                proc.StartInfo.FileName = ConfigInfo.compressSwcBat;
                proc.StartInfo.WorkingDirectory = ConfigInfo.build;
                //proc.StartInfo.RedirectStandardOutput = true;
                //proc.StartInfo.UseShellExecute = false;
                //proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.Start();
                proc.WaitForExit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        private Tuple<Boolean,String> copyClientFile()
        {
            int copyListIndex = 0;
            int failNum = 0;
            int sucNum = 0;
            List<String> failList = new List<string>();
            FlaItem[] clientFileList = getClientFileList();
            CopyInfo info = null;
            for (int i = 0; i < clientFileList.Length; i++)
            {
                FlaItem item = clientFileList[i];
                try
                {
                    for (copyListIndex = 0; copyListIndex < ConfigInfo.clientCopyList.Length; copyListIndex++)
                    {
                        info = ConfigInfo.clientCopyList[copyListIndex];
                        if (Path.GetExtension(item.fullPath) == ".xml")
                        {
                            XmlDocument xmlDoc = ConfigInfo.getXmlDoc(item.fullPath);
                            if (info.type == "ftp")
                            {
                                copyXmlToFtp(xmlDoc, info.path, Path.Combine(info.path, item.assetsPath), info.user, info.pw);
                            }
                            else
                            {
                                xmlDoc.Save(Path.Combine(info.path, item.assetsPath));
                            }
                        }
                        else
                        {
                            if (info.type == "ftp")
                            {
                                copyFileToFtp(info.path, item.fullPath, Path.Combine(info.path, item.assetsPath), info.user, info.pw);
                            }
                            else
                            {
                                copyTo(ConfigInfo.assets, info.path, item.assetsPath);
                            }
                        }
                    }
                    sucNum++;
                }
                catch (Exception error)
                {
                    failNum++;
                    failList.Add(item.dis);
                }
            }
            String msg = "";
            Boolean isSuc = false;
            if (failNum > 0)
            {
                msg = "上传完成。成功" + sucNum + "个，失败" + failNum + "个。失败列表如下:\r\n";
                for (int i = 0; i < failList.Count; i++)
                {
                    msg += failList[i] + "\r\n";
                }
            }
            else
            {
                isSuc = true;
                msg = "上传成功，没有出现错误。";
            }
            return new Tuple<Boolean, String>(isSuc, msg);
        }


        private void copyXmlToFtp(XmlDocument doc, String baseRoot, String targetFile, String userName, String pw)
        {
            targetFile = targetFile.Replace("\\", "/");//把符号换成ftp符号

            Uri uri = new Uri(targetFile);

            String targetDir = getFileDir(targetFile);
            FtpCheckDirectoryExist(baseRoot, targetDir.Substring(baseRoot.Length), userName, pw);
            //creatFtpRoot(targetDir, userName, pw);

            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(uri);
            req.Credentials = new NetworkCredential(userName, pw);
            req.Method = WebRequestMethods.Ftp.UploadFile;
            req.Timeout = 10 * 1000;
            req.KeepAlive = false;

            Stream stream = req.GetRequestStream();
            doc.Save(stream);

            stream.Close();
            stream.Dispose();

            req.Abort();
        }


        private void copyFileToFtp(String baseRoot, String srcFile, String targetFile, String userName, String pw)
        {
            targetFile = targetFile.Replace("\\", "/");//把符号换成ftp符号

            FileInfo fi = new FileInfo(srcFile);
            FileStream fs = fi.OpenRead();
            Uri uri = new Uri(targetFile);

            String targetDir = getFileDir(targetFile);
            FtpCheckDirectoryExist(baseRoot, targetDir.Substring(baseRoot.Length), userName, pw);
            //creatFtpRoot(targetDir, userName, pw);

            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(uri);
            req.Credentials = new NetworkCredential(userName, pw);

            req.Method = WebRequestMethods.Ftp.UploadFile;
            req.ContentLength = fs.Length;
            req.Timeout = 10 * 1000;
            req.KeepAlive = false;

            Stream stream = req.GetRequestStream();
            int BufferLength = 2048; //2K
            byte[] b = new byte[BufferLength];
            int i;
            while ((i = fs.Read(b, 0, BufferLength)) > 0)
            {
                stream.Write(b, 0, i);
            }

            stream.Close();
            stream.Dispose();

            fs.Close();
            req.Abort();
        }


        private void FtpCheckDirectoryExist(String baseRoot, String createDir, String user, String pw)
        {
            string[] dirs = createDir.Split(new String[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            string curDir = "/";
            for (int i = 0; i < dirs.Length; i++)
            {
                string dir = dirs[i];
                //如果是以/开始的路径,第一个为空     
                if (dir != null && dir.Length > 0)
                {
                    curDir += dir + "/";
                    FtpMakeDir(baseRoot, curDir, user, pw);
                }
            }
        }


        private Boolean FtpMakeDir(String baseRoot, String localFile, String user, String pw)
        {
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(baseRoot + localFile);
            req.Credentials = new NetworkCredential(user, pw);
            req.Method = WebRequestMethods.Ftp.MakeDirectory;
            try
            {
                FtpWebResponse response = (FtpWebResponse)req.GetResponse();
                response.Close();
            }
            catch (Exception)
            {
                req.Abort();
                return false;
            }
            req.Abort();
            return true;
        }


        private string getFileDir(string s)
        {
            int i = s.LastIndexOf("/");
            return s.Substring(0, i + 1);
        }


        private void copyTo(String srcDir, String targetDir, String filePath)
        {
            String srcFile = Path.Combine(srcDir, filePath);
            String targetFile = Path.Combine(targetDir, filePath);
            copyTo(srcFile, targetFile);
        }


        private void copyTo(String srcFile, String targetFile)
        {
            FileInfo targetFi = new FileInfo(targetFile);
            if (targetFi.Directory.Exists == false)
            {
                targetFi.Directory.Create();//如果文件夹不存在，新建
            }
            if (targetFi.Exists && targetFi.IsReadOnly)
            {
                targetFi.IsReadOnly = false;//如果只读，修改为可修改
            }
            File.Copy(srcFile, targetFile, true);
        }

        private FlaItem[] getClientFileList()
        {
            FlaItem[] flaItemList = new FlaItem[uploadList.Length];
            for (int i = 0; i < uploadList.Length; i++)
            {
                FlaItem item = uploadList[i] as FlaItem;
                FileInfo fi = new FileInfo(item.fullPath);
                if (fi.Extension == ".fla")
                {
                    fi = new FileInfo(Path.ChangeExtension(fi.FullName, ".swf"));
                    item = new FlaItem(fi.FullName);
                }
                flaItemList[i] = item;
            }
            return flaItemList;
        }


        private FlaItem[] getFlaItemList(String[] fileList)
        {
            List<FlaItem> itemList = new List<FlaItem>();
            for (int i = 0; i < fileList.Length; i++)
            {
                itemList.Add(new FlaItem(fileList[i]));
            }
            return itemList.ToArray();
        }
    }
}
