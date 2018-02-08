using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthRay : MonoBehaviour {

	//方法1，直接用射线碰撞来做
	//这个是用玩家操纵（主动）
	void rayPointPosition(Vector3 mousePosition)
	{
		Ray theRay = Camera.main.ScreenPointToRay (mousePosition);
		RaycastHit theHit;
		if (Physics.Raycast (theRay, out theHit)) {
			print( "Destination is "+ theHit.point);	
		}
	}

	//方法2，用逆透视变换来做
	//但是其实就是用相似来做的
	//折耳根是物体Plane感知射线（被动）
	void rayPointPositionWithDepth(Vector3 mousePosition)
	{
		Ray theRay = Camera.main.ScreenPointToRay (mousePosition);
		float depth = 0;
		if (this.GetComponent<Plane> ().Raycast (theRay, out depth)) 
		{
			print( "Destination is "+ theRay.origin + theRay.direction * depth);	
		}

	}

	void Update () {
		
	}
}
