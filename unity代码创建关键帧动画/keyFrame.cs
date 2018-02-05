using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyFrame : MonoBehaviour {

	//用代码操作，制作动画

	//使用代码做keyFrame动画的做法
	private AnimationCurve X,Z,Y;//关键帧的记录项
	void Start () 
	{
		makeStart ();
	}
	void makeStart()
	{

		int count = 30;
		//创建关键帧

		Keyframe[] xkeys = new Keyframe[count] ;
		Keyframe[] zKeys = new Keyframe[count] ;
		Keyframe[] yKeys = new Keyframe[count] ;

		//初始化设定，在这里写出来是为了展现可以这么写
		float timer = 1f;//延迟时间（也就是第一个关键帧的位置）
		//初始化项目
		xkeys[0] = new Keyframe (timer, this.transform .position .x);
		zKeys[0] = new Keyframe (timer ,this.transform .position .z);
		yKeys[0] = new Keyframe (timer, this.transform .rotation .y);
		//其余的关键帧
		for (int i = 1; i < count; i++) 
		{
			timer ++;//不同时间插入关键帧，要不然直接计算根本就不会有效果
			//作为示例，随机乱动
			xkeys[i] = new Keyframe (timer, this.transform .position .x + 3-Random.Range(1,7));
			zKeys[i] = new Keyframe (timer ,this.transform .position .z+ 3-Random.Range(1,7));

			yKeys[i] = new Keyframe (timer ,this.transform .rotation.y   + 45-Random.Range(0,90));
		}

		//设置到动画的记录里面去
		X = new AnimationCurve (xkeys);
		Z = new AnimationCurve (zKeys);
		Y = new AnimationCurve (yKeys);

		//下面这一句测试说明信吸已经插入进去了
	   //for (int u = 0; u < yKeys.Length; u++)
		//print (yKeys[u].value);
	}
		

	void Update ()
	{
		//时间驱动改变位置
		//说实话自己写一个结构来控制比这样隐式的调用要明白
		//但这个结构确实很给力，包含内容可以很多

		//位移
		this.transform.position = new Vector3 (X.Evaluate(Time.time),this.transform .position .y , Z.Evaluate(Time.time));

		//旋转
		//这里有一其实很诡异的时，因为四元组的限制，直接赋值有可能因为超界导致各种赋值错误
		//所以这里先转成vector然后再转换回去(事实上这是因为我的随机角度都是欧拉角)
		Quaternion Tq = this.transform.rotation;
		Vector3 vq = Tq.eulerAngles;
		vq.y = Y.Evaluate (Time.time);
        this.transform.rotation  = Quaternion.Euler(vq);
		/*
		四元数q转欧拉角v
		Vector3 v = q.eulerAngles;

		欧拉角v转四元数q
		Quaternion q = Quaternion.Euler(v);
       */
	}
}
