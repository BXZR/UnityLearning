using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// where T : new()	类型参数必须具有无参数的公共构造函数。 当与其他约束一起使用时，new() 约束必须最后指定。
// where T : class	类型参数必须是引用类型；这一点也适用于任何类、接口、委托或数组类型。
public class thePool : MonoBehaviour {

	//对象池的脚本示例

    
	void makeDemo()
	{
		PlayerList<Player>  thePlayerList = new PlayerList<Player> ();
		Player thePlayer1 = new Player ("TK" , 3);
		Player thePlayer2 = new Player ("VD" , 4);
		thePlayerList.ADDHead (thePlayer1);
		thePlayerList.ADDHead (thePlayer2);
		IEnumerator<Player> iterator = thePlayerList.GetEnumerator ();
		print("迭代开始------------------");
		while (true)
		{
			bool result = iterator .MoveNext();
			if (!result)
				break;

			print(string .Format( "获得玩家信息： {0}" , ( iterator.Current) .ToString()) );
		}
		print("迭代结束------------------");
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

public  class Player
{
	private string playerName = "player";
	private int playerLv = 1;

	public Player (string nameIn , int lv)
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
public class PlayerList<T> where T: Player
{
	private class Node
	{
		private Node next;
		private T data;

		public Node(T dataIn)
		{
			data = dataIn;
			next = null;
		}

		public Node Next {
			get { return next; }
			set { next = value; }
		}
		public T Data
		{
			get { return data;}
			set { data = value;}
		}
	}

	private Node thehead;
	public PlayerList()
	{
		thehead = null;
	}

	public void ADDHead(T VT)
	{
		Node newNode = new Node (VT);
		newNode.Next = thehead;
		thehead = newNode;
	}

	public IEnumerator<T> GetEnumerator()
	{
		Node current = thehead;
		T temp = null;
		while (current != null) 
		{
			yield return current.Data;
			current = current.Next;
		}
	}
}
