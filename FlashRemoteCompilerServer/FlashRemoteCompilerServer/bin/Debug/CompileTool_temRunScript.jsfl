﻿compile();
function compile(){
var fileList = [];
var root="file:///D|/vstsworkspace/mmo/source/assets/";
var logPath = "file:///D|/vstsworkspace/mmo/source/tools/FlashRemoteCompiler/FlashRemoteCompilerServer/FlashRemoteCompilerServer/bin/Debug/log.txt";
FLfile.remove(logPath);
var path = "file:///D|/vstsworkspace/mmo/source/tools/FlashRemoteCompiler/FlashRemoteCompilerServer/FlashRemoteCompilerServer/bin/Debug/buildsomeswc.jsfl";
var buildSwcRes = fl.runScript(path, "buildSwc", root, fileList, logPath);
if(buildSwcRes != "true"){
fl.closeAll();
fl.quit(false);
return;
}
var fileList = [];
fileList.push("scene/scene1.fla");
var path = "file:///D|/vstsworkspace/mmo/source/tools/FlashRemoteCompiler/FlashRemoteCompilerServer/FlashRemoteCompilerServer/bin/Debug/compile.jsfl";
fl.runScript(path, "compile", root, fileList, logPath);
}