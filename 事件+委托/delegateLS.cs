using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void theDelegateForDemo ();
public class delegateLS : MonoBehaviour {


	void Start () 
	{
		outClass outman = new outClass ();
		outman.make ();
		outman.theDEmo += operate5;
		outman.Ev += operate5;
		outman.show ();
		Debug.Log ("DelegateOut----------------------------------");
		//outman.Ev ();//event,不可以类外调用，个人认为是与委托最大的不同
		Debug.Log ("Event----------------------------------");
		outman.theDEmo ();
	}

	public void operate5()
	{
		Debug.Log ("op5");
	}
}

class outClass
{
	public theDelegateForDemo theDEmo;
	public event theDelegateForDemo  Ev;
	public   void make()
	{
		Ev += operate1;
		Ev += operate2;
		Ev += operate3;
		Ev += outClass.operate4;
	

		theDEmo += operate1;
		theDEmo += operate2;
		theDEmo += operate3;
		theDEmo += outClass.operate4;
		 
	}

	public void show()
	{
		Debug.Log ("Delegate---------------------------");
		theDEmo ();
		Debug.Log ("Event---------------------------");
		Ev (); 
	
	}
	private void operate1()//私有方法可以加在委托和事件里面
	{
		Debug .Log  ("op1");
	}

	private void operate2()//私有方法可以加在委托和事件里面
	{
		Debug .Log  ("op2");
	}
	public void operate3()//公有方法可以加在委托和事件里面
	{
		Debug .Log ("op3");
	}
	public static void operate4()//静态方法可以加在委托和事件里面
	{
		Debug .Log ("op4");
	}
}
