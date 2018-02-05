// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Suck/texture__normal1"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("MainTex", 2D) = "white"{}
		_BumpMap ("NormalTex", 2D) = "bump"{}
		_BumpScale("BumpScale", float) = 1.0
		_Specular("Specualr", color) = (1,1,1,1)
		_Gloss ("Gloss", Range(8.0,256)) = 20 
		_AP ("AP" , Range(-0.3,0.3)) = 0
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
			half _AP;

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
			   float3 lightDir : TEXCOORD1;
			   float3 viewDir : TEXCOORD2;
			};
			//顶点着色器
			v2f vert (a2v v)
			{
			  v2f o;//返回用的结构体
			  o.pos = UnityObjectToClipPos(v.vertex + v.normal *_AP);//矩阵转换
			  o.uv.xy = v.texcoord.xy *_MainTex_ST.xy + _MainTex_ST.zw;//计算主贴图偏移量
			  o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;//计算法线贴图偏移量
			  //float3 binormal = cross (normalize (v.normal), normalize (v.tangent.xyz)) * v.tangent.w;
			 // float3x3 rotation = float3x3 (v.tangent.xyz , binormal , v.normal); 
			 TANGENT_SPACE_ROTATION;//获取模型空间到切线控件的变换矩阵
			  o.lightDir = mul(rotation,ObjSpaceLightDir(v.vertex)).xyz;//获取光照方向
              o.viewDir = mul(rotation , ObjSpaceViewDir(v.vertex)).xyz;//获取观察方向
              return o;
			} 

			//这只是计算预留接口，内存循环处理已近给包好，这里暂时不能看到
			fixed4 frag(v2f i) : SV_Target
			{

			  
			    //因为是逐顶点计算，这里只需要标准化一下接收的值就可以了
			    //名字很诡异，其实就是光照方向
			    fixed3 tangentLightDir = normalize (i.lightDir);
			    //名字很诡异，其实就是观察方向
			    fixed3 tangentViewDir = normalize (i.viewDir);

			    fixed4 packedNormal = tex2D(_BumpMap ,i.uv.zw);
			    fixed3 tangentNormal;

			    //如果不是法线贴图还需要做一次转换
			    tangentNormal .xy = (packedNormal .xy *2 - 1 ) * _BumpScale;
			    tangentNormal.z = sqrt (1.0 - saturate (dot (tangentNormal .xy , tangentNormal .xy)));

			    //如果是法线贴图就走这些内容
			    //tangentNormal = UnpackNormal(packedNormal);//获得正确的法线方向的方法
			    //tangentNormal.xy *= _BumpScale;
			   // tangentNormal.z = sqrt (1.0- saturate (dot (tangentNormal.xy,tangentNormal.xy )));

			    //计算纹理效果，与下面的光照效果结合使用
			    fixed3 albedo =tex2D (_MainTex , i.uv).rgb * _Color.rgb;
			    //基础光照效果
			    fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
			    //计算漫反射
			    fixed3 diffuse = _LightColor0.rgb * albedo *max (0, dot(tangentNormal, tangentLightDir));
			    //Blinn-phong光照模型用来计算高光效果
			    fixed3 halfDir = normalize (tangentLightDir+ tangentViewDir);
			    fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow (max(0,dot(tangentNormal, halfDir)), _Gloss);

			    return fixed4 (ambient + diffuse + specular ,1.0);
			}

			ENDCG
		}
	}

	FallBack "Standard"
}
