using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//2D鼠标左边转3D世界坐标
public class mouse2Dt3D : MonoBehaviour {

	void Update () 
	{
		Vector3 mouse2D = Input.mousePosition;
		mouse2D.z = -Camera.main.transform.position.z;
		Vector3 mouse3D = Camera.main.ScreenToWorldPoint (mouse2D);
		//已经转成世界坐标，那就想怎么玩都可以
		//这个脚本的关键在于Z的计算使用的是Camera.main的Z对称，还是有点意思

	}
}
