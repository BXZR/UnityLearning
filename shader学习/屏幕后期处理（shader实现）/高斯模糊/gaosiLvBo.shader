// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "suckScreen/gaosiLvBo"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BlurSize ("blurSize" , float)  = 1.0
	}
	SubShader
	{

	    //内部定义一个所有pass通用的方法集合，所有的pass直接调用就可以
	    CGINCLUDE
	    #include "UnityCG.cginc"
	    //通用的公有代码
		sampler2D _MainTex;//必须要用这个变量名和属性名
		half4 _MainTex_TexelSize;//获得的额外参数：像素大小的倒数
		float _BlurSize;

		//共用的结构体
		struct v2f
		{
		 float4 pos : SV_POSITION;
		 half2 uv[5] : TEXCOORD0;
		};

		v2f vertH(appdata_img v)//横轴
		{
		   v2f o;
		   o.pos = UnityObjectToClipPos( v.vertex);

		   half2 uv = v.texcoord;
		   o.uv[0] = uv;
		   o.uv[1] = uv + float2(_MainTex_TexelSize.y *1.0,0.0) * _BlurSize;
		   o.uv[2] = uv - float2(_MainTex_TexelSize.y *1.0,0.0) * _BlurSize;
		   o.uv[3] = uv + float2(_MainTex_TexelSize.y *2.0,0.0) * _BlurSize;
		   o.uv[4] = uv - float2(_MainTex_TexelSize.y *2.0,0.0) * _BlurSize;
		   return o;
		}

		  v2f vertV(appdata_img v)//纵轴
            {
               v2f o;
               o.pos = UnityObjectToClipPos( v.vertex); 

               half2 uv = v.texcoord;
               o.uv[0] = uv;
               o.uv[1] = uv + float2(0.0 , _MainTex_TexelSize.y *1.0) * _BlurSize;
               o.uv[2] = uv - float2(0.0 , _MainTex_TexelSize.y *1.0) * _BlurSize;
               o.uv[3] = uv + float2(0.0 , _MainTex_TexelSize.y *2.0) * _BlurSize;
               o.uv[4] = uv - float2(0.0 , _MainTex_TexelSize.y *2.0) * _BlurSize;
               return o;
            }

		fixed4 frag (v2f i) : SV_Target
		{
		    float weight[3] = {0.4026 , 0.2442 , 0.0545};
		    fixed3 sum = tex2D (_MainTex , i.uv[0]).rgb *weight[0];
		    for(int j=1 ;j<3 ; j++)
		    {
		       sum += tex2D(_MainTex , i.uv[j*2-1]).rgb * weight[j];
		        sum += tex2D(_MainTex , i.uv[j*2]).rgb * weight[j];
		    }
		    return fixed4 (sum ,1.0);
		}

		ENDCG
		ZTest Always Cull Off ZWrite Off

		//第一个pass
		Pass
		{

		    NAME "HCOV"//横轴的卷积
			CGPROGRAM
			#pragma vertex vertH
			#pragma fragment frag
			ENDCG
		}

		//第二个pass
		Pass
		{
		    NAME "VCOV"//纵轴的卷积
			CGPROGRAM
			#pragma vertex vertV
			#pragma fragment frag
          
			ENDCG
		}

	}
	FallBack "Diffuse"
}
