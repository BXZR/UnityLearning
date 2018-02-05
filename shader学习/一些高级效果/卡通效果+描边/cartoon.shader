// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CARTOOONSUCK/cartoon"
{
	Properties
	{
		 _Color ("Color" , Color) = (1,1,1,1)
		 _MainTex("MainTex", 2D) =  "white" {}
		 _Ramp ("Ramp" , 2D) = "white" {}
		 _OutLine ("outLine" , Range(0,1)) = 0.1
		 _OutLineColor ("outLineColor" , Color) = (1,1,1,1)
		 _Specuar ("specular" , Color) = (1,1,1,1)
		 _SpecularScale("SpecularScale" , Range(0,0.1)) = 0.01
	} 
	SubShader
	{
		//这个pass仅仅绘制背面，展现出来的就是个边缘
		Pass
		{
	        NAME "outline" //这个name写出来是为了别的文件可以重复调用之
	        cull front
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			float _OutLine;
			fixed4 _OutLineColor;
			
			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			}; 
			
			struct v2f {
			    float4 pos : SV_POSITION;
			};
			
			v2f vert (a2v v)
			{
			  v2f  o;
			  float4 pos = mul (UNITY_MATRIX_MV , v.vertex);
			  float3 normal = mul ((float3x3)UNITY_MATRIX_IT_MV , v.normal);
			  normal.z = -0.5;
			  pos = pos +float4 (normalize (normal) , 0) *_OutLine;
			  o.pos = mul(UNITY_MATRIX_P , pos);
			  return o;
			}

			float4 frag (v2f i) : SV_Target
			{
			  return float4 (_OutLineColor.rgb , 1);
			}
			ENDCG
		}

		//绘制正面的pass
		pass
		{
		  Tags {"LightMode" = "ForwardBase"}
		   cull back
		   CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
 
			#pragma multi_compile_fwdbase

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "UnityShaderVariables.cginc"
			
			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _Ramp;
			fixed4 _Specular;
			fixed _SpecularScale;


			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				float4 tangent : TANGENT;
			}; 
		
			struct v2f {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				SHADOW_COORDS(3)
			};
			
			v2f vert (a2v v) {
				v2f o;
				
				o.pos = UnityObjectToClipPos( v.vertex);
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				o.worldNormal  = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				
				TRANSFER_SHADOW(o);
				
				return o;
			}
			
			float4 frag(v2f i) : SV_Target { 
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
				fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				fixed3 worldHalfDir = normalize(worldLightDir + worldViewDir);
				
				fixed4 c = tex2D (_MainTex, i.uv);
				fixed3 albedo = c.rgb * _Color.rgb;
				
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
				
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
				
				fixed diff =  dot(worldNormal, worldLightDir);
				diff = (diff * 0.5 + 0.5) * atten;
				
				fixed3 diffuse = _LightColor0.rgb * albedo * tex2D(_Ramp, float2(diff, diff)).rgb;
				
				fixed spec = dot(worldNormal, worldHalfDir);
				fixed w = fwidth(spec) * 2.0;
				fixed3 specular = _Specular.rgb * lerp(0, 1, smoothstep(-w, w, spec + _SpecularScale - 1)) * step(0.0001, _SpecularScale);
				
				return fixed4(ambient + diffuse + specular, 1.0);
			} 
			ENDCG
		}

	}
}
