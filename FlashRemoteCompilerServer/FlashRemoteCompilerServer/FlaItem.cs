using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashRemoteCompilerServer
{
    class FlaItem
    {
        public String dis;
        public String assetsPathForJsfl;//以assets/(不含assets/)为根目录的路径，用/分隔符
        public String fullPath;//全路径
        public String assetsPath;//以assets/(不含assets/)为根目录的路径
        public FlaItem()
        {
        }

        public FlaItem(String fullPath)
        {
            FileInfo info = new FileInfo(fullPath);
            this.dis = info.Name;
            this.fullPath = fullPath;

            String assetsName = ConfigInfo.assets;
            int index = fullPath.IndexOf(assetsName);
            if (index != -1)
            {
                assetsPath = fullPath.Substring(index + assetsName.Length);
                if (assetsPath.IndexOf("\\") == 0)
                {
                    assetsPath = assetsPath.Substring(1);//去掉前面的\\符号，这样才能成功合并路径
                }
            }
            else
            {
                assetsPath = dis;
            }

            assetsPathForJsfl = assetsPath.Replace("\\", "/");
        }

        public override string ToString()
        {
            return dis.ToString();
        }
    }
}
