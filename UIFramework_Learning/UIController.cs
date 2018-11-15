using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

	private static UIController theUIcontroller;

	public static UIController GetInstance()
	{
		return theUIcontroller;
	}

	private Dictionary <string , GameObject> UIBook = new Dictionary<string, GameObject>();

	public  void ShowUI <T> () where T : UIBasic
	{
		string UIName = typeof(T).ToString ();
		GameObject theUI;
		if (!UIBook.TryGetValue (UIName, out theUI))
		{
			theUI = (GameObject)Resources.Load ("UI/" + UIName);
			theUI = Instantiate (theUI);
			theUI.name = UIName;
			UIBook.Add (UIName, theUI);
		}
		if(theUI)
		{
			theUI.SetActive (true);
		}
	}

	public  void CloseUI<T> () where T: UIBasic
	{
		
		string UIName = typeof(T).ToString ();
		GameObject theUI;
		if (!UIBook.TryGetValue (UIName, out theUI))
		{
			theUI = (GameObject)Resources.Load ("UI/" + UIName);
			theUI = Instantiate (theUI);
			theUI.name = UIName;
			UIBook.Add (UIName, theUI);
		}
		if(theUI)
		{
			theUI.SetActive (false);
		}
	}


	void Start()
	{
		theUIcontroller = this;
	}
	void Update()
	{
		if (Input.GetKeyDown (KeyCode.A))
			ShowUI<PlayerActCanvas> ();
		if (Input.GetKeyDown (KeyCode.S))
			CloseUI<PlayerActCanvas> ();
	}
}
