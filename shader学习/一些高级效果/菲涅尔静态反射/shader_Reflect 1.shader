Shader "Sucks/shader_Reflect_Fr"
{
	Properties
	{
		_Color("Color" , color) = (1,1,1,1)
		_ReflectColor ("Reflection Color", Color) = (1,1,1,1)//反射颜色
		_ReflectAmount ("RTeflect Amount", range(0,1)) = 1
		_CubeMap ("Cube Map" , cube) = "_skybox"{}
		_FresnelScale ("FresnelScale" , Range(0,1)) = 0.5
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 _Color;
			fixed4 _ReflectColor;

			fixed _ReflectAmount;
			fixed _FresnelScale;
			//cube的内置类型
			samplerCUBE _CubeMap;
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#pragma multi_compile_fwdbase

			//输入结构体
			struct a2v
			{
			    float4 vertex : POSITION;//获取位置矩阵，存到vertex的矩阵里面
			    float3 normal : NORMAL;//获取发现信息矩阵，存到normal里面
			};

			//中间结构体（现在的理解不算是输入输出）
			struct v2f 
			{
			  float4 pos :SV_POSITION;
			  float3 worldPos :TEXCOORD0;//使用寄存器0来保存世界坐标的值
			  fixed3 worldNormal : TEXCOORD1;
			  fixed3 worldViewDir : TEXCOORD2;
			  fixed3 worldRefl : TEXCOORD3;
			  SHADOW_COORDS(4)//创建阴影的套路中的第一步，选定寄存器
			};

			//片元着色器的处理方法
			v2f vert(a2v v)
			{
			  v2f o;
			  //进行坐标转换,获取模型坐标
			  o.pos = UnityObjectToClipPos( v.vertex);
			  o.worldNormal = UnityObjectToWorldNormal(v.normal);
			  o.worldPos = mul (unity_ObjectToWorld , v.vertex).xyz;
			  o.worldViewDir = UnityWorldSpaceViewDir(o.worldPos);
			  o.worldRefl = reflect (-o.worldViewDir , o.worldNormal);
			  //产生影子套路的第二步
			  TRANSFER_SHADOW(o);
			  return o;			  
			}

			fixed4 frag (v2f i) : SV_Target
			{
			    fixed3 worldNormal = normalize (i.worldNormal);
			    fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
			    fixed3 worldViewDir = normalize (i.worldViewDir);
			    //基础光照 
			    //获取到基础的光照
			    //并且有自己设置的颜色加权
			    fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * _Color.xyz;
			    //漫反射
			    fixed3 diffuse  = _LightColor0 .rgb *_Color * max (0,dot(worldNormal, worldViewDir));
			    //调用API做的立方体纹理赋值
			    fixed3 reflection = texCUBE (_CubeMap , i.worldRefl).rgb * _ReflectColor.rgb;

			    fixed3 fresnel = _FresnelScale+( 1- _FresnelScale) *pow (1-dot(worldNormal, worldViewDir),5);

			    	UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
			    return fixed4 (ambient + lerp(diffuse , reflection , saturate (fresnel))*atten,1.0);
			}

			ENDCG
		}
	}
		FallBack "Reflective/VertexLit"//为了使用阴影制作的通用pass这里的fallback还是应该加上的
}
