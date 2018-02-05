// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Suck/shaderStep1"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex verts
			#pragma fragment frag

			struct demoStruct
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
			};

			float4 verts(demoStruct f) : SV_POSITION
			{
			    return UnityObjectToClipPos (f.vertex );
			}
			fixed4 frag() : SV_Target
			{
			   return fixed4 (0.8, 0.7, 0.6 ,1.0);
			}
 
			ENDCG
		}
	}
}
