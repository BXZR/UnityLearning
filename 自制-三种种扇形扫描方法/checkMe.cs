using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkMe : MonoBehaviour {

	//这是一段没有考虑性能的示例代码

	public Transform searchAim;
	public float seeAreaAngel = 45f;
 
	void Update () 
	{
		showArea (this.seeAreaAngel);
		if (Input.GetKeyDown (KeyCode.Space))
			checkMethod (this.seeAreaAngel,this.searchAim);
	}


	//搜查范围显示随着人物旋转而旋转的关键当然在于任务的欧拉角也作为旋转角计算的参数
	void showArea(float angel,float distance = 100)
	{
 
		float angleInPart =  (angel  + transform.eulerAngles.y);
		Vector3 areaPosition1 = this.transform .position + distance* new  Vector3 (Mathf .Sin(angleInPart *Mathf.PI/180),0,Mathf.Cos(angleInPart *Mathf.PI/180));
		Debug.DrawLine (transform.position,areaPosition1,Color .magenta);

		angleInPart =  (-angel  + transform.eulerAngles.y);
		Vector3 areaPosition2 = this.transform .position + distance* new  Vector3 (Mathf .Sin(angleInPart *Mathf.PI/180),0,Mathf.Cos(angleInPart *Mathf.PI/180));
		Debug.DrawLine (transform.position,areaPosition2,Color .magenta);

	}

	//作为示例的角度检测，无视距离
	//这个方法已经和非常简化了
	//虽然有关数学的封装难以操控（毕竟不是自己封装的），但是其计算与效果都很好
	void checkMethod(float angel,Transform aimTranstrans)
	{
		float Now = Mathf.Abs( Vector3.Angle (this.transform .forward , (aimTranstrans.position - this.transform .position).normalized ));
		if (Now <= angel)
			print ("find");
		else
			print ("not find");
	}
}
