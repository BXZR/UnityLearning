using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStateShowImage : Graphic {


	public Transform[] pos;

	// 自己手动刷新
	void Update()
	{
		　　//SetNativeSize();
		　　SetVerticesDirty();
	}
	protected override void OnPopulateMesh(VertexHelper vh)
	{
		Color32 color32 = color;
		vh.Clear();
		　　　　　// 这里我用5对GameObject的坐标来与该Image对象的五个顶点绑定起来
		　　　　　// AddVert的最后一个参数是UV值

//		vh.AddVert(pos[0].position, color32, new Vector2(0f, 0f));
//		vh.AddVert(pos[1].position, color32, new Vector2(0f, 1f));
//		vh.AddVert(pos[2].position, color32, new Vector2(1f, 1f));
//		vh.AddVert(pos[3].position, color32, new Vector2(1f, 0f));
//		vh.AddVert(pos[4].position, color32, new Vector2(0.5f, 0f));
//
//		vh.AddTriangle(0, 1, 2);
//		vh.AddTriangle(2, 3, 4);
//		vh.AddTriangle(2, 4, 0);

		vh.AddVert(pos[0].position, color32, new Vector2(0f, 0f));
		vh.AddVert(pos[1].position, color32, new Vector2(0f, 1f));
		vh.AddVert(pos[2].position, color32, new Vector2(1f, 1f));
		vh.AddVert(pos[3].position, color32, new Vector2(1f, 0f));
		vh.AddVert(pos[4].position, color32, new Vector2(0.5f, 0f));
		vh.AddVert(pos[5].position, color32, new Vector2(0.5f, 0f));

		vh.AddTriangle(0, 1, 2);
		vh.AddTriangle(0, 2, 3);
		vh.AddTriangle(0, 3, 4);
		vh.AddTriangle(0, 4, 5);
	}
}
