using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boundCheck : MonoBehaviour {
 
	//用碰撞盒进行范围检测的方法

	public Transform theAim ;
	Collider theCollider;
	void Start () 
	{
		theCollider = this.GetComponent <CapsuleCollider> ();
	}
	
 
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			if (theAim && theCollider)
			{
				if (theCollider.bounds.Contains (theAim.transform.position))
					print ("find");
				else
					print ("not find");
			}
		}
	}
}
