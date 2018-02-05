using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setset : MonoBehaviour {
 
	public Material mat;  
	private Vector3 pos1;  
	private Vector3 pos2;  
	private Color theColor;
	void Start () 
	{
		theColor = Color.magenta;
	}

	void Update()  
	{  
		if (Input.GetMouseButtonDown(0))  
		{  
			pos1 = Input.mousePosition;  
		}  
		if (Input.GetMouseButton(0))  
		{  
			pos2 = Input.mousePosition;  
		}  
	}  

	//鼠标拖动画线框
	void  GLStep2()
   {
		GL.PushMatrix();  
		mat.SetPass(0);  
		GL.LoadOrtho();  
		GL.Begin(GL.LINES);  
		GL.Color(theColor);  
		GL.Vertex3(pos1.x / Screen.width, pos1.y / Screen.height, 0);  
		GL.Vertex3(pos2.x / Screen.width, pos1.y / Screen.height, 0);  
		GL.Vertex3(pos2.x / Screen.width, pos1.y / Screen.height, 0);  
		GL.Vertex3(pos2.x / Screen.width, pos2.y / Screen.height, 0);  
		GL.Vertex3(pos2.x / Screen.width, pos2.y / Screen.height, 0);  
		GL.Vertex3(pos1.x / Screen.width, pos2.y / Screen.height, 0);  
		GL.Vertex3(pos1.x / Screen.width, pos2.y / Screen.height, 0);  
		GL.Vertex3(pos1.x / Screen.width, pos1.y / Screen.height, 0);  
		GL.End();  
		GL.PopMatrix();  
   }

	//一条线，对角线
	void  GLStep1()
	{
		GL.PushMatrix(); //保存当前Matirx
		mat.SetPass(0); //刷新当前材质
		GL.LoadPixelMatrix();//设置pixelMatrix

		GL.Begin(GL.LINES);
		GL.Color(Color.yellow);
		GL.Vertex3(0, 0, 0);
		GL.Vertex3(Screen.width, Screen.height, 0);
		GL.End();
		GL.PopMatrix();//读取之前的Matrix
	}
	void OnPostRender()  
	{  
		GLStep1 ();
		GLStep2 ();
	}  
}
