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

 

			float4 verts(float4 f :POSITION) : SV_POSITION
			{
			    return UnityObjectToClipPos (f);
			}
			fixed4 frag() : SV_Target
			{
			   return fixed4 (0.8, 0.7, 0.6 ,1.0);
			}
 
			ENDCG
		}
	}
}
