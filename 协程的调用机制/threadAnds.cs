using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class threadAnds : MonoBehaviour {

	//这个脚本将用来展现协程的使用实现机制
	//先说结论：用yield返回只是将后面的代码暂时挂起，程序会运行协程之外的下面的代码直到约定的时间

	IEnumerator theDemo()
	{
			print ("Step1");//这个就是协程没有等待的第一句调用，立刻执行
		yield return new WaitForSeconds (1.7f);//进入等待阶段，于是程序开始调用协程之外的第一句
			print ("Step2");//等待一段时间之后就可以
		//顺带一提，当协程运行到最后，那么这个协程也就关闭了，这个跟调用方法是一样的
	}


	void check()
	{
		print ("Step0");//这个在协程开启之前调用
		StartCoroutine (theDemo());//开启协程，但是实际上是类似调用方法的处理
		print ("Step3");//在协程等待的时候立刻调用
		Thread.Sleep (2000);//主线程等待2s,参数单位为毫秒的
		print ("Step4");//因为主线停止之后复活，运行的是这句话
		//因此可以得到结论：协程也运行在主线程，随着主线程的休眠休眠了2s
	}

	//此外有一个值得注意的现象：
	//所有的输出都是在等待2s之后才出现在控制台的，这里估计是Unity控制台在这里有缓冲区等等的优化问题

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Space))
			check ();
	}
}
