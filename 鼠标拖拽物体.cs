using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (OnMouseDown());
	}
	
	//下面的函数是当鼠标触碰到碰撞体或者刚体时调用，我的碰撞体设置是mesh collider，然后别忘了，给这个collider添加物理材质  

	//值得注意的是世界坐标系转化为屏幕坐标系，Z轴是不变的  
	IEnumerator OnMouseDown()  
	{  
		//将物体由世界坐标系转化为屏幕坐标系 ，由vector3 结构体变量ScreenSpace存储，以用来明确屏幕坐标系Z轴的位置  
		//所以，这个其实是一耳光二维的变化，因为Z似乎是没有变化的
		Vector3 ScreenSpace = Camera.main.WorldToScreenPoint(transform.position);  
		//完成了两个步骤，1由于鼠标的坐标系是2维的，需要转化成3维的世界坐标系，2只有三维的情况下才能来计算鼠标位置与物体的距离，offset即是距离  
		//offset就是用鼠标二维坐标XY和自身坐标转二维坐标Z行程的三维坐标再转回三维坐标的误差
		//这个无法是固定的值所以在初始化的时候做一次就好 
		Vector3 offset = transform.position-Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,ScreenSpace.z));  
		//当鼠标左键按下时  
		while(Input.GetMouseButton(0)) //这个写法我很喜欢 
		{  
			//得到现在鼠标的2维坐标系位置，Z轴是钉死的了来自于初始的位置
			Vector3 curScreenSpace =  new Vector3(Input.mousePosition.x,Input.mousePosition.y,ScreenSpace.z);     
			//将当前鼠标的2维位置转化成三维的位置，再加上鼠标的移动量（这个修正个人认为其实是引擎本身的计算问题......）  
			Vector3 CurPosition = Camera.main.ScreenToWorldPoint(curScreenSpace)+offset;          
			//CurPosition就是物体应该的移动向量赋给transform的position属性        
			transform.position = CurPosition;  
			//这个很主要  
			yield return new WaitForFixedUpdate();  
		}  
	}  
}
