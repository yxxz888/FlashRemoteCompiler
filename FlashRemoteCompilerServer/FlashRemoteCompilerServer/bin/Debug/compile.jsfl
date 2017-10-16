//编译文件
//root 根路径 例子file:///D|/vstsworkspace/mmo/
//fileList 文件地址列表 例子assets/hp/matchinfo.fla
//logPath log文件地址 file:///D|/vstsworkspace/mmo/log.txt
function compile(root, fileList, logPath){
	var tracePath = getPath(logPath) + "publish_trace.txt";
	var errorPath = getPath(logPath) + "publish_error.txt";
	FLfile.remove(logPath);
	for(var i = 0;i < fileList.length;i++){
		var filePath = root + fileList[i];
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
			traceLog("编译文件异常:" + filePath + "\r\n", logPath);
			traceLog(traceMsg + "\r\n", logPath);
			traceLog(errorMsg + "\r\n", logPath);
			traceLog("===============\r\n\r\n", logPath);
			if(confirm("编译错误或警告，是否中断？")){
				break;
			}
		}
		FLfile.remove(tracePath);
		FLfile.remove(errorPath);
	}
	
	var out = FLfile.read(logPath);
	if(out.length > 0){
		var msg = "编译完成，发生了一些编译错误或警告。";
		fl.trace(msg);
		if(confirm(msg + "\n是否现在查看这些错误？")){
			fl.openScript(logPath);
		}
	}else{
		fl.trace("编译完成，没有任何错误。");
	}
}

function traceLog(msg, logPath){
	FLfile.write(logPath, msg, "append");
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