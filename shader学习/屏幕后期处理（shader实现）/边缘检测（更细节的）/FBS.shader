// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/FBS"
{
	Properties
	{
	    _MainTex ("MainTexture" , 2D) = "white"{}
		_EdgeOnly ("_EdgeOnly", Float) = 1.0
		_EdgeColor("EdgeColor" , Color) = (1,1,1,1)
		_BackgroundColor("_BackgroundColor" , Color) = (1,1,1,1)
		_SampleDistance("_SampleDistance", float) = 1.0
		_Sensitivity ("_Sensitivity" , Vector) = (1,1,1,1)
	}
	SubShader
	{
	   //总控用代码块
	    CGINCLUDE

	    #include "UnityCG.cginc"

	    sampler2D _MainTex;
	    half4 _MainTex_TexelSize;
	    fixed _EdgeOnly;
	    fixed4 _EdgeColor;
	    fixed4 _BackgroundColor;
	    float _SampleDistance;
	    half4 _Sensitivity;
	    sampler2D _CameraDepthNormalsTexture;//内置内容，名字固定了

	    struct v2f
	    {
	      float4 pos : SV_POSITION;
	      half2 uv[5] : TEXCOORD0;
	    };

	    v2f vert(appdata_img v)
	    {
	      v2f o;
	      o.pos = UnityObjectToClipPos( v.vertex);

	      half2 uv = v.texcoord;
	      o.uv[0] = uv;//原始UI坐标

	      //平台性质的差异
	       #if UNITY_UV_STARTS_AT_TOP
	       if(_MainTex_TexelSize .y <0)
	       uv.y = 1 - uv.y;
	       #endif
	      //获取到的上下左右四个元素的坐标
	      //可以有距离加成
	      o.uv[1] = uv + _MainTex_TexelSize.xy * half2 (1,1) * _SampleDistance;
	      o.uv[2] = uv + _MainTex_TexelSize.xy * half2 (-1,-1) * _SampleDistance;
	      o.uv[3] = uv + _MainTex_TexelSize.xy * half2 (-1,1) * _SampleDistance;
	      o.uv[4] = uv + _MainTex_TexelSize.xy * half2 (1,-1) * _SampleDistance;

	      return o;
	    }

 
	    half checkSame (half4 center , half4 sample)
	    {
	        half2 centerNormal = center.xy;
	        //跟颜色的BA值想相关
			float centerDepth = DecodeFloatRG(center.zw);
			half2 sampleNormal = sample.xy;
			float sampleDepth = DecodeFloatRG(sample.zw);

			half2 diffNormal = abs(centerNormal - sampleNormal) * _Sensitivity.x;
			int isSameNormal = (diffNormal.x + diffNormal.y) < 0.1;
			// difference in depth
			float diffDepth = abs(centerDepth - sampleDepth) * _Sensitivity.y;
			// scale the required threshold by the distance
			int isSameDepth = diffDepth < 0.1 * centerDepth;
			
			// return:
			// 1 - if normals and depth are similar enough
			// 0 - otherwise

			//为了保证风格，这块做了小修改
			//这也说明shader是更偏向于C风格的
			return isSameNormal * isSameDepth == 1.0;
 
	    }

	    fixed4 frag (v2f i) : SV_Target
	    {
	      half4 sample1 = tex2D (_CameraDepthNormalsTexture ,i.uv[1]);
	      half4 sample2 = tex2D (_CameraDepthNormalsTexture ,i.uv[2]);
	      half4 sample3 = tex2D (_CameraDepthNormalsTexture ,i.uv[3]);
	      half4 sample4 = tex2D (_CameraDepthNormalsTexture ,i.uv[4]);

            half edge = 1.0;
			
			edge *= checkSame(sample1, sample2);
			edge *= checkSame(sample3, sample4);
			
			fixed4 withEdgeColor = lerp(_EdgeColor, tex2D(_MainTex, i.uv[0]), edge);
			fixed4 onlyEdgeColor = lerp(_EdgeColor, _BackgroundColor, edge);
	        return lerp (withEdgeColor , onlyEdgeColor , _EdgeOnly);
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
}
