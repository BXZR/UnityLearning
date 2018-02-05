using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class saver : MonoBehaviour {



	void saveXML()
	{
		datas toSave = new datas ();	
		toSave.theInformation = "SAVEMEMEMEME";
		XmlSerializer xmlSaver = new XmlSerializer (typeof(datas));
		FileStream fileSaver = new FileStream (@"D:\save.xml",FileMode.Create);
		xmlSaver.Serialize (fileSaver , toSave);
		fileSaver.Close ();
	}


	void loadXML()
	{
		if (!File.Exists (@"D:\save.xml"))
			return;

		XmlSerializer xmlLoad = new XmlSerializer (typeof(datas));
		FileStream fileLoader = new FileStream (@"D:\save.xml",FileMode .Open);
		datas theLoad =  xmlLoad.Deserialize (fileLoader) as datas;
		print ("loaded -> "+ theLoad .theInformation);
		fileLoader.Close ();
	}
 

	void saveBinary()
	{
		datas toSave = new datas ();
		toSave.theInformation = "SAVEMEMEMEME";
		BinaryFormatter binarySaver = new BinaryFormatter ();
		FileStream fileSaver = new FileStream (@"D:\save.binary",FileMode.Create);
		binarySaver.Serialize (fileSaver , toSave);
		fileSaver.Close ();
	}

	void loadBinary()
	{
		if (!File.Exists (@"D:\save.xml"))
			return;

		BinaryFormatter binaryLoader = new BinaryFormatter ();
		FileStream fileLoader = new FileStream (@"D:\save.binary",FileMode .Open);

		datas theLoad =  binaryLoader.Deserialize (fileLoader) as datas;
		print ("loaded -> "+ theLoad .theInformation);
		fileLoader.Close ();
	}

	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Q))
			saveXML ();
		if (Input.GetKeyDown (KeyCode.W))
			loadXML ();
		if (Input.GetKeyDown (KeyCode.E))
			saveBinary ();
		if (Input.GetKeyDown (KeyCode.R))
			loadBinary ();
	}
}

//这个结构不太好，仅仅作为一个示例存在
//用这个类保留应存储的东西
[System .Serializable]//标记为可序列化的
public class datas
{
	public  string theInformation = "";
	//有点坑爹的一点就是不能有构造器
	//否则编译器会报错
}