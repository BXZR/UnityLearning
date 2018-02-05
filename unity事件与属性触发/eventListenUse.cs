using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void EventsUse (int i= 0);
public enum theEventType {error,log}

public class eventListenUse : MonoBehaviour {
	//事件侦听示例，使用委托
	EventsUse theEvents ;//方法指针保存
	public Dictionary <theEventType ,List <EventsUse>>theListeners = new Dictionary<theEventType, List<EventsUse>>();
	private static  eventListenUse theInstance;//保持单例模式，尤其是在跨场景中使用这个功能的时候

	//超级早期初始化，用于真正创建instance
	void Awake()
	{
		if (!theInstance) 
		{
			theInstance = this;
			DontDestroyOnLoad (this.gameObject);//保证自身是跨行场景的
		} 
		else 
		{
			DestroyImmediate (this.gameObject);//跨场景中可能会有重复的对象，为此也需要考虑一下单例
		}
	}

	//获取引用的方法
	public static eventListenUse  getInstance()
	{
		return  theInstance;
	}

	//事件响应
	public void OnEvents(theEventType  theType,object param = null)
	{
		List <EventsUse> theListensOfType = null;//输出方法out的目标引用
		if (!theListeners.TryGetValue (theType, out theListensOfType))
			return;

		for(int i=0;i<theListensOfType.Count;i++)
			theListensOfType[i]((int)param);

	}

	//增加响应事件
	public void addEvents(theEventType  theType , EventsUse theEventIn)
	{
		List <EventsUse> theListensOfType = null;//输出方法out的目标引用
		if (theListeners.TryGetValue (theType, out theListensOfType)) 
		{
			theListensOfType.Add (theEventIn);
			return;
		}
		theListensOfType = new List<EventsUse> ();
		theListensOfType.Add (theEventIn);
		theListeners.Add (theType , theListensOfType);
	}


	/****************************测试方法************************************************/
	void check()
	{
		operater op1 = new operater ();
		op1.makeStart ();
		operater2 op2 = new operater2 ();
		op2.makeStart ();
		//上面是初始化，没有初始化的注册这个功能不能实现的
		op1.ErrorNumber = 9;
		op1.ErrorNumber = 998;
		op2.LogNumber = 998;
		op2.LogNumber = 9;
	}
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space))
			check ();
	}
}
 
//貌似只有这个类是脚本的主类的时候才会调用那些给的回调方法
 

public class operater 
{

	private int errorNumber = 0;
	public int ErrorNumber
	{
		set
		{
			this.errorNumber = value;
			eventListenUse.getInstance ().OnEvents (theEventType.error,this.errorNumber);
		}
		get
		{
			return this.errorNumber;
		}
	}


	public void  makeStart()
	{
		eventListenUse.getInstance ().addEvents ( theEventType.error,makeUse );
		//eventListenUse.getInstance ().addEvents ( theEventType.log, makeUse );
	}

	private void makeUse(int i=0)
	{
		Debug.Log ("operater——error——"+i);
	}
}


public class operater2 
{
	private int logNumber = 0;
	public int LogNumber
	{
		set
		{
			this.logNumber = value;
			eventListenUse.getInstance ().OnEvents (theEventType.log,this.logNumber);
		}
		get
		{
			return this.logNumber;
		}
	}

	public void  makeStart()
	{
		eventListenUse.getInstance ().addEvents ( theEventType.log,makeUse );
	}
	private void makeUse(int i=0)
	{
		Debug.Log ("operater2——log——"+i);
	}
}
	
