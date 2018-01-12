compile();
function compile(){
var fileList = [];
var root="file:///D|/vstsworkspace/mmo/source/";
var logPath = "file:///D|/vstsworkspace/mmo/source/tools/FlashRemoteCompiler/FlashRemoteCompilerServer/FlashRemoteCompilerServer/bin/Debug/log.txt";
var failFilePath = "file:///D|/vstsworkspace/mmo/source/tools/FlashRemoteCompiler/FlashRemoteCompilerServer/FlashRemoteCompilerServer/bin/Debug/fail_file.txt";
var compileHistoryPath = "file:///D|/vstsworkspace/mmo/source/tools/FlashRemoteCompiler/FlashRemoteCompilerServer/FlashRemoteCompilerServer/bin/Debug/compileHistory.txt";
FLfile.remove(logPath);
FLfile.remove(failFilePath);
FLfile.remove(compileHistoryPath);
var path = "file:///D|/vstsworkspace/mmo/source/tools/FlashRemoteCompiler/FlashRemoteCompilerServer/FlashRemoteCompilerServer/bin/Debug/buildsomeswc.jsfl";
var buildSwcRes = fl.runScript(path, "buildSwc", root, fileList, logPath, compileHistoryPath);
if(buildSwcRes != "true"){
fl.closeAll();
fl.quit(false);
return;
}
var fileList = [];
fileList.push("activity/20171013/fanssupport.fla");
fileList.push("activity/20171013/fanssupportgame.fla");
fileList.push("scene/partybeach.fla");
fileList.push("scene/scene1.fla");
var path = "file:///D|/vstsworkspace/mmo/source/tools/FlashRemoteCompiler/FlashRemoteCompilerServer/FlashRemoteCompilerServer/bin/Debug/compile.jsfl";
fl.runScript(path, "compile", root, fileList, logPath, failFilePath, compileHistoryPath);
}