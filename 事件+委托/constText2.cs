using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//论事件和委托的犀利
//如果是在接口里面定义事件或者不希望事件被人在类外操作，则需要使用事件
//在使用回调函数的时候只能够使用委托
//相对而言，事件要比委托耦合性低，封装的思想更明确

public delegate string sucking(int count);//定义一个叫做sucking的委托，有点像是类型
//随便瞎写的一个测试类
public class suckers{
	public string suckMethod1(int count){
		Debug.Log ("suckMehtod1_Used");
		return "我的口及数量为："+count;
	}
	public string suckMethod2(int count){
		Debug.Log ("suckMehtod2_Used");
		return "我缺少的口及数量为："+count;
}}
public class suckKing{
	public event sucking suckEvent;//这是一个事件，需要至少一个委托进行赋值使用
	public string OnSuck(int count){
		if(suckEvent!=null)//判断一下是不是为空
		return "大吸教主——"+suckEvent (count);//发生这个事件的时候可以调用这个事件里面不止一个的方法
		return "不好意思，我不是教主，他正在吸呢......";
}}
public class constText : MonoBehaviour{
	//此外值得注意的事情是如果是带有返回值的方法，其实返回的是最后添加的方法的返回值
	void makeSuck(){//测试方法
		suckers aSucker = new suckers ();//测试类对象
		sucking theSuck = new sucking (aSucker.suckMethod1);//新建委托对象
			print (theSuck(1));//通过委托运行这个方法
		theSuck += aSucker.suckMethod2;//添加方法
		 	print (theSuck(1));//通过委托运行这个方法
		//前面的是使用委托得到的方法调用
		//中间做一些处理
		theSuck -= aSucker.suckMethod2;//添加方法
		//下面是通过事件得到的东西
		suckKing theKing = new suckKing ();//这是一个有着事件的类对象
		theKing .suckEvent += theSuck;//这个事件决定使用theSuck委托，这样就可以在调用这个事件的时候实际上调用一堆方法
			 print (theKing .OnSuck(1));//通过一个类内的方法进行调用
	}
	void Update (){
		if (Input.GetKeyDown (KeyCode.Space))
			makeSuck ();
}}




