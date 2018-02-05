using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class FullMovie : MonoBehaviour {


	void OnGUI()
	{
		if (GUI.Button (new Rect (Screen.width / 3, Screen.height / 5, Screen.width / 3, 30), "播放"))
		{
			try
			{
				Handheld.PlayFullScreenMovie("LOOL.mp4");
		 
			}
			catch(Exception r)
			{
				print (r.ToString());
			}
		}
    }
	// Use this for initialization
	void Start () {
		AssetDatabase.Refresh (ImportAssetOptions.ForceSynchronousImport);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
 
}
