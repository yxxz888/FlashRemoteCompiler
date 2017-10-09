using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FlashRemoteCompilerClient
{
    class FileItem
    {
        public String realPath;
        public String relativePath;
        public String fileName;

        public FileItem(string realPath)
        {
            this.realPath = realPath;
            FileInfo info = new FileInfo(realPath); 
            this.fileName = info.Name;
            relativePath = realPath.Substring(FlashRemoteCompilerClient.assetsPath.Length);
        }


        public static FileItem fromXmlNode(XmlNode node)
        {
            FileItem result = null;
            String path = node.Attributes["path"].Value;
            String fileType = node.Attributes["filetype"].Value;
            if (fileType == "fla")
            {
                path = path.Replace("../", "");
                path = path.Replace("/", "\\");
                String realPath = Path.Combine(FlashRemoteCompilerClient.sourcePath, path);
                result = new FileItem(realPath);
            }
            return result;
        }


        public String displayName
        {
            get
            {
                return fileName;
            }
        }
    }
}
