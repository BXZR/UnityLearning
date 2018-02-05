// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/smogShader"
{
	Properties
	{
	    _MainTex ("MainTexture" ,  2D) = "red"{}
	    _fogDensity ("density" , float) = 1.0
	    _fogColor ("color" , color) = (1,1,1,1)
	    _fogStart ("fogStart" , float) = 0.0
	    _fogEnd ("fogEnd" , float) = 0.0

	}
	SubShader
	{
        CGINCLUDE
        #include "UnityCG.cginc"
        float4x4 _FrustumCornersRay;
        sampler2D _MainTex;
        half4 _MainTex_TexelSize;
        sampler2D _CameraDepthTexture;
        half _fogDensity;
        fixed4 _fogColor ;
        float _fogStart;
        float _fogEnd;

        struct v2f
        {
         float4 pos : SV_POSITION;
         half2 uv : TEXCOORD0;
         half2 uv_depth : TEXCOORD1;
         float4 interpolatedRay : TEXCOORD2;
        };

        v2f vert(appdata_img v)
        {
          v2f o;
          o.pos = UnityObjectToClipPos( v.vertex);

          o.uv = v.texcoord;
          o.uv_depth = v.texcoord;

          #if UNITY_UV_SATRTS_AT_TOP

           if(_MainTex_TexelSize.y < 0)
              o.uv_depth.y = 1 -  o.uv_depth.y;
          #endif

          int index = 0;
          if(v.texcoord.x < 0.5 && v.texcoord.y < 0.5)
          {
               index = 0;
          }
          else  if(v.texcoord.x > 0.5 && v.texcoord.y < 0.5)
          {
               index = 1;
          }    
          else  if(v.texcoord.x > 0.5 && v.texcoord.y > 0.5)
          {
               index = 2;
          }     
          else
          {
               index = 3;
          }

         #if UNITY_UV_STARTS_AT_TOP
		  if (_MainTex_TexelSize.y < 0)
			  index = 3 - index;
         #endif

		  o.interpolatedRay = _FrustumCornersRay[index];

		  return o;

          return o;
        }

        fixed4 frag(v2f i) : SV_Target
        {
           float ld = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture , i.uv_depth));

           float3 worldPos = _WorldSpaceCameraPos + ld * i.interpolatedRay.xyz;
           float fogDensity = (_fogEnd - worldPos.y) / (_fogEnd - _fogStart);
           fogDensity = saturate (fogDensity * _fogDensity);

           fixed4 finalColor = tex2D (_MainTex , i.uv);
           finalColor.rgb = lerp (finalColor.rgb , _fogColor.rgb ,fogDensity);

           return finalColor;
        }
        ENDCG

		Pass
		{
		ZTest Always Cull Off Zwrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
	FallBack off
}
