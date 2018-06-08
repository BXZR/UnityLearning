using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class RRE : MonoBehaviour {

	public bool  force = true;
	public float speed = 20f;
	Rigidbody R;
	void Start () {
		R = this.GetComponent <Rigidbody> ();
		R.useGravity = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!force)
			this.transform.Rotate (Vector3.up * speed*Time .deltaTime  , Space.Self);
		if(force)
			R.AddTorque (Vector3.up * speed *Time .deltaTime);
	}
	 
}
    
