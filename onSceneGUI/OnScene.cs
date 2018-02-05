
using UnityEngine;
using UnityEditor;
[CustomEditor (typeof (Transform))]
public class OnScene : Editor
{

	void OnSceneGUI ()
	{

		//，不知为何不生效，难道需要升级为专业版？
		Handles.color = new Color (0,0.8f,0.4f,0.1f);
		Handles.DrawSolidDisc (new Vector3 (0,0,0) ,new Vector3 (0,1,0) ,5);
		Handles.color = new Color (0,0.8f,0.4f,0.1f);
	}
}
