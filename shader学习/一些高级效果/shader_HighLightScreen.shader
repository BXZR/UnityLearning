// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Suck/shader_HighLightScreen"
{
	  //逐顶点的高光反射效果shader

	Properties
	{
	   _Diffuse ("Diffuse", color) = (1,1,1,1)//漫反射颜色
	   _Specular("Specular" , color) = (1,1,1,1)//高光反射的颜色
	   _Gloss ("Closs", Range(8.0, 256)) = 20 //高光反射的区域大小
	}

	SubShader
	{
	   //定义pass在Unity光照流水线的角色
	   //只有正确定义lightMode才可以获取到内置光照变量例如_LightColor0
	   tags { "LightMode" = "ForwardBase" }	 
		Pass
		{
		    //CG 代码块
		    //在这个代码块里面的语句就比较正常的，该带分号的需要带分号了
			CGPROGRAM

	        //预编译内容，规定各种地方使用的方法
	        //顶点着色器对应方法vert
			#pragma vertex vert
			//片面着色器对应方法frag
			#pragma fragment frag
			//引用头文件以获取引擎计算参数
			//这个文件是Unity的内置文件
			#include "Lighting.cginc"

			//定义变量与上面属性的类型和名字保持一致，引擎自动接收值
			fixed4  _Diffuse; //漫反射颜色
	        fixed4  _Specular; //高光反射的颜色
	        float   _Gloss; //反射高光的区域大小

	        //顶点着色器的输入结构体
	        struct a2v
	        {
	           //这个是结构体
	           //自带初始化 POSITION 和  NORMAL 引擎支持的值
	           float4 vertex : POSITION;
	           float3 normal : NORMAL;
	        };

	        //顶点着色器的输出结构体，也是片面着色器的输出结构体
	        //要理解这个需要看一下下面的方法调用
	        struct v2f
	        {
	            float4 pos : SV_POSITION;
	            fixed3 worldNormal : TEXCOORD0;
	            //TEXCOORD0 , TEXCOORD1 只可以在方法中使用一次
	            fixed3 worldPos : TEXCOORD1; 
	        };

	        //计算方法
	        //将结果输出到v2f里面，使用参数a2v
	        //也就是说返回类型为v2f
	        v2f vert (a2v v)
	        {
	           v2f o;//用来返回的v2f
	            //  o.pos = UnityObjectToClipPos( v. vertex);
	           o.pos = UnityObjectToClipPos (v.vertex);
	           o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
	           o.worldPos = mul (unity_ObjectToWorld , v.vertex).xyz;
	           return o;       
	        }

	        //片面着色器
	        fixed4 frag(v2f i) : SV_Target
	        {
	           //获取基础光照信息
	           float3 ambient  = UNITY_LIGHTMODEL_AMBIENT.xyz;
	           fixed worldNormal = normalize (i.worldNormal);
	           fixed worldLightDir = normalize (_WorldSpaceLightPos0.xyz);
	           fixed3 diffuse = _LightColor0.rgb * _Diffuse .rgb * saturate( dot (worldNormal,worldLightDir ));
	           fixed3 reflectDir = normalize (reflect (-worldLightDir ,worldNormal));
	           fixed3 viewDir = normalize (_WorldSpaceCameraPos.xyz - i.worldPos.xyz );
	           fixed3 specular = _LightColor0.rgb * _Specular .rgb * pow (saturate (dot (reflectDir, viewDir)), _Gloss );

	           return fixed4(ambient + diffuse + specular, 1.0);
	        }

			ENDCG
		}
	}

	FallBack "Standard"
}
