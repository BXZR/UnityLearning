using UnityEngine;
using System.Collections;

public class FPS : MonoBehaviour {

//		Application.targetFrameRate 
//		描述：
//		命令游戏尝试以一个特定的帧率渲染。
//		设置targetFrameRate为-1（默认）使独立版游戏尽可能快的渲染，并且web播放器游戏以50-60帧/秒渲染，取决于平台。
//		注意设置targetFrameRate不会保证帧率，会因为平台的不同而波动，或者因为计算机太慢而不能取得这个帧率。
//		在编辑器中targetFrameRate被忽略。

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
