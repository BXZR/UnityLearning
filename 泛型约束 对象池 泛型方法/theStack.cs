using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//栈的尝试
public class theStack : MonoBehaviour {

	void  makeDemo()
	{
		Player2 thePlayer1 = new Player2 ("赤红" ,3);
		Player2 thePlayer2 = new Player2 ("天之舞", 99);
		PlayerStack <Player2> theStack = new PlayerStack<Player2> ();
		theStack.Store (thePlayer1);
		theStack.Store (thePlayer2);
		print ( "玩家“天之舞” 等级："+ theStack .getPlayerLV("天之舞") );
		print ( "玩家“罗纳尔多” 等级："+ theStack .getPlayerLV("罗纳尔多") );
	}
	void Update () {
		if (Input.GetKeyDown (KeyCode.F))
			makeDemo ();
	}
}


/*	where T: struct
类型参数必须是值类型。可以指定除 Nullable 以外的任何值类型。有关更多信息，请参见使用可以为 null 的类型（C# 编程指南）。
	where T : class
类型参数必须是引用类型；这一点也适用于任何类、接口、委托或数组类型。
	where T：new()
类型参数必须具有无参数的公共构造函数。当与其他约束一起使用时，new() 约束必须最后指定。
	where T：<基类名>
类型参数必须是指定的基类或派生自指定的基类。
	where T：<接口名称>
类型参数必须是指定的接口或实现指定的接口。可以指定多个接口约束。约束接口也可以是泛型的。
	where T：U
为 T 提供的类型参数必须是为 U 提供的参数或派生自为 U 提供的参数。
*/

public  class Player2
{
	private string playerName = "player";
	private int playerLv = 1;
	//必须要有无参数的构造方法
	public Player2 ()
	{
		playerName = "下弦月";
		playerLv = 1;
	}
	public Player2 (string nameIn , int lv)
	{
		playerName = nameIn;
		playerLv = lv;
	}
	public string PlayerName
	{
		get { return  playerName;}
		set { playerName = value;}
	}
	public  int  PlayerLV
	{
		get { return playerLv;}
		set { playerLv = value;}
	}
	public string ToString()
	{
		return "玩家姓名：" + playerName + " 等级：" + playerLv;
	}
}

//限制内容必须使Player类的对象
public class PlayerStack <T> where T: Player2 ,new ()
{
	private Stack<T> playerStack = new Stack<T> ();

	public T New()
	{
		return playerStack.Count == 0 ? new T () : playerStack.Pop ();
	}

	public void Store(T t)
	{
		playerStack.Push (t);
	}

	public string getPlayerLV(string playerNameIn)
	{
		foreach (Player2 p in playerStack) 
		{
			if (p.PlayerName.Equals (playerNameIn))
				return p.PlayerLV.ToString ();
		}
		return "查无此人";
	}
}