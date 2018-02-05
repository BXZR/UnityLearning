using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class opopop : MonoBehaviour {
	/******************下面的内容是用于测试的内容***************************/
	void showIE()
	{
		print ("n");
		//用迭代器
		zoos theFirst = zoos.theFirstZoos;
		if (theFirst != null) 
		{
			foreach (zoos theZ in theFirst)//因为实现了迭代器接口，所以可以用这种方式处理了
				print (theZ.theName);
		} 
		else 
		{
			print ("gg");
		}
	}

	void showLink()
	{
		print ("n1");
		IEnumerator i = zoos.theFirstZoos.GetEnumerator();
		while (i.MoveNext ())
			print (((zoos)i.Current).theName );
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.L))
			showLink ();
		if (Input.GetKeyDown (KeyCode.K)) 
			showIE ();
	}

}
