using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyFrameLv2 : MonoBehaviour {
	//可以编辑曲线的摄像机动画
	//其实是泛用类型在这里只是使用摄像机作为一个可视化的例子

	//动画坐标引用
	//可以自己画曲线来设定移动
	public AnimationCurve xCurve;
	public AnimationCurve yCurve;
	public AnimationCurve zCurve;

	private Transform thisTransform;//自己的Transform引用的保留
	public float allTimer = 10f;//总时间
	public float timerNow = 0;//当前使用的时间，这个也是结束的判断
	private float totalMoveDistance = 10;//作为例子的总共移动的长度
	void Start ()
	{
		thisTransform = this.GetComponent <Transform> ();

	}
	//动画播放方法，值得注意的是这个是用时间驱动的
	void  playAnimation()
	{

		if (timerNow < allTimer) 
		{
			float percent = timerNow / allTimer;

			/*重点元素：
			 * Vector3 newPosition = thisTransform.right.normalized * xCurve.Evaluate (percent) *totalMoveDistance;
			 * thisTransform.right.normalized 某一个方向
			 * xCurve.Evaluate (percent) 根据百分比来获取曲线的值
			 * totalMoveDistance 总移动值，这个就是作为例子设定的值
			 */
			Vector3 newPosition = thisTransform.right.normalized * xCurve.Evaluate (percent) *totalMoveDistance;
			newPosition += thisTransform.up.normalized * yCurve.Evaluate (percent) *totalMoveDistance;
			newPosition += thisTransform.forward.normalized * zCurve.Evaluate (percent) *totalMoveDistance;
			//用坐标相加的方法我不是分别计算xyz的值的方法
			thisTransform.position = newPosition;
			timerNow += Time.deltaTime;
		}
	}

	void Update () 
	{
		playAnimation ();
	}
}
