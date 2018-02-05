Shader "Suck/demoShader" 	
{
   //UnityShader——shaderLab的最基本语法框架
	Properties
	{
       //_MainTex 变量名
       //Texture 面板显示的内容
       //2D 类型，在这里表示texture
       //"black" {} 默认值 写成 "" {}都行
		_MainTex ("Texture", 2D) = "black" {}

	
	
	}

	//渲染通道1，机器可以根据环境选择第一个匹配环境的渲染通道
	SubShader
	{	//用这种方式直接标定用哪一个渲染通道
		//在这里的名字需要全大写，因为shader编译之后会全变成大写的
		//也正因为如此shader似乎不区分大小写
	     usePass "demoShader/SUCKPASS"

	     //标签的写法(仅仅可以在SubShader写)
		 tags 
		 {
		 "ForceNoShadowCasting" = "False"	//是否投射阴影
		 "DisableBatching" = "True" 		//禁止对这个shader进行批处理
		// "PreviewType" = "Plane"          //面板材质显示方式，例如平面
		 }

		 //状态设置参数，这个在这里SubShader写就是宏，在pass里面写就是私有
		 // cull Front //前面剔除
		    cull Back //后面剔除
		 // cull off  //不剔除
			ZWrite on //开启深度写入（流水线后期环节）
		 // ZWrite off 关闭深度写入（流水线后期环节）

		//定义完整的渲染流程
		Pass
		{
			//这个渲染通道的名字
			Name "SuckPass"
		    tags
		    {
		      "LightMode" = "ForwardBase"
		    }

		}
	}

	//如果上面的subShader都不能用就用下面这个
	FallBack "standard"
	//也可以破罐破摔，不管了
	//FallBAck off
}
