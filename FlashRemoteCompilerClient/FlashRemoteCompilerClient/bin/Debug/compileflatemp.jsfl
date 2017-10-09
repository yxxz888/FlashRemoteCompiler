//编译一批fla的jsfl模版
var docList = [
/替代列表/
];

startCompile();

function startCompile(){
	compileFlas(docList);

	closeFlash();
}

function closeFlash(){
	fl.closeAll();
	fl.quit(false);
}

function compileFlas(docList){
	for(var i = 0;i < docList.length;i++){
		var doc = fl.openDocument(docList[i]);
		setDocLinkageExternal( doc );
		doc.publish();
		fl.closeDocument(doc, false);
	}
}

function setDocLinkageExternal( doc ){
	var doc = fl.getDocumentDOM();
	var profileXML = doc.exportPublishProfileString();
	profileXML = loopSetLinkageExternal( profileXML );
	doc.importPublishProfileString(profileXML);

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