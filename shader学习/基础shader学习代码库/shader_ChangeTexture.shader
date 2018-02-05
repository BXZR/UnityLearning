// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Suck/shader_ChangeTexture"
{
	Properties
	{
	    //主要基调颜色
	   _Color ("Color",Color)= (1,1,1,1)
	    //目标贴图
	   _RampTex ("RampTex", 2D) = "white"{}
	    //高光光晕颜色
	   _Specular("Specular", Color) = (1,1,1,1)
	    //高光光晕强度
	   _Gloss ("Gloss", Range(8.0,256)) = 20
	}

	SubShader
	{
		Pass
		{
		  //前向的光照模式
		  Tags {"LightMode" = "ForwardBase"}
		  CGPROGRAM
		  //定义编译器指定的方法
		  //顶点着色器
		  #pragma vertex vert
		  //片元着色器
		  #pragma fragment frag
		  //为了获取光照变量需要引用的“头文件”
		  #include "Lighting.cginc"

		  //与属性对应的变量
		  fixed4 _Color;
		  sampler2D _RampTex;
		  float4 _RampTex_ST;//用于记录偏移量的数组‘
		  fixed4 _Specular;
		  float _Gloss;

		  //输入结构体
		  struct a2v
		  {
		     //从POSITION中获取顶点位置信息
		     float4 vertex : POSITION;
		     //获取世界发现信心
		     float3 normal : NORMAL;
		     //获取第一个贴图的信息
		     float4 texcoord : TEXCOORD0;
		  };

		  //片元着色器的输入结构体
		  struct v2f
		  {
		      //这个初值应该是为了建立映射信息
		      //记录的目标位置
		      float4 pos : SV_POSITION;
		      //记录法线
		      float3 worldNormal : TEXCOORD0;
		      //记录世界坐标
		      float3 worldPos : TEXCOORD1;
		      //记录UV坐标信息
		      float2 uv : TEXCOORD2;
		  };


		  v2f vert (a2v v)
		  {
		     v2f o;
		     //世界坐标转换，这个貌似是必须要的
		     o.pos = UnityObjectToClipPos ( v.vertex);
		     //模型坐标->世界坐标的转化获得发现
		     o.worldNormal = UnityObjectToWorldNormal(v.normal);
		     //模型坐标转世界坐标
		     o.worldPos = mul(unity_ObjectToWorld , v.vertex).xyz;
		     //这个方法用计算经过平铺和偏移之后的纹理坐标
		     o.uv = TRANSFORM_TEX(v.texcoord, _RampTex);
		     return o;
		  }

		 fixed4 frag(v2f i):SV_Target
		 {
		  //世界法线方向
		  fixed3 worldNormal = normalize(i.worldNormal);
		  //世界光照方向
		  fixed3 worldLightDir = normalize (UnityWorldSpaceLightDir (i.worldPos));
		  //基础光照，白的
		  fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
		  //半兰伯特模型做漫反射
		  //这个数值是我自己定的，否则太过亮了
		  fixed halfLambert = 0.8 * dot (worldNormal,worldLightDir)+0.2;
		  //这个方法记录漫反射的光，用的是半兰伯特模型
		  fixed3 diffuseColor = tex2D (_RampTex , fixed2 (halfLambert,halfLambert)).rgb * _Color.rgb;
		  fixed3 diffuse = _LightColor0.rgb *diffuseColor  ;
		   //Blinn-phong光照模型用来计算高光效果
		  fixed3 viewDir = normalize (UnityWorldSpaceViewDir (i.worldPos));
		  fixed3 halfDir = normalize (worldLightDir + viewDir);
		  fixed3 specular = _LightColor0.rgb * _Specular .rgb * pow(max (0, dot(worldNormal,halfDir)), _Gloss);
		  return fixed4 (ambient + diffuse + specular ,1.0);

		 }	 	 	 	 	 
		  ENDCG
		}
	}
}
