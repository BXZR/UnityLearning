using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStateShowImage : Graphic {

	public GameObject theLabelProfab;
	public  List<float> values ;
	public  List<string> ValueTitles;
	public float  fullDistance = 50f;
	public bool isBackPicture = false;

	//开始绘制的标记
	public bool isDrawing = false;
	//每一个分量分到的角度
	private float angleForEach = 360f;

	private List<Vector3> postions = new List<Vector3> ();
	private List<Vector3> postionsForBack = new List<Vector3> ();
	private List<GameObject> theLabels = new List<GameObject> ();

	void Start()
	{
		//values = new List<float> ();
		//ValueTitles = new List<string> ();

		if(values.Count > 0 ) 
			angleForEach = 360f / values.Count;

		makeClear ();
		makeDrawing (values,ValueTitles);
	}


	void makeClear()
	{
		Text[] TD = this.GetComponentsInChildren<Text> ();
		for (int i = 0; i < TD.Length; i++)
			DestroyImmediate(TD[i].gameObject);
	}

	void makeDrawing(List<float> values , List<string> titles )
	{
		//简单清理工作
		float angleNow = 0;
		for (int i = 0; i < theLabels.Count; i++)
			Destroy (theLabels[i].gameObject);
		postionsForBack.Clear ();
		postions.Clear ();

		Vector3 positionForThis = this.transform.position;
		postions.Add (positionForThis);
		for (int i = 0; i < values.Count; i++) 
		{
			if (!isBackPicture)
			{
				//平面计算，没有Z分量
				Vector3 postionAdd = new Vector3 (Mathf.Cos (angleNow * Mathf.Deg2Rad) * fullDistance * values [i], 
					                    Mathf.Sin (angleNow * Mathf.Deg2Rad) * fullDistance * values [i], 0);
				Vector3 pos = this.transform.position + postionAdd;
				postions.Add (pos);
			}
			else 
			{
				Vector3 postionForLabel = new Vector3 (Mathf.Cos(angleNow * Mathf.Deg2Rad)*fullDistance, Mathf.Sin(angleNow * Mathf.Deg2Rad)*fullDistance,0);
				GameObject theLabel = GameObject.Instantiate (theLabelProfab);
				//theLabel.transform.SetParent (this.transform .root.transform);
				theLabel.transform.SetParent (this.transform);
				theLabel.transform.localPosition = postionForLabel;
				theLabel.transform.localScale = new Vector3 (1, 1, 1);
				theLabel.GetComponent <Text> ().text = titles [i];
				theLabels.Add (theLabel);

				postionsForBack.Add (postionForLabel);
			}
			angleNow += angleForEach;
		}
		isDrawing = true;
	}
		
	// 自己手动刷新
	void Update()
	{
		　　//SetNativeSize();
		　　SetVerticesDirty();
	}
	protected override void OnPopulateMesh(VertexHelper vh)
	{
		if (values.Count > 0 && isDrawing) 
		{
			Color32 color32 = color;
			vh.Clear ();

			if (isBackPicture) 
			{
				//完全背景
				for (int i = 0; i < postionsForBack.Count; i++) 
				{
					vh.AddVert (postionsForBack [i], color32, new Vector2 (0f, 0f));
				}
				for (int i = 1; i < postionsForBack.Count - 1; i++) 
				{
					vh.AddTriangle (0, i, i + 1);
				}
				//vh.AddTriangle (0, postionsForBack.Count - 1, 1);
			} 
			else 
			{
				//前面绘制的内容
				for (int i = 0; i < postions.Count; i++)
				{
					vh.AddVert (postions [i], color32, new Vector2 (0f, 0f));
				}
				for (int i = 1; i < postions.Count - 1; i++) 
				{
					vh.AddTriangle (0, i, i + 1);
				}
				vh.AddTriangle (0, postions.Count - 1, 1);
			}
		}

	}
}
