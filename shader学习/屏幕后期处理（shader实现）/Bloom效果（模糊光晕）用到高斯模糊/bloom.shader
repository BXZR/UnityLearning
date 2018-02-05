// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/bloom"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Bloom ("TextureUseForEnd" , 2D ) = "white" {}
		_lum ("Lum" , float) = 0.5
		_BlurSize ("BlurSize" , float) = 1.0
	}
	SubShader
	{
	 	//内部定义一个所有pass通用的方法集合，所有的pass直接调用就可以
	    CGINCLUDE
		#include "UnityCG.cginc"
		sampler2D _MainTex ;//贴图
		half4 _MainTex_TexelSize;//大小的倒数
		sampler2D _Bloom;//Bloom效果的中间图片缓存
		float _lum;//判断某一个像素亮的阀值
		float _BlurSize;//模糊大小

		struct v2f
		{
		    float4 pos : SV_POSITION;
		    half2 uv : TEXCOORD0;

		};

		v2f vert(appdata_img v)
		{
		  v2f o;
		  o.pos = UnityObjectToClipPos ( v.vertex);
		  o.uv = v.texcoord;
		  return o;
		}

		fixed lumMade(fixed4 theColor)
		{
		  return  0.2125 * theColor.r + 0.7154 * theColor.g + 0.0721 *theColor.b;
		}

	    fixed4 frag (v2f i) : SV_Target
	    {
	      fixed4 c =  tex2D (_MainTex , i.uv);
	      fixed val = clamp (lumMade(c) - _lum , 0.0,1.0);
	      return c * val;
	    }

	    //混合亮度图像和原始图像的pass
	    struct v2fBloom
	    {
	       float4 pos : SV_POSITION;
	       half4 uv : TEXCOORD0;
	    };

	    v2fBloom  vertBloom (appdata_img v) 
	    {
	      v2fBloom o ;
	      o.pos = UnityObjectToClipPos( v.vertex);
	      o.uv .xy = v.texcoord;
	      o.uv.zw = v.texcoord;

	      #if UNITY_UV_STARTS_AT_TOP
	      if(_MainTex_TexelSize.y < 0.0)
	      o.uv.w = 1.0 - o.uv.w;

	      #endif

	      return o;
	    }

	    fixed4 fragBloom (v2fBloom i) : SV_Target
	    {
	      return tex2D (_MainTex , i.uv.xy )+ tex2D(_Bloom , i.uv.zw);
	    }

		ENDCG
		ZTest Always Cull Off ZWrite Off

		Pass
		{
		    //提取亮度贴图
		 	CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}

		//高斯模糊提取到的亮度图
		UsePass "suckScreen/gaosiLvBo/HCOV"
		UsePass "suckScreen/gaosiLvBo/VCOV"

		Pass
		{
		    //提取亮度贴图
		 	CGPROGRAM
			#pragma vertex vertBloom
			#pragma fragment fragBloom
			ENDCG
		}

	}
	FallBack Off
}
