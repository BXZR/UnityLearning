using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class logmake : MonoBehaviour {

	private StreamWriter theLogger;
	void Start () 
	{
		theLogger = new StreamWriter ("log.txt");
		Application.logMessageReceived +=logInformation;
		 
	}
	
	void logInformation(string information, string stackTrace, LogType theType)
	{
		theLogger.WriteLine("**************************************");
		theLogger.WriteLine(string.Format("type: {0}",theType));
		theLogger.WriteLine(string.Format("Trace: {0}",stackTrace));
		theLogger.WriteLine(string.Format("information: {0}",information));
	}


	void OnDestroy()
	{
		Application.logMessageReceived -= logInformation;
		theLogger.Close ();
		theLogger.Dispose ();
	}

	void check()
	{
		//会出异常的方法，测试log是否生效
		int a = 9;
		int b = 0;
		int c = a / b;
	}
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			theLogger.WriteLine ("也可以人工控制写");
		}
		if (Input.GetKeyDown (KeyCode.L)) 
		{
			check ();
		}
	}
}
