using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stringddd : MonoBehaviour {
 

	void Update () 
	{
		//非常神奇的就是这个居然扩展了string的方法
		string su = "kkk";
		print( su.check ());
		su = " ";
		print( su.check ());
		print (su.make());
	}
}

//这个类是string类的扩展类
//可以通过string的对象直接调用，调用方法参见上面的例子
//方法功能为如果字符串为空或者全是空格，返回true
public static class StringExtensions
{
	public static bool check(this string theString)
	{
		return theString == null || theString.Trim ().Length == 0;
		//如果为空或者全是空格就饿返回true
	}

	public static string  make(this string theString)
	{
		return theString +"_suck";
	}
}
