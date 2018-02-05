// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/moveBlurs"
{
	Properties
	{
		_MainTex ("MainTexture" , 2D) = "white"{}
		_BlurAmounr("BlurAmount" , Float) = 1.0
	}
	SubShader
	{
        CGINCLUDE

        #include "UnityCG.cginc"
        sampler2D _MainTex;
        fixed _BlurAmount;

        struct  v2f
        {
           float4 pos : SV_POSITION;
           half2 uv : TEXCOORD0;
        };

         v2f vert (appdata_img v)
         {
            v2f o;
            o.pos = UnityObjectToClipPos( v.vertex);
            o.uv = v.texcoord;

            return o;
         }

         fixed4 fragRGB (v2f i) : SV_Target
         {
            return fixed4(tex2D(_MainTex , i.uv).rgb , _BlurAmount);
         }

         fixed4 fragA(v2f i) : SV_Target
         {
            return tex2D(_MainTex ,i.uv);
         }
        ENDCG

        ZTest Always Cull off ZWrite Off

		Pass
		{
		    ColorMask RGB
		    Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragRGB
			ENDCG
		}

		Pass
		{
		    ColorMask A
		    Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragA
			ENDCG
		}

	}
}
