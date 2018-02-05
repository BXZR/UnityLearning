using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABLoader : MonoBehaviour {

	//动态加载assetBundles资源的示例脚本
	private const string thePath = @"E:/ABSETs/";

	void load(string fileName,string theOBJName)
		{
		GameObject theLoadDemo = GameObject .Instantiate( AssetBundle.LoadFromFile (fileName).LoadAsset<GameObject>(theOBJName));
			theLoadDemo.transform.position = new Vector3 (0, 0, 0);
			theLoadDemo.name = "loadWithPath";
		}
	
	
	     IEnumerator loadWithWWW()
		{
		    string fileName = @"file:///E:\ABSETs\g";
		   string theOBJName = "Cube";
			WWW w = new WWW (fileName);
		   yield return w;
		    GameObject theLoadDemo =  GameObject .Instantiate( w.assetBundle.LoadAsset<GameObject> (theOBJName) );
			theLoadDemo.transform.position = new Vector3(0,0,0);
			theLoadDemo.name = "loadWithWWW";
		}
			
		void Update ()
		{
			if(Input .GetKeyDown(KeyCode .Space))
			{
			load(thePath +"g" , "Cube");
			}
			if (Input.GetKeyDown (KeyCode.L))
			{
			StartCoroutine  (    "loadWithWWW" );
			}
	 
		}
}
