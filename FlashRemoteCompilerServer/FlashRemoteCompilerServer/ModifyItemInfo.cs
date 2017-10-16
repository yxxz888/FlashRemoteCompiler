using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FlashRemoteCompilerServer
{
    class ModifyItemInfo
    {
        public static String actionModify = "modify";
        public static String actionDel = "del";

        private string action;
        private string path;
        //检查条件列表[key, value]
        private ArrayList checkList = new ArrayList();
        //修改结果列表[key, value]
        private ArrayList valueList = new ArrayList();
        public ModifyItemInfo(XmlNode node)
        {
            path = node.Attributes["path"].Value.ToString();
            action = node.Attributes["action"].Value.ToString();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                String type = node.ChildNodes[i].Name.ToString();
                if (type == "check")
                {
                    checkList.Add(new String[]{
                        node.ChildNodes[i].Attributes["key"].Value.ToString(),
                         node.ChildNodes[i].Attributes["value"].Value.ToString()
                    });
                }
                else if (type == "value")
                {
                    valueList.Add(new String[]{
                        node.ChildNodes[i].Attributes["key"].Value.ToString(),
                         node.ChildNodes[i].Attributes["value"].Value.ToString()
                    });
                }
            }
        }

        public void modify(XmlDocument doc)
        {
            if (action == actionModify)
            {
                doModify(doc);
            }
            else if (action == actionDel)
            {
                doDel(doc);
            }
        }

        private void doModify(XmlDocument doc)
        {
            XmlNodeList list = doc.SelectNodes(path);
            for (int i = 0; i < list.Count; i++)
            {
                XmlNode node = list[i];
                Boolean isCheck = false;
                for (int checkI = 0; checkI < checkList.Count; checkI++)
                {
                    String checkKey = (checkList[checkI] as String[])[0].ToString();
                    if (checkKey == "" && node.FirstChild.Value == (checkList[checkI] as String[])[1].ToString())
                    {
                        isCheck = true;
                        break;
                    }
                    else if (node.Attributes[checkKey].Value == (checkList[checkI] as String[])[1].ToString())
                    {
                        isCheck = true;
                        break;
                    }
                }

                if (isCheck)
                {
                    XmlElement xe = (XmlElement)node;
                    String valueKey = (valueList[0] as String[])[0];
                    if (valueKey == "")
                    {
                        xe.InnerText = (valueList[0] as String[])[1];
                    }
                    else
                    {
                        xe.SetAttribute((valueList[0] as String[])[0], (valueList[0] as String[])[1]);
                    }
                }
            }
        }

        private void doDel(XmlDocument doc)
        {
            XmlNodeList list = doc.SelectNodes(path);
            for (int i = 0; i < list.Count; i++)
            {
                XmlNode node = list[i];
                Boolean isCheck = false;
                for (int checkI = 0; checkI < checkList.Count; checkI++)
                {
                    String checkKey = (checkList[checkI] as String[])[0].ToString();
                    if (checkKey == "" && node.FirstChild.Value == (checkList[checkI] as String[])[1].ToString())
                    {
                        isCheck = true;
                        break;
                    }
                    else if (node.Attributes[checkKey].Value == (checkList[checkI] as String[])[1].ToString())
                    {
                        isCheck = true;
                        break;
                    }
                }

                if (isCheck)
                {
                    node.ParentNode.RemoveChild(node);
                }
            }
        }
    }
}
