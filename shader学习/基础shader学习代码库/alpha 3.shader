
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Suck/alpha_Use_d"
{
  //正确的双面渲染+透明度混合
  //使用两个渲染通道分别绘制正面和背面

	Properties
	{
	    _Color ("Color" , Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_AlphaScale ("AlphaScale " , range (0,1)) = 1
	}


	SubShader
	{
	   //"AlphaTest"透明度测试
	    //"TransparentCutout"把这个shader归到TransparentCutout
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

		Pass
		{
		   Tags {"LightMode"="ForwardBase"}
		   Zwrite off//关掉Z缓存的写入
		   cull front
		   Blend SrcAlpha oneMinusSrcAlpha//开启混合模式非常重要的（）
		    //CG脚本代码块
			CGPROGRAM
			//预编译控制，指定顶点着色器和片面着色器的调用方法
			#pragma vertex vert
			#pragma fragment frag
			//为了获取光照信息，这个头文件需要被引用
			#include "Lighting.cginc"

			//定义与属性相对应的变量
			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _AlphaScale;

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
		      return o;
		    }

		     fixed4 frag(v2f i) : SV_Target
		     {
		        //世界法线计算
		        fixed3 worldNormal = normalize(i.worldNormal);
		        //光照方向计算
		        fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
		        //点的颜色，用纹理和纹理坐标获取得到
		        fixed4 texColor = tex2D(_MainTex , i.UV);
		        float theAUse = 1.0;
		        if(texColor.a < 0.7)
		        {
		           theAUse = texColor.a;
		        }
		        //强制控制漫反射率的颜色(个人认为是主基调)
		        fixed3 albedo = texColor.rgb *_Color.rgb;
		        //基础光照效果（光照颜色，自选颜色，贴图颜色都有控制最终的颜色）
		        fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
		        //漫反射效果（兰伯特光照模型）
		        //顺带一提，这个是在片元着色器计算的漫反射，效率比在顶点的低下，凡是效果会好很多
		        fixed3 diffuse = _LightColor0.rgb * albedo * max(0,dot(worldNormal , worldLightDir));
		        //最后的叠加在这一步计算得到（例如没有高光就不加上就好）
		        return fixed4(ambient + diffuse , theAUse * _AlphaScale);

		     }
	
			ENDCG
		}


		Pass
		{
		   Tags {"LightMode"="ForwardBase"}
		   Zwrite off//关掉Z缓存的写入
		   cull back
		   Blend SrcAlpha oneMinusSrcAlpha//开启混合模式非常重要的（）
		    //CG脚本代码块
			CGPROGRAM
			//预编译控制，指定顶点着色器和片面着色器的调用方法
			#pragma vertex vert
			#pragma fragment frag
			//为了获取光照信息，这个头文件需要被引用
			#include "Lighting.cginc"

			//定义与属性相对应的变量
			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _AlphaScale;

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
		      return o;
		    }

		     fixed4 frag(v2f i) : SV_Target
		     {
		        //世界法线计算
		        fixed3 worldNormal = normalize(i.worldNormal);
		        //光照方向计算
		        fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
		        //点的颜色，用纹理和纹理坐标获取得到
		        fixed4 texColor = tex2D(_MainTex , i.UV);
		        float theAUse = 1.0;
		        if(texColor.a < 0.7)
		        {
		           theAUse = texColor.a;
		        }
		        //强制控制漫反射率的颜色(个人认为是主基调)
		        fixed3 albedo = texColor.rgb *_Color.rgb;
		        //基础光照效果（光照颜色，自选颜色，贴图颜色都有控制最终的颜色）
		        fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
		        //漫反射效果（兰伯特光照模型）
		        //顺带一提，这个是在片元着色器计算的漫反射，效率比在顶点的低下，凡是效果会好很多
		        fixed3 diffuse = _LightColor0.rgb * albedo * max(0,dot(worldNormal , worldLightDir));
		        //最后的叠加在这一步计算得到（例如没有高光就不加上就好）
		        return fixed4(ambient + diffuse , theAUse * _AlphaScale);

		     }
	
			ENDCG
		}


	}

	FallBack "Standard"
}
