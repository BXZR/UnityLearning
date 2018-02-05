using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BeginSS : MonoBehaviour {

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.H))
			makeCC ();

		if (Input.GetKeyDown (KeyCode.G))
			StartCoroutine (theReturnDemo());

		if (Input.GetKeyDown (KeyCode.S))
			StartCoroutine (theRunDemo());
	}

	//这是一个协程，参数为Action类的对象，也就是一个方法
	//此外还有一个延迟时间
	IEnumerator theDelayDemo(Action dos , float delayTime)
	{
		yield return new WaitForSeconds (delayTime);
		dos();//直接调用被用作参数的Action对象，当做方法用
	}

	void makeCC()
	{
		//将一个匿名方法作为一个参数传入，在协程中直接调用
		//注意这个匿名方法的写法
		StartCoroutine (theDelayDemo (   ()=> {print ("This is the method!");}   ,3f ) );
	}

	/////////////////////

	//可以返回为null，立即进入到下一步，也就是输出"dddd"
	IEnumerator theReturnDemo()
	{
		yield return null;
		print ("dddd");
		yield return new WaitForSeconds (1);
		print ("CCCC");
	}

	/////////////////////

	//monoBeahaviour的enabled == false不会关掉这个协程
	//gameObject . setActIve（false） 可以打断这个协程，再次打开也不会继续
	//说明协程和monoBehaviour是一个层次的，但是都被gameObject管辖	
	IEnumerator theRunDemo()
	{
		for (int i = 0; i < 100000; i++)
		{
			//WaitForSeconds 收到Time.timeScale影响
			yield return new WaitForSeconds (0.5f);
			print ("Suck"+i);
		}
	}
}
