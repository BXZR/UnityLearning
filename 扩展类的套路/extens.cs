using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class extens : MonoBehaviour {

 //扩展类的套路，感觉很神奇
	void Update ()
	{
		new theSuck ("suck_is_god").suckSuck ();
	}
}


public class theSuck
{
	public string theSuckString = "";
	public theSuck(string inin)
	{
		theSuckString = inin;
	}
	public void makeSuck()
	{
		Debug.Log ("dddddddd");
	}
}

//扩展类的方法，用一个静态类的静态方法传入this类型对象就可以扩展这个类的共有方法
public static class suckEx
{
	public static void suckSuck(this theSuck inin)
	{
		Debug.Log (inin.theSuckString +"_Extene");	
	}
}