// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CARTOOONSUCK/BIG"
{
	Properties
	{
		_Distortion ("Distortion" , Range(0,1000)) = 10//扭曲程度
		_Larger ("Larger" , Range(0.1,1.9)) = 1.0//放大镜效果
	}
	SubShader
	{
        Tags{"Queue" = "Transparent" "RenderType" = "Opaque"}
        GrabPass{"_RefractionTex"}//定义抓取屏幕图像的pass
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

	        float _Distortion;

	        sampler2D _RefractionTex;//用来存储屏幕图像的缓存
	        float4 _RefractionTex_TexelSize;

	        fixed _Larger;


			struct a2v 
			{
				float4 vertex : POSITION;
			};
			struct v2f
			{
			  float4 pos : SV_POSITION;
			  float4 scrPos : TEXCOORD0;
			};

       	   	v2f vert(a2v v)
       	   	{
       	   	   v2f o;
       	   	   o.pos = UnityObjectToClipPos( v.vertex);
       	   	   o.scrPos = ComputeGrabScreenPos(o.pos);//抓取屏幕图像采样坐标
       	   	   return o;
       	   	}

       	   	fixed4 frag (v2f i) : SV_Target
       	   	{
       	   	 float2 offset =   _Distortion *_RefractionTex_TexelSize.xy ;
       	   	 i.scrPos.xy = offset * i.scrPos.z + i.scrPos.xy  ;
       	   	 //因为坐标，是从左下角开始的，所以需要适当转成中心为起点
       	   	 fixed3 refrCol = tex2D(_RefractionTex , (i.scrPos.xy/ i.scrPos.w - float2(0.5,0.5))*_Larger +float2(0.5,0.5)  ).rgb;//折射颜色
       	   	 return fixed4(refrCol ,1);
       	   	}

			ENDCG
		}
	}
}
