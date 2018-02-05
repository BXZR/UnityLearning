using UnityEngine;
using System.Collections;

public class fakeFather : MonoBehaviour
{

	public Transform theFather;
	public Transform theChild;

	private Vector3 fatherChildOffset; 
	Quaternion aoffset ;
	//---------------------------------------------------------------------------------------------------------------------//
	void fatheMove()
	{
		theFather.transform.Translate (new Vector3(0,0,1) *Time.deltaTime);
	}

	void fatherRotate()
	{
		theFather.transform.Rotate (new Vector3 (0, 1, 0) * 50 * Time.deltaTime);
	}

	void childMove()
	{
		fatherChildOffset += new Vector3 (0, 1, 0) * Time.deltaTime;
		//print ("father = "+ theFather.transform .position);
		//print ("child = "+ theChild.transform .position);
		//print ("fcO = " + fatherChildOffset);
	}

	void childRotate()
	{
		theChild .transform.Rotate ( new Vector3 (0, 1, 0) * 50 * Time.deltaTime , Space.World);
		aoffset = Quaternion.Inverse (theFather.transform.rotation) * theChild.transform.rotation;
	}
 
	//---------------------------------------------------------------------------------------------------------------------//
	//修正子物体的位置
	void fixedChild()
	{
		theChild.transform .position = theFather.transform.position +  theFather.transform.rotation * fatherChildOffset ;
		theChild.rotation = theFather.transform.rotation * aoffset ;
	}

	void Start ()
	{
		aoffset = Quaternion.Inverse (theFather.transform.rotation) * theChild.transform.rotation;
		fatherChildOffset =   theChild.transform.position - theFather.transform.position  ;
	}

	public bool use = false;
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKey (KeyCode.LeftArrow))
			fatheMove ();
		if (Input.GetKey (KeyCode.RightArrow))
			childMove ();
		if (Input.GetKey (KeyCode.UpArrow))
			fatherRotate();
		if (Input.GetKey (KeyCode.DownArrow))
			childRotate ();

 
		if(use)
		fixedChild ();
	}
}

