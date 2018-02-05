using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//泛型约束
public class KT : MonoBehaviour {

    
	void Start () {
		
	}
	
 
	void Update () {
		if (Input.GetKeyDown (KeyCode.O)) 
		{
			methodUse demo = new methodUse();
			List<int> demoList = new List<int> ();
			demo.save (demoList , new  int [] {1,2,3,4,5});
			print ("j");
			foreach (int demoInt in demoList)
				print ("value is "+ demoInt+"\n");
		}
	}
}

class A1 <T> where T : struct
{
	//类里面的T参数必须是值类型
}
class A2<T> where T : class
{
	//类里面的T参数必须为引用类型
}
class A3<T> where T : new()
{
	//这个类必须要有无参构造方法
	public A3()
	{
	}
}


class useT<K1,K2> where K1:T where K2:T2
{
    //多类型的泛型约束的写法
	public void UseTMethod(K1 demo)
	{
		demo.TMethod ();
	}
	public void useT2Method(K2 demo2)
	{
		demo2.T2Method ();
	}
}
class useT <K> where K:  T2   
{
 
	public void useT2Method(K demo2)
	{
		demo2.T2Method ();
	}

	//用于方法的泛型约束
	void make<U>(List<U> items) where U : T2
	{
		foreach (U d in items) 
		{
			d.T2Method ();
		}
	}

}

class methodUse
{
	public void save<T>(List<T> d , params T[] values)
	{
		///直接根据values的类型来推断出List<T>的T类型进行检查
		foreach (T a in values)
			d.Add (a);
	}
}

class T
{
	public void TMethod()
	{
		 
	}
}

class T2
{
	public void T2Method()
	{
 
	}
}