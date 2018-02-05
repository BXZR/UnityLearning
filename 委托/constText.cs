using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate string sucking(int count);//定义一个叫做sucking的委托，有点像是类型
public class suckers//随便瞎写的一个测试类
{
	public string suckMethod1(int count)
	{
		return "我的口及数量为："+count;
	}
	public string suckMethod2(int count)
	{
		return "我缺少的口及数量为："+count;
	}
}
public class constText : MonoBehaviour
{
	//此外值得注意的事情是如果是带有返回值的方法，其实返回的是最后添加的方法的返回值
	void makeSuck()//测试方法
	{
		suckers aSucker = new suckers ();//测试类对象
		sucking theSuck = new sucking (aSucker.suckMethod1);//新建委托对象
		print (theSuck(1));//通过委托运行这个方法
		theSuck += aSucker.suckMethod2;//添加方法
		print (theSuck(1));//通过委托运行这个方法
	}
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space))
			makeSuck ();
	}
}




