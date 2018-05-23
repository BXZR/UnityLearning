using UnityEngine;
using System.Collections;

public class cursor : MonoBehaviour {
	public Texture2D [] Cursors;

	//0 普通的
	//1 怪物
	//2 介绍
	//3 寻找目标

	void Start () 
	{
		setCursor(0);
	}
	void Update () {
 

	}
	public void setCursor(int index)
	{
		Cursor .SetCursor (Cursors[index],new Vector2 (0,0),CursorMode.Auto);
	}
 
 
}
