using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class TalkCanvas : UIBasic {

	//显示对话
	XmlDocument xml = new XmlDocument();
	XmlNodeList theXmlList;



	void OnEnable()
	{
		
	}

	void Update ()
	{
		
	}

	public override void OnShow (string value = "")
	{
		TextAsset textAsset = (TextAsset)Resources.Load ("XML/" + value);
		xml.LoadXml (textAsset.text);
		theXmlList = xml.SelectNodes ("Root/Dialog");
		foreach (XmlNode node in theXmlList) 
		{
			print (node.SelectSingleNode("Name").InnerText);
			print (node.SelectSingleNode("Picture").InnerText);
			print (node.SelectSingleNode("Information").InnerText);
		}
	} 
}
