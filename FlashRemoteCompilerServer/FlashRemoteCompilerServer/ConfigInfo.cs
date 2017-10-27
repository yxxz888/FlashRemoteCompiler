using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace FlashRemoteCompilerServer
{
    class ConfigInfo
    {
        private const String configFilePath = "config.xml";

        public static String flashPath;
        public static String build;
        public static String source;
        public static String assets;
        public static String project;
        public static String clientConfig;
        public static String music;
        public static String compileJsfl;//编译jsfl的路径
        public static String assetsForJsfl;//jsfl格式的assets路径
        public static String compileLog;//编译日志路径
        public static CopyInfo[] clientCopyList;//客户端拷贝列表
        public static String swcConfigPath;//swc配置路径
        public static SwcInfo[] swcConfig;//swc配置
        public static String buildSwcJsfl;//编译swc的jsfl路径（jsfl格式的路径）
        public static String compressSwcBat;//压缩swc的bat路径（jsfl格式的路径）

        public static String serverConfig;
        public static String buildServer;
        public static String serverPath;
        public static CopyInfo[] serverCopyList;//服务端拷贝列表

        public static ModifyXmlInfo[] modifyXmlList;//需要修改内容的xml文件

        private static XmlDocument doc;
        public static void initConfig()
        {
            doc = new XmlDocument();
            doc.Load(configFilePath);

            flashPath = getConfigValue("/config/client/flashExePath");
            build = getConfigValue("/config/client/build");
            source = getConfigValue("/config/client/source");
            assets = getConfigValue("/config/client/assets");
            project = getConfigValue("/config/client/project");
            clientConfig = getConfigValue("/config/client/config");
            music = getConfigValue("/config/client/music");
            clientCopyList = getCopyList("/config/client/copy/path");
            swcConfigPath = getConfigValue("/config/client/swcConfig");
            initSwcInfo();

            compileJsfl = getJsflPath(Path.Combine(Application.StartupPath, getConfigValue("/config/client/compileJsfl")), false);
            assetsForJsfl = getJsflPath(assets, true);
            compileLog = getJsflPath(Path.Combine(Application.StartupPath, getConfigValue("/config/client/compileLog")), false);
            buildSwcJsfl = getJsflPath(Path.Combine(Application.StartupPath, getConfigValue("/config/client/buildSwcJsfl")), false);
            compressSwcBat = getJsflPath(Path.Combine(build, "compress_swc.bat"), false);
            serverConfig = getConfigValue("/config/server/serverConfig");
            buildServer = getConfigValue("/config/server/buildServer");
            serverPath = getConfigValue("/config/server/serverBuildPath");
            serverCopyList = getCopyList("/config/server/copy/path");

            initModifyXmlList();
        }

        private static void initModifyXmlList()
        {
            XmlNodeList xnl = doc.SelectNodes("/config/modify/item");
            modifyXmlList = new ModifyXmlInfo[xnl.Count];
            int i = 0;
            foreach (XmlNode node in xnl)
            {
                modifyXmlList[i++] = new ModifyXmlInfo(node);
            }
        }

        public static XmlDocument getXmlDoc(String path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.Load(path);
            initDeclaration(xmlDoc);
            for (int i = 0; i < modifyXmlList.Length; i++)
            {
                if (modifyXmlList[i].isThisXml(path))
                {
                    modifyXmlList[i].modify(xmlDoc);
                }
            }
            return xmlDoc;
        }

        //添加头定义
        private static void initDeclaration(XmlDocument xmlDoc)
        {
            if (xmlDoc.FirstChild.NodeType != XmlNodeType.XmlDeclaration)
            {
                XmlDeclaration d = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                xmlDoc.InsertBefore(d, xmlDoc.FirstChild);
            }
        }

        private static CopyInfo[] getCopyList(String path)
        {
            XmlNodeList nodeList = doc.SelectNodes(path);
            CopyInfo[] list = new CopyInfo[nodeList.Count];
            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlNode node = nodeList[i];
                list[i] = new CopyInfo(
                    getAttributesMaybeNull(node, "type")
                    , getAttributesMaybeNull(node, "user")
                    , getAttributesMaybeNull(node, "pw")
                    , node.FirstChild.Value.ToString());
            }
            return list;
        }

        private static String getAttributesMaybeNull(XmlNode node, String name)
        {
            if (node.Attributes[name] == null)
            {
                return null;
            }
            else
            {
                return node.Attributes[name].Value;
            }
        }

        private static void initSwcInfo()
        {
            FileInfo fi = new FileInfo(swcConfigPath);
            StreamReader sr = fi.OpenText();

            ArrayList swcList = new ArrayList();
            while (true)
            {
                String line = sr.ReadLine();
                if (line == null)
                {
                    break;
                }
                String[] vals = line.Split(new String[] { "," }, StringSplitOptions.None);
                SwcInfo si = new SwcInfo(vals[0], vals[1], vals[2]);
                swcList.Add(si);
            }
            swcConfig = (SwcInfo[])swcList.ToArray(typeof(SwcInfo));
        }

        public static String getConfigValue(String path)
        {
            return doc.SelectSingleNode(path).FirstChild.Value.ToString();
        }

        public static String[] getConfigStringList(String path)
        {
            XmlNodeList nodeList = doc.SelectNodes(path);
            String[] list = new String[nodeList.Count];
            for (int i = 0; i < nodeList.Count; i++)
            {
                list[i] = nodeList[i].FirstChild.Value.ToString();
            }
            return list;
        }

        /// <summary>
        /// 获取路径的jsfl格式
        /// </summary>
        /// <param name="path">原始路径</param>
        /// <param name="isFolder">是否目录(自动补全/)</param>
        /// <returns></returns>
        public static String getJsflPath(String path, Boolean isFolder)
        {
            path = path.Replace(@"\", "/");
            path = path.Replace(":", "|");
            path = "file:///" + path;
            if (isFolder && path.Substring(path.Length - 1) != "/")
            {
                path = path + "/";
            }
            return path;
        }


        public static String getCSharpPath(String path)
        {
            String result = path.Clone() as String;
            result = result.Substring("file:///".Length);
            result = result.Replace("/", @"\");
            result = result.Replace("|", ":");
            return result;
        }
    }

    public class SwcInfo
    {
        public String name;
        public String dir;
        public String dest;
        public SwcInfo(String name, String dir, String dest)
        {
            this.name = name;
            this.dir = dir;
            this.dest = dest;
        }
    }

    public class CopyInfo
    {
        public String type;
        public String user;
        public String pw;
        public String path;
        public CopyInfo(String type, String user, String pw, String path)
        {
            this.type = type;
            this.user = user;
            this.pw = pw;
            this.path = path;
        }
    }
}
