using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//回调方法的学习
//似乎，其原理是用到了委托
//委托，用这个存储方法的指针用于调用
//能够把函数搞成类似变量使用



//有点像自定义类
public delegate void theMethodUse (string information);
//使用回调的原因
//你想让别人的代码执行你的代码，而别人的代码你又不能动
//在C#中有的线程没办法直接调用主线程的方法，所以需要用到回调

public class callBackCheck : MonoBehaviour {

	theMethodUse theMethodUser ;//委托的“对象”

	void Start ()
	{
		step2 ();
	}

	void step1()//回调第一段，调用本类方法
	{
		theMethodUser = new theMethodUse (makeShow);//存入一个方法
		theMethodUser ("这是回调的第一阶段");//把这个对象当做“方法”使用
	}

	void step2()//回调第二段，调用别的类方法
	{
		theMethodUser = new theMethodUse (new illusion().makeIllusion);//存入一个方法
		theMethodUser ("这是回调的第二阶段");//把这个对象当做“方法”使用
	}


	private void makeShow(string theInformation)
	{
		print ("information - "+theInformation);
	}
}


class illusion
{
	public void makeIllusion(string theInformation)
	{
		Debug.Log ("illusion - "+theInformation);
	}
}