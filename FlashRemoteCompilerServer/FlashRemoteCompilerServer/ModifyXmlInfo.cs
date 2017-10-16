using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FlashRemoteCompilerServer
{
    class ModifyXmlInfo
    {
        private String path;
        private ModifyItemInfo[] itemInfoList;
        public ModifyXmlInfo(XmlNode node)
        {
            path = node.Attributes["path"].Value.ToString();
            itemInfoList = new ModifyItemInfo[node.ChildNodes.Count];
            ArrayList infoList = new ArrayList();
            for (int i = 0; i < itemInfoList.Length; i++)
            {
                if (node.ChildNodes[i].NodeType != XmlNodeType.Comment)
                {
                    infoList.Add(new ModifyItemInfo(node.ChildNodes[i]));
                }
            }
            itemInfoList = (ModifyItemInfo[])infoList.ToArray(typeof(ModifyItemInfo));
        }

        public Boolean isThisXml(String path)
        {
            return this.path == path;
        }

        public void modify(XmlDocument doc)
        {
            for (int i = 0; i < itemInfoList.Length; i++)
            {
                itemInfoList[i].modify(doc);
            }
        }
    }   
}
