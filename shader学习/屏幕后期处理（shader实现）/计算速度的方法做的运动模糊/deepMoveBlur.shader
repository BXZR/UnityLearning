// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/deepMoveBlur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BlurSize ("BlurSize" , Float) = 1.0
	}
	SubShader
	{
		CGINCLUDE

		#include "UnityCG.cginc"
		sampler2D _MainTex ; 
		half4 _MainTex_TexelSize;
		//摄像机的深度纹理（内置参数）
		sampler2D  _CameraDepthTexture;

		float4x4 _CVPIM;
		float4x4 _PVPM;

		half _BlurSize;

		struct v2f
		{
		   float4 pos : SV_POSITION;
		   half2 uv : TEXCOORD0;
		   half2 uv_Depth : TEXCOORD1;
		};

 
	     v2f vert(appdata_img v) 
	     {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			
		     o.uv = v.texcoord;
			o.uv_Depth = v.texcoord;


			#if UNITY_UV_STARTS_AT_TOP
			if (_MainTex_TexelSize.y < 0)
				o.uv_Depth.y = 1 - o.uv_Depth.y;
			#endif
		   return o;
		}
 
		fixed4 frag(v2f i) : SV_Target
		{
		   //取消平台差异性的深度纹理采样
		   float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture , i.uv_Depth);
		   float4 H = float4 (i.uv.x*2 -1 , i.uv.y *2 -1 , d*2-1 ,1);
		   float4 D = mul(_CVPIM ,H);
		   float4 worldPos =  D /D.w;
		   float4 currentPos = H;
		   float4 previousPos = mul (_PVPM , worldPos);
		   previousPos /= previousPos.w;

		   float2 V = (currentPos.xy - previousPos.xy)/2.0f;

		   float2 uv = i.uv;
		   float4 c = tex2D (_MainTex,uv);
		   uv += V * _BlurSize;//其实模糊大小的参数就是移动模糊的偏移量

		   for(int j = 1; j<3 ; j++ , uv+= V*_BlurSize)
		   {
		        float4 currentColor = tex2D(_MainTex , uv);
		        c += currentColor;
 		   }
 		   c /= 3;
 		   return fixed4(c.rgb , 1.0);

		}

		ENDCG


		Pass
		{
		    ZTest Always Cull Off  ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			 
			ENDCG
		}
	}
	FallBack Off
}
