Shader "SuckSurfaceShaders/surfaceShader1"
{
	 Properties
	 {
	   _Color ("Color" , Color) = (1,1,1,1)
	   _MainTex ("MainTexture" , 2D) = "white"{}
	   _BumpMap ("BumpMap" , 2D ) = "bump"{}
	 }
	 SubShader
	 {
	      Tags {"RenderType" = "Opaque"}
	      LOD 300 //人为规定LOD的数值

	      CGPROGRAM
	      #pragma surface surf Lambert
	      #pragma target 3.0

	      sampler2D _MainTex;
	      sampler2D _BumpMap;
	      fixed4 _Color;

	      struct Input
	      {
	        float2 uv_MainTex;
	        float2 uv_BumpMap;
	      };

	      void surf (Input IN , inout SurfaceOutput o)
	      {
	        fixed4 tex = tex2D (_MainTex , IN.uv_MainTex);
	        o.Albedo = tex.rgb * _Color.rgb;
	        o.Alpha = tex.a * _Color.a;
	        o.Normal = UnpackNormal(tex2D (_BumpMap , IN.uv_BumpMap));
	      }

	      ENDCG
	 }
	 FallBack "Legacy Shaders/Diffuse"
}
