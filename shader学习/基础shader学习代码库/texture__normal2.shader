// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Suck/texture__normal2_worldSpace"
{
    //在世界坐标之下计算凹凸性质的shader
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("MainTex", 2D) = "white"{}
		_BumpMap ("NormalTex", 2D) = "bump"{}
		_BumpScale("BumpScale", float) = 1.0
		_Specular("Specualr", color) = (1,1,1,1)
		_Gloss ("Gloss", Range(8.0,256)) = 20 
	}

	SubShader
	{ 
	    cull false
		Pass
		{
		   //定义光照模式
		   Tags {"LightMode" = "ForwardBase"}
			CGPROGRAM
			//定义顶点着色器的计算方法
			#pragma vertex vert
			//定义片元着色器的计算方法
			#pragma fragment frag
			//引擎内置的光照信息“头文件”
			#include "Lighting.cginc"

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;//引擎自己控制的纹理偏移量信息记录
			sampler2D _BumpMap;
			float4 _BumpMap_ST;
			float _BumpScale;
			fixed4 _Specular;
			float _Gloss;

			//输入结构体
		    struct a2v
		    {
		     float4 vertex : POSITION;//获取点矩阵信息
		     float3 normal : NORMAL;//获取引擎中的法线方向
		     float4 tangent : TANGENT;//获取引擎中的切线方向
		     float4 texcoord: TEXCOORD0;//获取贴图信息
		    };

			//输出结构体
			struct v2f 
			{
			   float4 pos : SV_POSITION;
			   float4 uv : TEXCOORD0;
			   float4 TtoW0 : TEXCOORD1;
			   float4 TtoW1 : TEXCOORD2;
			   float4 TtoW2 : TEXCOORD3;
			};
			//顶点着色器
			v2f vert (a2v v)
			{
			  v2f o;//返回用的结构体
			  o.pos = UnityObjectToClipPos(v.vertex);//矩阵转换
			  o.uv.xy = v.texcoord.xy *_MainTex_ST.xy + _MainTex_ST.zw;//计算主贴图偏移量
			  o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;//计算法线贴图偏移量

			  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			  fixed3 worldNormal = UnityObjectToWorldNormal (v.normal);//法线方向
			  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);//切线方向
			  fixed3 worldBinormal = cross (worldNormal , worldTangent).xyz;//顶点法线方向

			  o.TtoW0= float4 (worldTangent.x , worldBinormal.x ,worldNormal.x ,worldPos.x);
			  o.TtoW1= float4 (worldTangent.y , worldBinormal.y ,worldNormal.y ,worldPos.y);
			  o.TtoW2= float4 (worldTangent.z , worldBinormal.z ,worldNormal.z ,worldPos.z);

              return o;
			} 

			//这只是计算预留接口，内存循环处理已近给包好，这里暂时不能看到
			fixed4 frag(v2f i) : SV_Target
			{
		        //接收传来的信息
			    float3 worldPos = float3 (i.TtoW0.w,i.TtoW1.w,i.TtoW2.w);//注意获取的是坐标
			    //计算光照角度
			    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
			    //计算观察角度
			    fixed3 viewDir = normalize (UnityWorldSpaceViewDir(worldPos));


			    fixed4 packedNormal = tex2D(_BumpMap ,i.uv.zw);
			    fixed3 tangentNormal;

			    //如果不是法线贴图还需要做一次转换
			    //tangentNormal .xy = (packedNormal .xy *2 - 1 ) * _BumpScale;
			    //tangentNormal.z = sqrt (1.0 - saturate (dot (tangentNormal .xy , tangentNormal .xy)));

			    //如果是法线贴图就走这些内容
			    tangentNormal = UnpackNormal(packedNormal);//获得正确的法线方向的方法
			    tangentNormal.xy *= _BumpScale;
			    tangentNormal.z = sqrt (1.0- saturate (dot (tangentNormal.xy,tangentNormal.xy )));
			    tangentNormal = normalize (half3(dot(i.TtoW0.xyz,tangentNormal),dot(i.TtoW1.xyz,tangentNormal),dot(i.TtoW2.xyz,tangentNormal)));
			    //计算纹理效果，与下面的光照效果结合使用
			    fixed3 albedo =tex2D (_MainTex , i.uv).rgb * _Color.rgb;
			    //基础光照效果
			    fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
			    //计算漫反射
			    fixed3 diffuse = _LightColor0.rgb * albedo *max (0, dot(tangentNormal,lightDir));
			    //Blinn-phong光照模型用来计算高光效果
			    fixed3 halfDir = normalize (lightDir+ viewDir);
			    fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow (max(0,dot(tangentNormal, halfDir)), _Gloss);

			    return fixed4 (ambient + diffuse + specular ,1.0);
			}

			ENDCG
		}
	}

	FallBack "Standard"
}
