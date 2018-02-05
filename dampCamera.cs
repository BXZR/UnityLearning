using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dampCamera : MonoBehaviour {

	public Transform target;//目标
	private Transform thisTransform;//自身的Transform引用
	public float distanceFromTarget=3f;//与目标的距离
	public float cameraHeight =3f;//摄像机高度
	public float rotateDamp = 4f;//旋转阻尼
	public float moveDamp = 4f;//移动阻尼

	void Start () 
	{
		this.thisTransform = this.GetComponent<Transform> ();
	}

	void updateCamera()
	{
		thisTransform.rotation = Quaternion.Slerp (thisTransform .rotation ,target .rotation ,rotateDamp *Time .deltaTime);

		Vector3 destination = Vector3.Slerp (this.transform .position ,target .position ,moveDamp *Time .deltaTime);

		thisTransform.position = destination - thisTransform.forward *distanceFromTarget;

		thisTransform.position = new Vector3 (thisTransform .position.x , cameraHeight,thisTransform .position .z);
		thisTransform.LookAt (target);

	}


	void FixedUpdate()
	{
		updateCamera ();
	}

	// Update is called once per frame
	void Update () {
		
	}
}
