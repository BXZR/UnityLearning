using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RFD : MonoBehaviour {

	ReflectionProbe probe;

	void Start() {
		this.probe = GetComponent<ReflectionProbe>();
	}

	//注意摄像机的坐标范围
	void Update () {
		this.probe.transform.position = new Vector3(
			Camera.main.transform.position.x, 
			Camera.main.transform.position.y * -1, 
			Camera.main.transform.position.z
		);

		probe.RenderProbe();
	}
}
