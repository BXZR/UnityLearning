// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Suck/shader9_HighLights_BlinnPhone"
{
  //逐像素的高光反射效果shader
  //一般会用这个，使用了Blinn-Phone模型做额外的计算
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
	          float3 worldNormal : TEXCOORD0;
	          float3 worldPos : TEXCOORD1;
	        };

	        //计算方法
	        //将结果输出到v2f里面，使用参数a2v
	        //也就是说返回类型为v2f
	        v2f vert (a2v v)
	        {
	           v2f o;//用来返回的v2f
	           //获取位置信息  mul顶点变换方法
	           o.pos = UnityObjectToClipPos( v.vertex);
	           //获取世界发现坐标
	           o.worldNormal = mul(v.normal , (float3x3)unity_WorldToObject);
	           //世界坐标的位置
	           o.worldPos = mul(unity_ObjectToWorld , v.vertex).xyz ;
	            return o;       
	        }


	           //片面着色器
	       fixed4 frag(v2f i) : SV_Target
	       {
	             //基础光照信息
	             fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT .xyz;
	             //世界法线计算，其实就是一个计算的位置问题，因为这里是片面着色器的单元，所以是逐像素的
	             fixed3  worldNormal = normalize (i.worldNormal);
	             //同样，逐像素计算世界光照方向
	             fixed3  worldLightDir = normalize (_WorldSpaceLightPos0.xyz);
	             //根据公式计算基本的漫反射模型，saturate方法是限制方法
	             fixed3 diffuse = _LightColor0.rgb * _Diffuse .rgb * saturate (dot (worldNormal , worldLightDir));

	             //计算高光反射的信息
	             //反射方向
	            // fixed3 reflectDir = normalize(reflect (-worldLightDir, worldNormal));
	             //观察方向
	             fixed3 viewDir = normalize (_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
	             //用于计算的方向
	             fixed3 halfDir = normalize(viewDir + worldLightDir );
	             //根据公式计算观察方向上的额外光照效果信息
	             fixed3 specular = _LightColor0 .rgb * _Specular.rgb * pow(max (dot (worldNormal ,  halfDir),0), _Gloss);
	             //返回RGBA
	             return fixed4(ambient + diffuse  + specular  ,1.0);//所以高光是漫反射光照模型之上的额外计算加成效果
	       }

			ENDCG
		}
	}

	FallBack "Standard"
}
