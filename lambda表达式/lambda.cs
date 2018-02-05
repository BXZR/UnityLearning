using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 


//委托，匿名方法，lambda表达式
/*
 *表达式位于 => 运算符右侧的 lambda 表达式称为“表达式 lambda”。 表达式 lambda 会返回表达式的结果，并采用以下基本形式：

       (input parameters) => expression 
 * 
 * 
当lambda表达式中，有多个语句时，写成如下形式：

		(input parameters) => {statement;}

		delegate void TestDelegate(string s);
		TestDelegate myDel = n => { string s = n + " " + "World"; Console.WriteLine(s); };
		myDel("Hello");

其实就是伪方法体
 */
delegate int demoForDeleget(int i);
public class lambda : MonoBehaviour {

	//lambda表达式
	//似乎是C#比较新的特性
	//大概的思路类似于匿名方法，只不过有更好看的表达方式
	//同时这个思想似乎是LINQ的基础

	//示例1，委托类型
	void lambdaDemo1()
	{
		//相当于直接写了一个方法体
	    demoForDeleget demo1 =  x => x*x ;
		print ("value = "+ demo1(10));
		//结果是输出100，前面x被认为是用于计算的参数，计算结果被隐藏作为返回值
		//如果写成 demoForDeleget demo1 =  x => 12 ; 输出结果就是12
	}
 
	//这个似乎与List的默认委托方法其弧度比较高
	void lambdaDemo2()
	{
		List<int> lists = new List<int> ();
		for (int i = 0; i < 10; i++)
			lists.Add (i);

		List<int> listGet = new List<int> ();
		//用lambda表表达式直接做的判断方法
		listGet = lists.FindAll ( listItem => listItem >5);

		for (int i = 0; i < listGet.Count; i++)
			print (listGet[i]);
		//输出结果6789，说明listItem已经生效了
	}

	//准确地说这个不是lambda表达式的示例，而是与之相近的匿名方法的示例
	void lambdaDemo3()
	{
		List<int> lists = new List<int> ();
		for (int i = 0; i < 10; i++)
			lists.Add (i);

		List<int> listGet = new List<int> ();
		//用匿名方法处理
		listGet = lists.FindAll  (delegate(int listItem2) {    return listItem2>5 ?  true : false;  }   );

		for (int i = 0; i < listGet.Count; i++)
			print (listGet[i]);
		//输出结果6789，说明listItem2已经生效了
	}


	//其实这个是最根本的方法：委托
	void lambdaDemo4()
	{
		List<int> lists = new List<int> ();
		for (int i = 0; i < 10; i++)
			lists.Add (i);

		List<int> listGet = new List<int> ();
		//用匿名方法处理

		//theCheck包含一个check方法的委托做的判断
		Predicate <int>theChecker = new Predicate<int>(check);
		listGet = lists.FindAll  ( theChecker  );

		for (int i = 0; i < listGet.Count; i++)
			print (listGet[i]);
		//输出结果6789
	}

	bool check(int listItem3)
	{
		return listItem3>5 ?  true : false;
	}


	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Q))
			lambdaDemo1 ();
		if (Input.GetKeyDown (KeyCode.W))
			lambdaDemo2 ();
		if (Input.GetKeyDown (KeyCode.E))
			lambdaDemo3 ();
		if (Input.GetKeyDown (KeyCode.R))
			lambdaDemo4 ();
	}
}
