﻿//编译文件
//root 根路径 例子file:///D|/vstsworkspace/mmo/
//fileList 文件地址列表 例子assets/hp/matchinfo.fla
//logPath log文件地址 file:///D|/vstsworkspace/mmo/log.txt
function compile(root, fileList, logPath, failFilePath, compileHistoryPath){
	root += "assets/";
	var tracePath = getPath(logPath) + "publish_trace.txt";
	var errorPath = getPath(logPath) + "publish_error.txt";

	for(var i = 0;i < fileList.length;i++){
		var filePath = root + fileList[i];
		FLfile.write(compileHistoryPath, filePath + "\r\n", "append");

		if(fl.fileExists(filePath) == false)
		{
			traceLog("\r\n找不到文件:\r\n" + filePath + "\r\n", logPath);
			logFailFile(filePath,failFilePath);
			continue ;
		}
		var doc = fl.openDocument(filePath);		
		setVersion(doc);
		doc.publish();
		fl.closeDocument(doc,false);
		FLfile.remove(tracePath);
		FLfile.remove(errorPath);
		fl.outputPanel.save(tracePath);
		fl.compilerErrors.save(errorPath);
		var traceMsg = FLfile.read(tracePath);
		var errorMsg = FLfile.read(errorPath);
		if(traceMsg.length > 0|| (errorMsg.length > 0 && errorMsg != "0 个错误, 0 个警告")){
			traceLog("\r\n编译出现异常:\r\n" + filePath + "\r\n", logPath);
			traceLog(traceMsg + "\r\n", logPath);
			traceLog(errorMsg + "\r\n", logPath);
			traceLog("===============\r\n", logPath);
			logFailFile(filePath,failFilePath);
		}
		FLfile.remove(tracePath);
		FLfile.remove(errorPath);
	}

	fl.closeAll();
	fl.quit(false);
}

function traceLog(msg, logPath){
	FLfile.write(logPath, msg, "append");
}

function logFailFile(filePath, logPath){
	FLfile.write(logPath, filePath + "\r\n", "append");
}

//获取文件路径的目录部分
function getPath(filePath){
	var index = filePath.lastIndexOf("/");
	return filePath.substring(0, index + 1);
}

function setVersion(doc)
{
//	if(doc.name == "commoninit.fla"||doc.name == "scenecommon.fla")
//	{
//		return;
//	}
	var profileXML = doc.exportPublishProfileString();
	
	profileXML = profileXML.replace(/<Version>.*<\/Version>/,"<Version>18</Version>");
	//profileXML = profileXML.replace("<Version>9</Version>","<Version>18</Version>");
	//profileXML = profileXML.replace("<Version>15</Version>","<Version>18</Version>");
	//profileXML = profileXML.replace("<ExternalPlayer>FlashPlayer10</ExternalPlayer>","<ExternalPlayer>FlashPlayer11.5</ExternalPlayer>");
	//profileXML = profileXML.replace("<ExternalPlayer>FlashPlayer9</ExternalPlayer>","<ExternalPlayer>FlashPlayer11.5</ExternalPlayer>");
	profileXML = profileXML.replace(/<ExternalPlayer>FlashPlayer.*<\/ExternalPlayer>/,"<ExternalPlayer>FlashPlayer11.5</ExternalPlayer>");
	profileXML = profileXML.replace("<ExternalPlayer></ExternalPlayer>","<ExternalPlayer>FlashPlayer11.5</ExternalPlayer>");
	profileXML = profileXML.replace("<DebuggingPermitted>1</DebuggingPermitted>","<DebuggingPermitted>0</DebuggingPermitted>");
	if(profileXML.indexOf("<ExportSwc>1</ExportSwc>") < 0){
	   profileXML = profileXML.replace(/<CompressionType>.*<\/CompressionType>/,"<CompressionType>1</CompressionType>");
	}

	profileXML = loopSetLinkageExternal( profileXML );

	fl.getDocumentDOM().importPublishProfileString(profileXML);
}



function loopSetLinkageExternal( profileXML ){
	var merge = "<linkage>merge</linkage>";
	var external = "<linkage>external</linkage>";
	while( profileXML.indexOf( merge ) != -1 )
	{
		profileXML = profileXML.replace(merge, external);
	}
	return profileXML;
}