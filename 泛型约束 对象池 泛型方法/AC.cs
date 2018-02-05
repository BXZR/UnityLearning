
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//超级动态绑定方法。
//个人认为是目前最灵活的
public class AC : MonoBehaviour {

	void makeDemo()
	{
		peopleMV p1 = new peopleMV ();
		Suck <peopleMV> sucks = new Suck<peopleMV> ( 
			(peopleMV w ) => { w.HP -= 10; },
			(peopleMV w ,int d ) => {w.HP -= d;} 
		);

		sucks.suckPeopleMethod (p1);
		print ("people hp = "+ p1.HP);
		sucks.suckPeopleMethod2 (p1,22);
		print ("people hp = "+ p1.HP);
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.F))
			makeDemo ();
	}
}

public class peopleMV 
{
	public int HP = 100;
}


public  class Suck <T> where T : class , new()
{
	public Action <T> suckPeopleMethod = null;
	public Action <T , int > suckPeopleMethod2 = null;
	public Suck(Action <T> sk , Action <T , int> sd)
	{
		suckPeopleMethod = sk;
		suckPeopleMethod2 = sd;
	}
}
