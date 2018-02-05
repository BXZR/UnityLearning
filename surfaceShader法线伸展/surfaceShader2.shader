Shader "SuckSurfaceShaders/surfaceShader2"
{
   //surfaceShader的扩张的例子
	Properties
	{
	    _ColorTint("Color Tint"  , Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_BumpMap("BumpMap" , 2D) = "white"{}
		_Amount ("Amount" , Range(-0.5 , 0.5)) = 0.1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		CGPROGRAM

		#pragma surface surf CustomLambert  vertex:myvert finalcolor:mycolor addshadow exclude_path:deferred exclude_path:prepass nometa keepalpha
	  //空格不可以乱打，下面这句与上面除了空格都一样，但是无效果
      //  #pragma surface surf CustomLambert vertex : myvert finalcolor : muycolor addshadow exclude_path : deferred exclude_path : presspass nometa
        #pragma target 3.0

        fixed4 _ColorTint;
        sampler2D _MainTex;
        sampler2D _BumpMap;
        half _Amount;

        struct Input
        {
         float2 uv_MainTex;
         float2 uv_BumpMap;
        };

        void myvert(inout appdata_full v)
        {
            v.vertex.xyz += v.normal * _Amount;//向着法线的方向伸展
        }

        void surf (Input IN , inout SurfaceOutput o)
        {
           fixed4 tex = tex2D(_MainTex , IN.uv_MainTex);
           o.Albedo = tex.rgb;//朱文丽设置反射率
           o.Alpha = tex.a * _ColorTint.a;
           o.Normal = UnpackNormal ( tex2D (_BumpMap , IN.uv_BumpMap));
        }

        half4 LightingCustomLambert(SurfaceOutput s , half3 lightDir , half atten)
        {
	        half NdotL = dot (s.Normal , lightDir);
	        half4 c;
	        c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten);
	        c.a = s.Alpha;
	        return c;
        }

        void mycolor (Input IN , SurfaceOutput o , inout fixed4 color)
        {
           color *= _ColorTint;
        }

		ENDCG
	}

	FallBack "Legacy Shaders/Diffuse"
}
