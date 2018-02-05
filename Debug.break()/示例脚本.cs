using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TDXXS : MonoBehaviour {

	public Text theText;
	float value = 0f;
	void Start()
	{
		theText = this.GetComponent <Text> ();
	}
	void Update () 
	{
		value += 0.02f;
		theText.text = value.ToString ("f0");
		if (Input.GetKeyDown (KeyCode.Space))
			Debug.Break ();
		
	}
}
