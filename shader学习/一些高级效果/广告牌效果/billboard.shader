Shader "Unlit/billboard"
{
	Properties
	{
		 _Texture("Texture" , 2D ) = "white"{}
		 _Color ("Color" , Color) = (1,1,1,1)
		 _Type("Type" , Range(0,1)) = 1
	}
	SubShader
	{
 
		Tags {"Queue" = "Transparent"  "IgnoreProjector" = "True" "RenderType" = "Transparent" "DisableBatching" = "True"}
		Pass
		{
		    zwrite off
		    cull off
		    Blend SrcAlpha OneMinusSrcAlpha
		    Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _Texture;
			float4 _Texture_ST;
			fixed4 _Color;
			fixed _Type;

			 struct a2v {
			 //只是为了显示最基本的贴图内容
			 float4 vertex : POSITION;
			 float4 texcoord : TEXCOORD0;
			};
			
			struct v2f
			 {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			v2f vert (a2v v) {
				v2f o;
				//下面这两句是杆件代码，用原点作为广告牌的旋转锚点
				float3 center = float3(0, 0, 0);
				float3 viewer = mul(unity_WorldToObject,float4(_WorldSpaceCameraPos, 1));
				
				float3 normalDir = viewer - center;
				normalDir.y =normalDir.y * _Type;
				normalDir = normalize(normalDir);


				float3 upDir = abs(normalDir.y) > 0.999 ? float3(0, 0, 1) : float3(0, 1, 0);
				float3 rightDir = normalize(cross(upDir, normalDir));
				upDir = normalize(cross(normalDir, rightDir));
				
				//计算新的顶点
				float3 centerOffs = v.vertex.xyz - center;
				float3 localPos = center + rightDir * centerOffs.x + upDir * centerOffs.y + normalDir * centerOffs.z;
              
				o.pos = UnityObjectToClipPos(float4(localPos, 1));
				o.uv = TRANSFORM_TEX(v.texcoord,_Texture);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				fixed4 c = tex2D (_Texture, i.uv);
				c.rgb *= _Color.rgb;
				
				return c;
			}
			
			ENDCG
		}
	}
}
