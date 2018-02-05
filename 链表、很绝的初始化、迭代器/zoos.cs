using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zoos : MonoBehaviour , IEnumerable {

	//这个精彩的初始化就是依靠这两个变量实现的，务必注意以下
	public static zoos theFirstZoos;
	public static zoos theLastZoos;

	public zoos theNext;//链表下一个
	public zoos thePre;//链表上一个

	public  string theName;//随便写的一个属性用来测试
	void Awake()
	{
		//为了表示清晰，我将所有的静态的前面的类型也都写出来了
		if (zoos.theFirstZoos == null)
			zoos.theFirstZoos = this;
		
		if (zoos.theLastZoos != null)
		{
			zoos.theLastZoos.theNext = this;
			this.thePre = zoos.theLastZoos;
		}
		zoos.theLastZoos = this;
		//这段初始化的代码非常精彩，数据结构的构建与脚本的生命周期结合，很了不起
	}


	void OnDestroy()
	{
       //其实就是一个链表的节点删除
		if (this.thePre != null)
			this.thePre.theNext = this.theNext;

		if (this.theNext != null)
			this.theNext.thePre = this.thePre;
	}

	public IEnumerator GetEnumerator()
	{
		return new theEnumberator ();
	}


}


//这个类是迭代器类
//分开是为了结构更清晰一点点
public class theEnumberator : IEnumerator
{
	private  zoos theOBJ;//保留引用
	public bool MoveNext()
	{
		theOBJ = (theOBJ == null) ? zoos.theFirstZoos : theOBJ.theNext;
		return (theOBJ != null);
	}
	public void Reset()
	{
		theOBJ = null;
	}
	public object  Current
	{
		get{return theOBJ;}
	}
}