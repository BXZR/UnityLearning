using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class uguiTxt : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		string text1 = "<color=#FF0099>教主兽</color>超进化<color=#FFFF00>神圣教主兽</color>！！！\n";
		string text2 = "教主兽超进化[FF0099]神圣教主兽[000000]！！！";
		this.GetComponent <Text> ().text = text1 + text2;;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
