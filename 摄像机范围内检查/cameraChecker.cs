using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraChecker : MonoBehaviour {
	//摄像机检测可见性的类
	//似乎已经算是很深层次的方法调用了

	public Renderer theTestCubeRenderer;

	//检查渲染器是否在摄像机可视范围内的很底层的方法了
	//传入的参数是用于检查的renderer和针对的camera

	//其中：GeometryUtility.CalculateFrustumPlanes 计算视景平面
	//这个函数取得给定的摄像机的视景并返回它的六个面

	//GeometryUtility.TestPlanesAABB 测试平面
	//如果包围盒在平面数组内部返回true

	//theRender .bounds渲染器的包围盒（只读）。

	//获得这些值之后底层应该有一个纯的物理计算了
	public static bool  IsRendererInCameraArea(Renderer theRender, Camera theCamera)
	{
		Plane[] thePlanes = GeometryUtility.CalculateFrustumPlanes (theCamera);
		return GeometryUtility.TestPlanesAABB (thePlanes,theRender .bounds);
	}

	//看上面的注释，这个也是一样的套路只不过包围盒自己建立了一个
	//似乎物理引擎也是使用的包围盒的思路
	public static bool isPointInCameraArea(Vector3 point,Camera theCamera,out Vector3 showPosition)
	{
		showPosition = Vector3.zero;
		Bounds theBound = new Bounds (point,Vector3.zero);
		Plane[] thePlanes = GeometryUtility.CalculateFrustumPlanes (theCamera);
		bool isVisible = GeometryUtility.TestPlanesAABB (thePlanes,theBound);
		if(isVisible)
		{
			showPosition = theCamera.WorldToViewportPoint(point);
		}
		return isVisible;
	}

	/********************************************************************************************/

	void makeCheck()
	{
		Camera  cam = Camera.main;
		Plane[] thePlanes = GeometryUtility.CalculateFrustumPlanes(cam);
		int i = 0;
		while (i < thePlanes.Length) 
		{
			GameObject p = GameObject.CreatePrimitive(PrimitiveType.Cube);
			p.transform.localScale = new Vector3 (9,9,0.02f);
			p.name = "Plane " + i.ToString();
			p.transform.position = -thePlanes[i].normal * thePlanes[i].distance;
			p.transform.rotation = Quaternion.FromToRotation(Vector3.up, thePlanes[i].normal);
			i++;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input .GetKeyDown(KeyCode .Space))
			print(IsRendererInCameraArea(theTestCubeRenderer, Camera.main));
		if (Input.GetKeyDown (KeyCode.A))
		{
			Vector3 showPosition = Vector3.zero;
			print (isPointInCameraArea(new Vector3 (1,2,3),Camera.main,out showPosition));
			print ("showPosition = "+ showPosition );
		}
		if (Input.GetKeyDown (KeyCode.M))
		{
			print ("摄像机的面片大战");
			makeCheck ();
		}
	}
}
