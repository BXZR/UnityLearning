using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class inini : MonoBehaviour {
	
	[System.Runtime.InteropServices.DllImport("kernel32")]
	private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
	[System.Runtime.InteropServices.DllImport("kernel32")]
	private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);

	void demo()
	{
		StringBuilder demos = new StringBuilder (255);
		WritePrivateProfileString ("section", "name", "theDemo", @"D:\theDemoini.ini");
		GetPrivateProfileString ("section" , "name" ,"" ,demos,255 , @"D:\theDemoini.ini");
		print (demos);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space))
			demo ();
	}
}
