var assetsURL;

function buildSwc(root,buildList,logPath){
	assetsURL = root;
	var buildSuc = true;
	FLfile.remove(logPath);

	for (var fileIndex = 0; fileIndex < buildList.length; fileIndex++)
	{
		var config = buildList[fileIndex];

		if(!exportSWC(config,logPath)){
			buildSuc = false;
			break;
		}
	}
	
	return buildSuc;
}


function exportSWC(config,logPath){

	var tracePath = getPath(logPath) + "publish_trace.txt";
	var errorPath = getPath(logPath) + "publish_error.txt";

	var fileUrl = assetsURL;
	var localSwcUrl = assetsURL;
	if(config.src != ""){
		fileUrl += "/" + config.src
		localSwcUrl += "/" + config.src
	}
	fileUrl += "/" + config.name + ".fla";	
	localSwcUrl += "/" + config.name + ".swc";
	fl.trace(fileUrl);
	var doc = fl.openDocument(fileUrl);
	setVersion(doc);
	doc.publish();
	doc.close(false);
	fl.compilerErrors.save(errorPath);
	fl.outputPanel.save(tracePath);
	var s = FLfile.read(errorPath);
	var traceMsg = FLfile.read(tracePath);
	if(s != null && s.length > 0 && s != "0 个错误, 0 个警告" || traceMsg.length > 0){

		traceLog("\r\n编译swc出现异常:\r\n" + fileUrl + "\r\n", logPath);
		traceLog(traceMsg + "\r\n", logPath);
		traceLog(s + "\r\n", logPath);
		traceLog("===============\r\n", logPath);
		return false;
	}
	// copy
	var globalSwcUrl = asURL + "/" + config.name + ".swc";;
	
	if(config.dest == 2)
	{
		FLfile.remove(globalSwcUrl);
		FLfile.copy(localSwcUrl, globalSwcUrl);
	}
	if(!FLfile.exists(asURL)){
			FLfile.createFolder(asURL);
		}
	var swcFolder = asURL + "/swc/" + config.name;
	if(!FLfile.exists(swcFolder)){
		FLfile.createFolder(swcFolder);
	}	
	var folderSwcUrl = swcFolder + "/" + config.name + ".swc";
	FLfile.remove(folderSwcUrl);
	FLfile.copy(localSwcUrl, folderSwcUrl);
	return true;
}


function setVersion(doc)
{

	var profileXML = doc.exportPublishProfileString();
	
	profileXML = profileXML.replace(/<Version>.*<\/Version>/,"<Version>18</Version>");
	//profileXML = profileXML.replace("<Version>9</Version>","<Version>18</Version>");
	//profileXML = profileXML.replace("<Version>15</Version>","<Version>18</Version>");
	//profileXML = profileXML.replace("<ExternalPlayer>FlashPlayer10</ExternalPlayer>","<ExternalPlayer>FlashPlayer11.5</ExternalPlayer>");
	//profileXML = profileXML.replace("<ExternalPlayer>FlashPlayer9</ExternalPlayer>","<ExternalPlayer>FlashPlayer11.5</ExternalPlayer>");
	profileXML = profileXML.replace(/<ExternalPlayer>FlashPlayer.*<\/ExternalPlayer>/,"<ExternalPlayer>FlashPlayer11.5</ExternalPlayer>");
	profileXML = profileXML.replace("<ExternalPlayer></ExternalPlayer>","<ExternalPlayer>FlashPlayer11.5</ExternalPlayer>");
	profileXML = profileXML.replace("<DebuggingPermitted>1</DebuggingPermitted>","<DebuggingPermitted>0</DebuggingPermitted>");

	profileXML = loopSetLinkageExternal( profileXML );

	fl.getDocumentDOM().importPublishProfileString(profileXML);
}


/**
循环把所有库路径，链接方式，从【合并到代码】变成【外部】
避免把外部库代码签入到活动fla里面
*/
function loopSetLinkageExternal( profileXML ){
	var merge = "<linkage>merge</linkage>";
	var external = "<linkage>external</linkage>";
	while( profileXML.indexOf( merge ) != -1 )
	{
		profileXML = profileXML.replace(merge, external);
	}
	return profileXML;
}


function traceLog(msg, logPath){
	FLfile.write(logPath, msg, "append");
}


//获取文件路径的目录部分
function getPath(filePath){
	var index = filePath.lastIndexOf("/");
	return filePath.substring(0, index + 1);
}


