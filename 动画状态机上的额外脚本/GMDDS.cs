using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GMDDS :StateMachineBehaviour {

	public override void OnStateEnter (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter (animator, stateInfo, layerIndex);
		Debug.Log ("GFF");
	}
}
