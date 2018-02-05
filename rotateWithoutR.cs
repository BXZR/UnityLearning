using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateWithoutR : MonoBehaviour {

	public Transform Center;
	public float angle = 30f;
	void extraRotate()
	{
		Vector3 Minus = this.transform.position - Center.position;//偏移量
		Minus= Quaternion.AngleAxis(angle*Time.deltaTime , Vector3.up) * Minus;//偏移向量乘以旋转向量，于是偏移向量跟着旋转了
		this.transform .position = Center.position + Minus;//加回去偏移量
	}

	void Update () {
		extraRotate ();
	}
}
