using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floatMeshDemo : MonoBehaviour {

	MeshRenderer MR;
	// Use this for initialization
	void Start () {
		MR = this.GetComponent <MeshRenderer> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		float x = MR.material.mainTextureOffset.x > 3f ? 1f : MR.material.mainTextureOffset.x + Time.deltaTime * 1.2f; 
		float y = MR.material.mainTextureOffset.y > 3f ? 1f : MR.material.mainTextureOffset.y + Time.deltaTime * 0.6f; 
		Vector2 off = new Vector2 (x,y);
		MR.material.mainTextureOffset = off;
	}
}
