
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Suck/alpha_Use_2"
{
  //使用透明度测试的方法的“透明度”
  //用A通道同一控制，很有效
  //这个脚本中还开启一个新的渲染通道记录深度缓存的值，实现模型整体不因透明度互相穿透，且整体拥有透明效果
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

		//开启一个额外的渲染通道来“做标记”
		pass
		{
		   ZWrite On//开启深度缓冲写入
		   ColorMask 0//渲染命令，标记为0的时候为这个pass不写入任何颜色通道，也就是说不会输出颜色
		}

		Pass
		{
		   Tags {"LightMode"="ForwardBase"}
		   Zwrite off//关掉Z缓存的写入
		   Blend SrcAlpha oneMinusSrcAlpha//开启混合模式
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
