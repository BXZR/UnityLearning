// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CARTOOONSUCK/TAP"
{
	Properties
	{
		_Color ("Color" , color ) = (1,1,1,1)
		_TileFactor ("TileFactor" , float) = 1 //纹理平铺系数
		_OutLine ("outLint" , Range(0,1)) = 0.1
		//六张素描纹理
		_Hatch0 ("hatch0" , 2D) = "white"{}
		_Hatch1 ("hatch1" , 2D) = "white"{}
		_Hatch2 ("hatch2" , 2D) = "white"{}
		_Hatch3 ("hatch3" , 2D) = "white"{}
	    _Hatch4 ("hatch4" , 2D) = "white"{}
		_Hatch5 ("hatch5" , 2D) = "white"{}
		 
	}
	SubShader
	{
	    Tags{"RenderType" = "Opaque"  "Queue" = "Geometry"}
	    usePass "CARTOOONSUCK/cartoon/OUTLINE" //直接引用outLine的pass (因为unity会把所有的pass名称转换成大写的，所以引用的时候用大写)
		Pass
		{
		 Tags{"LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag 
			
			#pragma multi_compile_fwdbase

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "UnityShaderVariables.cginc"

			fixed4 _Color;
			float _TileFactor;
			sampler2D _Hatch0;
			sampler2D _Hatch1;
			sampler2D _Hatch2;
			sampler2D _Hatch3;
			sampler2D _Hatch4;
			sampler2D _Hatch5;

 
		    struct a2v {
				float4 vertex : POSITION;
				float4 tangent : TANGENT; 
				float3 normal : NORMAL; 
				float2 texcoord : TEXCOORD0; 
			};
			
			struct v2f {
			    float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				fixed3 hatchWeight0 : TEXCOORD1;
				fixed3 hatchWeight1 : TEXCOORD2;
				float3 worldPos : TEXCOORD3;
				SHADOW_COORDS(4)
			};
			
			//顶点着色器
			v2f vert (a2v v)
			{
				v2f o;
				
				o.pos = UnityObjectToClipPos(v.vertex);
				
				o.uv = v.texcoord.xy * _TileFactor;
				
				fixed3 worldLightDir = normalize(WorldSpaceLightDir(v.vertex));
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				fixed diff = max(0, dot(worldLightDir, worldNormal));
				
			    o.hatchWeight0 = fixed3(0, 0, 0);
				o.hatchWeight1 = fixed3(0, 0, 0);
				
			  float hatchFactor = diff * 7.0;

			  if(hatchFactor>6.0)
			  {
			    //纯白
			  }
			  else if(hatchFactor>5.0)
			  {
			   o.hatchWeight0.x = hatchFactor - 5.0;
			  }
			  else if(hatchFactor>4.0)
			  {
			   o.hatchWeight0.x = hatchFactor - 4.0;
			   o.hatchWeight0.y = 1.0 - o.hatchWeight0.x ;
			  }
			  else if(hatchFactor>3.0)
			  {
			   o.hatchWeight0.y = hatchFactor - 3.0;
			   o.hatchWeight0.z = 1.0 - o.hatchWeight0.y ;
			  }
			  else if(hatchFactor>2.0)
			  {
			   o.hatchWeight0.z = hatchFactor - 2.0;
			   o.hatchWeight1.x = 1.0 - o.hatchWeight0.z ;
			  }
			  else if(hatchFactor>1.0)
			  {
			   o.hatchWeight1.x = hatchFactor - 1.0;
			   o.hatchWeight1.y = 1.0 - o.hatchWeight1.x;
			  }
			  else  
			  {
			   o.hatchWeight1.y = hatchFactor ;
			   o.hatchWeight1.z = 1.0 - o.hatchWeight1.y;
			  }

			  o.worldPos = mul (unity_ObjectToWorld , v.vertex).xyz;

			  TRANSFER_SHADOW(o);

			  return o;
			}

		fixed4 frag(v2f i) : SV_Target {			
				fixed4 hatchTex0 = tex2D(_Hatch0, i.uv) * i.hatchWeight0.x;
				fixed4 hatchTex1 = tex2D(_Hatch1, i.uv) * i.hatchWeight0.y;
				fixed4 hatchTex2 = tex2D(_Hatch2, i.uv) * i.hatchWeight0.z;
				fixed4 hatchTex3 = tex2D(_Hatch3, i.uv) * i.hatchWeight1.x;
				fixed4 hatchTex4 = tex2D(_Hatch4, i.uv) * i.hatchWeight1.y;
				fixed4 hatchTex5 = tex2D(_Hatch5, i.uv) * i.hatchWeight1.z;
				fixed4 whiteColor = fixed4(1, 1, 1, 1) * (1 - i.hatchWeight0.x - i.hatchWeight0.y - i.hatchWeight0.z - 
							i.hatchWeight1.x - i.hatchWeight1.y - i.hatchWeight1.z);
				
				fixed4 hatchColor = hatchTex0 + hatchTex1 + hatchTex2 + hatchTex3 + hatchTex4 + hatchTex5 + whiteColor;
				
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
								
				return fixed4(hatchColor.rgb * _Color.rgb * atten, 1.0);
			}
 
			ENDCG
		}
	}
	FallBack "Diffuse"
}
