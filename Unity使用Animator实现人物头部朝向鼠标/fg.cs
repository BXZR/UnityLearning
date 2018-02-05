using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fg : MonoBehaviour {

	private Animator _animator;  
	private Vector3 _pos ;  

	// Use this for initialization  
	void Start () {  
		_animator = GetComponent<Animator>();  
	}  
	
	void OnAnimatorIK(int layer)
	{  
		if (layer == 1) {  
			Vector3 pos =  Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x,Input.mousePosition.y,Camera.main.transform.position.z));  
			_pos = Vector3.Lerp (_pos, pos, 0.075f);  

			_animator.SetLookAtPosition (_pos);  
			_animator.SetLookAtWeight (0.65f,0.9f,1f,1f,0.6f);  
		}  
	}  
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			print ("aaa");
			_animator.Play ("idle");
		}
		if(Input .GetKeyDown(KeyCode .L))
		{
			print ("run");
			_animator.Play ("run");
		}
	}
}
