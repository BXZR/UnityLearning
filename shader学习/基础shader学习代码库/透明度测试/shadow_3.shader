// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sucks/alpha_Shadow"
{
  //使用透明度测试的方法的“透明度”
  //个人认为是一个高效的不推荐的方法
  //并非是半透明效果
	Properties
	{
	    _Color ("Color" , Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_Cutoff ("Alpha CutOff" , range (0,1)) = 0.5
	}


	SubShader
	{
	   //"AlphaTest"透明度测试
	    //"TransparentCutout"把这个shader归到TransparentCutout
		Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		Pass
		{
		   Tags {"LightMode"="ForwardBase"}
		    //CG脚本代码块
			CGPROGRAM
			//预编译控制，指定顶点着色器和片面着色器的调用方法
			#pragma multi_compile_fwdbase

			#pragma vertex vert
			#pragma fragment frag
			//为了获取光照信息，这个头文件需要被引用
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
	
			//定义与属性相对应的变量
			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _Cutoff;

			struct a2v
			{
			    float4 vertex : POSITION;
			    float3 normal : NORMAL;
			    float4 texcoord : TEXCOORD0;
			};

			struct v2f
			{
			   float4 pos :SV_POSITION;
			   float3 worldNormal : TEXCOORD0;
			   float3 worldPos : TEXCOORD1;
			   float2 UV : TEXCOORD2;
			   SHADOW_COORDS(3)
			};

		    //顶点着色器
		    v2f vert (a2v v)
		    {
		      v2f o;
		      //坐标转换
		      o.pos = UnityObjectToClipPos ( v.vertex);
		      //变换之后的法线坐标
		      o.worldNormal = UnityObjectToWorldNormal(v.normal);
		      //变换之后的世界坐标
		      o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		      //变换之后的纹理坐标
		      o.UV = TRANSFORM_TEX(v.texcoord,_MainTex);

		      TRANSFER_SHADOW(o);
		      return o;
		    }

		     fixed4 frag(v2f i) : SV_Target
		     {
		        fixed3 worldNormal = normalize(i.worldNormal);
		        fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
		        fixed4 texColor = tex2D(_MainTex , i.UV);
		        //透明度测试的方法
		        clip(texColor.a - _Cutoff);
		        //强制控制漫反射率的颜色
		        fixed3 albedo = texColor.rgb *_Color.rgb;
		        fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
		        fixed3 diffuse = _LightColor0.rgb * albedo * max(0,dot(worldNormal , worldLightDir));
		        UNITY_LIGHT_ATTENUATION(atten , i ,i.worldPos);
		        return fixed4(ambient + diffuse * atten ,1.0);

		     }
	
			ENDCG
		}
	}

	//FallBack "VertexLit"//因为要使用这个文件里面shadowCaster的pass投射阴影
	FallBack "Transparent/Cutout/VertexLit"
	//因为要使用这个文件里面shadowCaster（带透明度测试）的pass投射阴影

}
