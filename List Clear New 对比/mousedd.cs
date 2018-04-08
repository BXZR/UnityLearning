using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mousedd : MonoBehaviour {

	//对比List  Clear 和 New  

	List<int> A = new List<int> ();
	void CL()
	{
		for (int j = 0; j < 10000; j++) 
		{
			for (int i = 0; i < 100; i++)
				A.Add (i);
			A.Clear ();
		}
	}
	void NE()
	{
		for (int j = 0; j < 10000; j++) 
		{
			for (int i = 0; i < 100; i++)
				A.Add (i);
			A = new List<int> ();
		}
	}

	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Q))
			CL ();
		if (Input.GetKeyDown (KeyCode.W))
			NE ();
	}
}
