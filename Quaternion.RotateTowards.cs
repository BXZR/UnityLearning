using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RRREES : MonoBehaviour {

	Quaternion target;
	void Start()
	{
		target = Quaternion.Euler (0,180,0);
	}
	void Update () 
	{
		transform.rotation = Quaternion.RotateTowards (transform.rotation , target , 1.0f);
		if(Input .GetKeyDown(KeyCode .O))
			target = Quaternion.Euler (0,0,0);
	}
}
