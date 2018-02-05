using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor (typeof(SUCKS))]
public class oninspectorEditor :Editor {

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();
		if (GUILayout.Button ("SUCK Edit")) 
		{
			SUCKS suck = target as SUCKS;
			suck.makeSuck ();
		}
	}
}
