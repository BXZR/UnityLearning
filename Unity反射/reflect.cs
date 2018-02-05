using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;//反射名字空间
using System;


public class reflect : MonoBehaviour {


	//反射出所有的类名
	void getTypeNames()
	{
		foreach (Type t in Assembly .GetExecutingAssembly().GetTypes())
			print (t.Name);
	}

	//反射出所有的类名加上公有属性名
	void getInformation()
	{
		foreach (Type t in Assembly .GetExecutingAssembly().GetTypes())
			foreach (FieldInfo FI in t.GetFields(BindingFlags.Public  | BindingFlags.Instance ))
				print (t.Name +"__"+FI .Name);
	}

	//反射出反射机制的根源类名
	void getInformationE()
	{
		foreach (Type t in Assembly .GetExecutingAssembly().GetTypes())
			foreach (Attribute attr in t .GetCustomAttributes(true))
				print (attr .GetType());
				
	}
		
	void Start ()
	{
		getTypeNames ();
		print ("--------------------------------");
		getInformation ();
		print ("--------------------------------");
		getInformationE ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
