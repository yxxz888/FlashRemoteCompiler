compile();
function compile(){
var fileList = [];
var log = "file:///D|/vstsworkspace/mmo/source/tools/FlashRemoteCompiler/FlashRemoteCompilerServer/FlashRemoteCompilerServer/bin/Debug/log.txt";
var path = "file:///D|/vstsworkspace/mmo/build/buildsomeswc.jsfl";
var buildSwcRes = fl.runScript(path, "buildSwc", fileList, false);
FLfile.remove(log);
if(buildSwcRes != "true"){
FLfile.write(logPath, "编译swc失败", "append");
fl.closeAll();
fl.quit(false);
return;
}
var fileList = [];
fileList.push("activity/20171027/fffadventure.fla");
fileList.push("scene/scene1.fla");
var root="file:///D|/vstsworkspace/mmo/source/assets/";
var path = "file:///D|/vstsworkspace/mmo/source/tools/FlashRemoteCompiler/FlashRemoteCompilerServer/FlashRemoteCompilerServer/bin/Debug/compile.jsfl";
fl.runScript(path, "compile", root, fileList, log);
}