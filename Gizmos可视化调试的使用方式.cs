using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class giz : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//提供一种可视化的调试方式
	//导出之后将会无效
	/*
	Gizmos 类
	Gizmos用于Gizmos 类
	Gizmos用于场景中给出一个可视化的调试或辅助设置。 所有的Gizmos绘制都必须在脚本的OnDrawGizmos或OnDrawGizmosSelected函数中完成。 OnDrawGizmos在每一帧都被调用。所有在OnDrawGizmos内部渲染的Gizmos都是可见的。 OnDrawGizmosSelected尽在脚本所附加的物体被选中时调用。

	类变量

	◆ static var color : Color    //    描述：设置下次绘制的Gizmos的颜色。

	function OnDrawGizmosSelected()

	{

		Gizmos.color = Color.red;   
		var direction = transform.TransformDirection(Vector3.forward) * 5;  //    在物体的前方绘制一个5米长的线
		Gizmos.DrawRay(transform.position, direction);
	}

	◆ static var matrix : Matrix4x4    //    描述：设置用于渲染所有gizmos的矩阵。

	类方法

	◆ Static function DrawCube(center:Vector3,size:Vector3):void    //    描述:用center和size绘制一个立方体.

	Function OnDrawGizmosSelected(){

		Gizmos.color=Color(1,0,0,5);    //    在变换位置处绘制一个变透明的蓝色立方体
		Gizmos.DrawCube(transform.position,Vector3(1,1,1));
	}

	◆ Static function DrawGUITexture(screenRect:Rect,texture:Texture,mat:Material=null):void    //    描述：在屏幕坐标下绘制一个纹理。用于GUI背景。

	◆ Static function DrawGUITexture(screenRect:Rect,texture:Texture,leftBorder:int,rightBorder:int,topBorder:int,bottomBorder:int,mat:Material=null):void    //    描述:在屏幕坐标下绘制一个纹理。用于GUI背景。

	◆ Static function Drawicon(center:Vector3,name:string):void    //    描述:在世界位置center处绘制一个图标.这个图标被命名为name并放置在Assets/Gizmos文件夹或Unity.app/Resoutces文件夹.DrawIcon允许你在游戏中快速选择重要的物体。

	在物体位置处绘制光源灯泡图标.因为我们在OnDrawGizmos函数内部调用它,在场景视图中 ,这个图标总是可点选的.

	function OnDrawGizmos(){
		Gizmos DrawIcon(transform.position,”Light Gizmo.tiff”);
	}

	◆ Static function DrawLine(from:Vector3,to:Vector3):void    //    描述:绘制一条线从from到to.

	Var Larget:Transform;

	function OnDrawGizmosSelected(){
		if(target != null)
		{

			Gizmos.color = Color.blue;   //从transform到target绘制一条蓝色的线
			Gizmos.DrawLine(transform.position,target.position);
		}
	}

	◆ static function DrawRay(r:Ray):void

	static function DrawRay(from:Vector3,direction:Vector3):void    //   描述:绘制一个射线从from开始到from + direction.

	◆ function OnDrawGizmosSelected(){

		Gizmos.color = Color.red;
		Direction = transform.TransformDirection(Vector3.forward)*5;
		Gizmos.DrawRay(transform.positon,direction);
	}

	◆ Static function DrawSphere(center:Vector3,radius:flont):void    //   描述:用center和randins绘制一个球体.

	Function OnDrawGizmosSelected(){

		Gizmos.color = Color.yellow;     //    在变换位置处绘制一个黄色的球体
		Gizmos.DrawSphere(transtorm.position,1);
	}

	◆ Static function DrawWireCube(center:Vector3, size: Vector3):void    //    描述:用center和radius绘制一个线框立方体.

	Function OnDrawGizmosSelected(){

		Gizmos.color = Color.yellow;     //在变换位置处绘制一个黄色立方体
		Gizmos.DrawWireCube (transtorm.position, Vector3(1,1,1));

	}

	◆ Static function DrawWireSphere(center:Vector3,radius:float):void    //    描述:用center和radius绘制一个线框球体.

	Var explosionRadius = 5.0;

	Function OnDrawGizmosSelected(){

		Gizmos.color = Color.white;    //选中的时候显示爆炸路劲
		Gizmos.DrawSphere(transtorm.position,explpsionRadius);
	}

	场景中给出一个可视化的调试或辅助设置。 所有的Gizmos绘制都必须在脚本的OnDrawGizmos或OnDrawGizmosSelected函数中完成。 OnDrawGizmos在每一帧都被调用。所有在OnDrawGizmos内部渲染的Gizmos都是可见的。 OnDrawGizmosSelected尽在脚本所附加的物体被选中时调用。

	类变量

	◆ static var color : Color    //    描述：设置下次绘制的Gizmos的颜色。

	function OnDrawGizmosSelected()

	{

		Gizmos.color = Color.red;   
		var direction = transform.TransformDirection(Vector3.forward) * 5;  //    在物体的前方绘制一个5米长的线
		Gizmos.DrawRay(transform.position, direction);
	}

	◆ static var matrix : Matrix4x4    //    描述：设置用于渲染所有gizmos的矩阵。

	类方法

	◆ Static function DrawCube(center:Vector3,size:Vector3):void    //    描述:用center和size绘制一个立方体.

	Function OnDrawGizmosSelected(){

		Gizmos.color=Color(1,0,0,5);    //    在变换位置处绘制一个变透明的蓝色立方体
		Gizmos.DrawCube(transform.position,Vector3(1,1,1));
	}

	◆ Static function DrawGUITexture(screenRect:Rect,texture:Texture,mat:Material=null):void    //    描述：在屏幕坐标下绘制一个纹理。用于GUI背景。

	◆ Static function DrawGUITexture(screenRect:Rect,texture:Texture,leftBorder:int,rightBorder:int,topBorder:int,bottomBorder:int,mat:Material=null):void    //    描述:在屏幕坐标下绘制一个纹理。用于GUI背景。

	◆ Static function Drawicon(center:Vector3,name:string):void    //    描述:在世界位置center处绘制一个图标.这个图标被命名为name并放置在Assets/Gizmos文件夹或Unity.app/Resoutces文件夹.DrawIcon允许你在游戏中快速选择重要的物体。

	在物体位置处绘制光源灯泡图标.因为我们在OnDrawGizmos函数内部调用它,在场景视图中 ,这个图标总是可点选的.

	function OnDrawGizmos(){
		Gizmos DrawIcon(transform.position,”Light Gizmo.tiff”);
	}

	◆ Static function DrawLine(from:Vector3,to:Vector3):void    //    描述:绘制一条线从from到to.

	Var Larget:Transform;

	function OnDrawGizmosSelected(){
		if(target != null)
		{

			Gizmos.color = Color.blue;   //从transform到target绘制一条蓝色的线
			Gizmos.DrawLine(transform.position,target.position);
		}
	}

	◆ static function DrawRay(r:Ray):void

	static function DrawRay(from:Vector3,direction:Vector3):void    //   描述:绘制一个射线从from开始到from + direction.

	◆ function OnDrawGizmosSelected(){

		Gizmos.color = Color.red;
		Direction = transform.TransformDirection(Vector3.forward)*5;
		Gizmos.DrawRay(transform.positon,direction);
	}

	◆ Static function DrawSphere(center:Vector3,radius:flont):void    //   描述:用center和randins绘制一个球体.

	Function OnDrawGizmosSelected(){

		Gizmos.color = Color.yellow;     //    在变换位置处绘制一个黄色的球体
		Gizmos.DrawSphere(transtorm.position,1);
	}

	◆ Static function DrawWireCube(center:Vector3, size: Vector3):void    //    描述:用center和radius绘制一个线框立方体.

	Function OnDrawGizmosSelected(){

		Gizmos.color = Color.yellow;     //在变换位置处绘制一个黄色立方体
		Gizmos.DrawWireCube (transtorm.position, Vector3(1,1,1));

	}

	◆ Static function DrawWireSphere(center:Vector3,radius:float):void    //    描述:用center和radius绘制一个线框球体.

	Var explosionRadius = 5.0;

	Function OnDrawGizmosSelected(){

		Gizmos.color = Color.white;    //选中的时候显示爆炸路劲
		Gizmos.DrawSphere(transtorm.position,explpsionRadius);
	}
  */

	void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere (this.transform .position ,3);
	}
}
