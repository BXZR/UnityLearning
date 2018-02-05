using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class duiyidui : MonoBehaviour {

	public LayerMask layerMasker;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.J))
			getLayerWithName ("suck");
		if (Input.GetKeyDown (KeyCode.K))
			getLayerWithName ("sucking");
	}

	void getLayerWithName(string  nameInput)
	{
		int layerId = LayerMask.NameToLayer (nameInput);
		layerMasker = 1 << layerId;
	}
}
