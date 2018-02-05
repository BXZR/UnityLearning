// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "C15/dissolve"
{
	Properties
	{
	   _BurnAmount ("brun amount" , range (0.0,1.0)) = 0.0
	   _LineWidth ("burn line width" , Range(0.0 , 0.2)) = 0.1
	   _MainTex ("Main texture" , 2D) = "white"{}
	   _BumpMap ("normal map" , 2D) = "white"{}
	   _BurnFirstColor ("burn first color" , color)  = (1,0,0,1)
	   _BurnSecondColor ("burn second color" , color) = (1,0,0,1)
	   _BurnMap ("burn map" , 2D) = "white"{} 
	}
	SubShader
	{
		Pass
		{
		   Tags {"LightMode" = "ForwardBase"}
		   cull off//为了展现消融效果的真实性，应该关掉cull
			CGPROGRAM
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
 

            fixed _BurnAmount ;
            fixed _LineWidth;
            sampler2D _MainTex;
            sampler2D _BumpMap;
            fixed4 _BurnFirstColor;
            fixed4 _BurnSecondColor;
            sampler2D _BurnMap;
            float4 _MainTex_ST;
            float4 _BumpMap_ST;
            float4 _BurnMap_ST;

			struct a2v 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
			};
            struct v2f
            {
              float4 pos : SV_POSITION;
              float2 uvMainTex : TEXCOORD0;
              float2 uvBumpMap : TEXCOORD1;
              float2 uvBurnMap : TEXCOORD2;
              float3 lightDir : TEXCOORD3;
              float3 worldPos : TEXCOORD4;
              SHADOW_COORDS(5)//为阴影配置寄存器
            };

            v2f vert (a2v v)
            {
              v2f o;
              o.pos = UnityObjectToClipPos ( v.vertex);

              //获取三个贴图的UV信息并且保存下来
              o.uvMainTex = TRANSFORM_TEX(v.texcoord , _MainTex);
              o.uvBumpMap = TRANSFORM_TEX(v.vertex , _BumpMap);
              o.uvBurnMap = TRANSFORM_TEX(v.vertex , _BurnMap);

              TANGENT_SPACE_ROTATION;//获取模型空间到切线空间的变换矩阵
              o.lightDir = mul (rotation , ObjSpaceLightDir (v.vertex)).xyz; 
              o.worldPos = mul (unity_ObjectToWorld , v.vertex).xyz; 

              TRANSFER_SHADOW(o);

              return o;
            }

            fixed3 frag(v2f i): SV_Target
            {
              fixed3 burn = tex2D(_BurnMap , i.uvBurnMap);
              clip (burn.r - _BurnAmount);//透明度检查，将低于阀值的部分删除掉不显示

              float3 tangentLightDir = normalize (i.lightDir);
              fixed3 tangentNormal = UnpackNormal (tex2D(_BumpMap , i.uvBumpMap));

              fixed3 albedo = tex2D(_MainTex , i.uvMainTex).rgb;

              fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz *albedo;

              fixed3 diffuse = _LightColor0.rgb * albedo * max(0,dot(tangentNormal , tangentLightDir));

              fixed t = 1- smoothstep (0.0, _LineWidth , burn.r - _BurnAmount);
              fixed3 burnColor = lerp (_BurnFirstColor , _BurnSecondColor , t);
              burnColor = pow (burnColor , 5);

              UNITY_LIGHT_ATTENUATION(atten , i,i.worldPos);

              fixed3 finalColor = lerp (ambient + diffuse *atten , burnColor ,t*step (0.0001,_BurnAmount));
              return fixed4(finalColor, 1);
            }

			ENDCG
		}

		pass // 用于阴影投射的pass(防止消融的时候投射阴影)
		{
		  Tags {"LightMode" = "ShadowCaster"}
		  CGPROGRAM
		  #pragma vertex vert
		  #pragma fragment frag
		  #pragma multi_compile_shadowcaster
		  #include "UnityCG.cginc"

		  sampler2D _BurnMap;
		  fixed _BurnAmount ;
		  float4 _BurnMap_ST;

		   struct v2f
		   {
		     V2F_SHADOW_CASTER;
		     float2 uvBurnMap : TEXCOORD1;
		   };

		   v2f vert (appdata_base v)
		   {
		     v2f o;
		     TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
		     o.uvBurnMap = TRANSFORM_TEX(v.texcoord , _BurnMap);
		     return o;
		   }

		   fixed4 frag(v2f i):SV_Target
		   {
		     fixed3 burn = tex2D(_BurnMap , i.uvBurnMap).rgb;
		    clip(burn.r - _BurnAmount);
		     SHADOW_CASTER_FRAGMENT(i)
		   }
		   ENDCG
		}
	}
}
