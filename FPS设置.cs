using UnityEngine;
using System.Collections;

public class FPS : MonoBehaviour {

	public static int FPSCount=60;
	public GameObject FPSButtonController;
	public int thisFPSCount;

	void Start () {
		Application.targetFrameRate = FPSCount;//更新游戏FPS
	}

	public void setFPS()
	{
		FPSCount =thisFPSCount;//更新自定义FPS变量
		Application.targetFrameRate = FPSCount;//更新游戏FPS
 
		FPSButtonController.GetComponent <FPSButtoncontroller> ().setBack ();
	}
	 

}
