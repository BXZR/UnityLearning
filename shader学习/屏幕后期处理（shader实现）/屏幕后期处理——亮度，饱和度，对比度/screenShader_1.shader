// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "suckScreen/screenShader_1"
{
	Properties
	{
	    _MainTex("Texture" , 2D ) = "white"{}
	   _Brightness  ("Brightness" , float) = 1
	   _Saturation ("Saturation" , float ) = 1
	   _Contrast ("Contrast" ,  float) = 1
	}
	SubShader
	{
		 
		Pass
		{

		   ZTest Always
		   Cull off
		   Zwrite off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
		 
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			half _Brightness;
			half _Saturation;
			half _Contrast;

			//最简单的获取位置的v2f
			struct v2f
			{
			   float4 pos : SV_POSITION;
			   half2 uv : TEXCOORD0;
			};

			//用的是另一种API参数
			//需要appdata_img 的参数表
			v2f vert (appdata_img v)
			{
			v2f o ;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;
			return o;
			}
          
			fixed4 frag(v2f i) : SV_Target
			{
			    //亮度是一个颜色的加权
			   fixed4 renderTex = tex2D (_MainTex , i.uv);
			   fixed3 finalColor =  renderTex.rgb *_Brightness;

			   //饱和度是一个插值
			   fixed lm = 0.2125 * renderTex.r + 0.7154 *renderTex.g + 0.0721*renderTex.b;//计算像素对应亮度
			   fixed3 lmC = fixed3 (lm , lm , lm);//根据这个亮度算一个饱和度为0的值
			   finalColor = lerp(lmC , finalColor , _Saturation);//插值法处理饱和度

			   //对比度也是一个插值
			   fixed3 avgColor = fixed3 (0.5,0.5,0.5);//创建一个对比度为0的颜色值
			   finalColor = lerp (avgColor , finalColor , _Contrast);//插值处理

			   return fixed4 (finalColor , renderTex.a);

     
			}

			ENDCG
		}
	}

	fallback off 
}
