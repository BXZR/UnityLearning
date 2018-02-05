using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meshSearcher : MonoBehaviour {

	public float startAngle = 0;
	public float endAngle = 90;
	public float radius = 5f ;
	public float angelPer = 15f;

	MeshCollider theMeshCollider;

	public  List<GameObject> SearchGet = new List<GameObject> ();
	// Use this for initialization
	void Start () {
		theMeshCollider = this.GetComponent <MeshCollider> ();
	}
	
	// Update is called once per frame
	void Update () {
		makeMeshColliderSearch(startAngle,endAngle,radius,angelPer);
	}

	public void makeMeshColliderSearch(float startAngle,float endAngle , float radius = 5f , float angelPer = 15f)
	{
 
		if (Mathf.Abs (startAngle - endAngle) > 180f)
		{
			if (startAngle < 180f)
				startAngle += 360f;
			if (endAngle < 180f)
				endAngle += 360f;
		}
		//保证是正方向旋转
		if (startAngle > endAngle)
		{
			float temp = startAngle;
			startAngle = endAngle;
			endAngle = temp;
		}

		int triangleNum = (int)((endAngle - startAngle) / angelPer + 0.5f);//向上取整
		Vector3[] Virtices = new  Vector3 [triangleNum +2];
		int [] indexForVirtices = new int [triangleNum *3];//每个顶点的下标index
	    //制作mesh
		for (int i = 0; i < triangleNum +1; i++) 
		{
			float currendAngle = startAngle + angelPer * i;
			currendAngle = Mathf.Min (currendAngle, endAngle);//限制
			Virtices [i+1] = Quaternion.AngleAxis(currendAngle , Vector3.up) * Vector3.forward * radius;
		}
		for (int i = 0; i < triangleNum; i++) 
		{
			indexForVirtices [i * 3 + 0] = 0;
			indexForVirtices [i * 3 + 1] = i+1;
			indexForVirtices [i * 3 + 2] = i+2;
		}

		Mesh theMesh = new Mesh ();
		theMesh.Clear ();
		theMesh.vertices = Virtices;
		theMesh.triangles = indexForVirtices;
		;
		theMesh.RecalculateBounds ();
		theMesh.RecalculateNormals();
		theMeshCollider.sharedMesh = theMesh;

		theMeshCollider.enabled = false;
		theMeshCollider.enabled = true;

	}

	void OnCollisionEnter(Collision A)
	{
		flashSearchGet ();
		if (SearchGet.Count <= 3 && SearchGet.Contains (A.gameObject) == false) 
		{
			SearchGet.Add (A.gameObject);
			print (A.gameObject.name);
			Destroy (A.gameObject ,5f);
		}
	}

	//利用list特性来做检查（不重复查找）
	void flashSearchGet()
	{
		List<GameObject> toRemove = new List<GameObject> ();
		for (int i = 0; i < SearchGet.Count; i++)
			if (SearchGet [i] == null)
				toRemove.Add (SearchGet[i]);
		for (int i = 0; i < toRemove.Count; i++)
			SearchGet.Remove (toRemove [i]);
	}
}
