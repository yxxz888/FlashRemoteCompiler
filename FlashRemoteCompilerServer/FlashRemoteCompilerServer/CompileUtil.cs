﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashRemoteCompilerServer
{
    class CompileUtil
    {
        public delegate void CompileFinishedCallback();

        private CompileFinishedCallback onCompileFinished;

        class FindFlaListObj
        {
            public SwcInfo[] swcList;
            public FlaItem[] flaList;
        }


        public void compileFla(FlaItem[] fileList,CompileFinishedCallback callback)
        {
            onCompileFinished = callback;

            //如果已经Flash软件已经打开，则后面的编译脚本不能正确结束，所以先把Flash关闭一次。
            closeFlash();

            FindFlaListObj res = getSwcFlaList(fileList);

            String script = "";
            script += "compile();\r\n"
                + "function compile(){\r\n";
            script += getCompileSwcScript(res.swcList);
            script += getCheckSwcOkScript();
            script += getCompileFlaScript(res.flaList);
            script += "}";

            String jsflFileName = "CompileTool_temRunScript.jsfl";
            String jsflPath = Path.Combine(ConfigInfo.build, jsflFileName);
            String backupPath = Path.Combine(Application.StartupPath, jsflFileName);

            StreamWriter sw = new StreamWriter(jsflPath, false, Encoding.UTF8);
            sw.Flush();
            sw.Write(script);
            sw.Close();

            //由于临时jsfl保存到build，而且编完就删，所以这里保存一个备份
            copyTo(jsflPath, backupPath);
            Console.WriteLine(jsflPath);
            Process p = new Process();
            p.StartInfo.FileName = "\"" + jsflPath + "\"";
            p.Start();
            p.WaitForExit();
            p.Close();

            onCompileFinished();
        }

       
        private void closeFlash()
        {
            String jsflPath = Path.Combine(Application.StartupPath, "closeflash.jsfl");
            Process p = new Process();
            p.StartInfo.FileName = "\"" + jsflPath + "\"";
            p.Start();
            p.WaitForExit();
            p.Close();
        }


        private FindFlaListObj getSwcFlaList(FlaItem[] flaList)
        {
            ArrayList swcList = new ArrayList();
            ArrayList flaResList = new ArrayList(flaList);
            for (int i = 0; i < ConfigInfo.swcConfig.Length; i++)
            {
                for (int j = flaResList.Count - 1; j >= 0; j--)
                {
                    if (ConfigInfo.swcConfig[i].name + ".fla" == (flaResList[j] as FlaItem).dis
                        && (flaResList[j] as FlaItem).fullPath.IndexOf(ConfigInfo.swcConfig[i].dir) != -1)
                    {
                        swcList.Add(ConfigInfo.swcConfig[i]);
                        flaResList.RemoveAt(j);
                        break;
                    }
                }
            }
            FindFlaListObj res = new FindFlaListObj();
            res.swcList = (SwcInfo[])swcList.ToArray(typeof(SwcInfo));
            res.flaList = (FlaItem[])flaResList.ToArray(typeof(FlaItem));
            return res;
        }


        private String getCompileSwcScript(SwcInfo[] swcList)
        {
            String fileListStr = "";
            for (int i = 0; i < swcList.Length; i++)
            {
                fileListStr += "fileList.push({name:\""
                + swcList[i].name
                + "\",src:\""
                + swcList[i].dir
                + "\",dest:\""
                + swcList[i].dest
                + "\"});\r\n";
            }

            String script = "var fileList = [];\r\n"
                + fileListStr
                + "var root=\"" + ConfigInfo.sourceForJsfl + "\";\r\n"
                + "var logPath = \"" + ConfigInfo.compileLog + "\";\r\n"
                + "var failFilePath = \"" + ConfigInfo.failFileLog + "\";\r\n"
                + "var compileHistoryPath = \"" + ConfigInfo.compileHistoryPath + "\";\r\n"
                + "FLfile.remove(logPath);\r\n"
                + "FLfile.remove(failFilePath);\r\n"
                + "FLfile.remove(compileHistoryPath);\r\n"
                + "var path = \"" + ConfigInfo.buildSwcJsfl + "\";\r\n"
                + "var buildSwcRes = fl.runScript(path, \"buildSwc\", root, fileList, logPath, compileHistoryPath);\r\n";
            return script;
        }

        private String getCheckSwcOkScript()
        {
            return "if(buildSwcRes != \"true\"){\r\n"
                + "fl.closeAll();\r\n"
                + "fl.quit(false);\r\n"
                + "return;\r\n"
                + "}\r\n";
        }

        private String getCompileFlaScript(FlaItem[] flaList)
        {
            String fileListStr = "";
            for (int i = 0; i < flaList.Length; i++)
            {
                fileListStr += "fileList.push(\""
                + flaList[i].assetsPathForJsfl
                + "\");\r\n";
            }

            String script = "var fileList = [];\r\n"
                + fileListStr
                + "var path = \"" + ConfigInfo.compileJsfl + "\";\r\n"
                + "fl.runScript(path, \"compile\", root, fileList, logPath, failFilePath, compileHistoryPath);\r\n";

            return script;
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
    }
}
