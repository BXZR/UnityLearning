using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniMasks : MonoBehaviour {

	Animation thePlayController ;
	public Transform theControl;
	void Start () 
	{
		thePlayController = this.GetComponent <Animation> ();
		thePlayController ["Attack"].layer = 1;
		thePlayController ["Attack"].AddMixingTransform (theControl);
		thePlayController.Play ("Attack");
	}
	

}
