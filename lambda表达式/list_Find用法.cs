using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class SDK : MonoBehaviour {


	List <int> demo ;
	delegate int demoForDeleget(int i);


	void makeListDemo ()
	{
		demo = new List<int> ();
		for (int i = 0; i < 99; i++)
			demo.Add (i);
		//第一种方法： lambda表达式
		int a = demo .Find( x => x==77);
		print ("a = "+a);

		//第二种方法： 委托
		 Predicate <int>theChecker = new Predicate<int>(check);
		 int b = demo .Find(theChecker);
		 print ("b = "+b);

		 //第三种方法： 其实也是委托
		int c = demo .Find( delegate(int item) {
			return item ==77 ?  true : false;
		 });
		 print ("c = "+c);
	}

	//委托的方法
	bool check( int item)
	{
		return item ==77 ?  true : false;
	}

 
	// Update is called once per frame
	void Update () {
		makeListDemo ();
	}
}
