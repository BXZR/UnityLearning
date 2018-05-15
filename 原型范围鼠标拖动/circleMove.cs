using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class circleMove : MonoBehaviour {

	public Transform theCenter;
	public float R = 3f;

	void move()
	{
		Vector3 Mouse3D = Input.mousePosition;
		Mouse3D.z = theCenter.transform.position.z;
		Mouse3D = Camera.main.ScreenToWorldPoint (Mouse3D);
		Vector3 detla = Mouse3D - theCenter.transform.position;

		if (detla.magnitude > R) 
		{
			detla = detla.normalized * R;
		}
		this.transform.position = theCenter.position + detla;
	}

	void Update ()
	{
		move ();
	}
}
