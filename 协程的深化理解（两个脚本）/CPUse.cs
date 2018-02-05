using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUse : MonoBehaviour {

    //协程交替使用
	IEnumerator Cup;
	IEnumerator  Cdown;
	IEnumerator Cnow;

	int theValue = 0;

	IEnumerator up()
	{
		while (true) 
		{
			if (Input.GetKeyDown (KeyCode.Q))
				break;
			
			theValue++;
			yield return  new WaitForSeconds (0.5f);
		}
	}

	IEnumerator  down()
	{
		while (true) 
		{
			if (Input.GetKeyDown (KeyCode.Q))
				break;
		
			theValue--;
			yield return  new WaitForSeconds (0.5f);
		}
	}

	IEnumerator play()
	{
		while (true) 
		{
			if (Cnow != null && Cnow.MoveNext ())
				yield return  Cnow.Current;
			else
				yield return null;
		}
	}

	void switcher()
	{
		if (Cnow == Cup)
			Cnow = Cdown;
		else
			Cnow = Cup;
	}



	void Start ()
	{
		Cup = up ();
		Cdown = down ();
		StartCoroutine (play());
	}

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.P))
			print ("theValue = "+ theValue);
		if(Input .GetKeyDown(KeyCode .O))
			switcher();
			
	}


}
