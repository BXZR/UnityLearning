// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'

Shader "Sucks/shadow_2"
{

  //两个通道的光照效果
  //其中使用base pass存储一些公有的光照信息
  //用接下来的pass通过叠加的方式显示多光源的光照效果信息
	Properties
	{
	     //定义属性
	     //漫反射颜色
		 _Diffuse ("Diffuse" , color) = (1,1,1,1)
		 //高光颜色
		 _Specular("Specular" , color) = (1,1,1,1)
		 //高光一范围大小
		 _Gloss ("Gloss", Range(8.0 , 256 )) = 20
	}
	SubShader
	{
		Pass
		{
		    //这个是basePass所以需要标记
		    Tags {"LightMode" = "ForwardBase"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//为了获取base pass的光照信息需要引用的空间
		   	#pragma multi_compile_fwdbase
		   	//为了获取光照信息需要引用的控件
		   	#include "Lighting.cginc"
		   	#include "AutoLight.cginc"//这里加入貌似是为了获取到阴影信息
		    //属性对应的“字段”变量（个人认为用字段似乎更加好理解）
		   	fixed4 _Diffuse;
		   	fixed4 _Specular;
		   	float _Gloss;

		   	//输入结构体
		   	struct a2v
		   	{
		   	   //从模型空间中获取顶点坐标（个人理解）
		   	   float4 vertex : POSITION;
		   	   //从模型空间中获取顶点法线（个人理解）
		   	   float3 normal : NORMAL;
		   	};

		   	//输出结构体
		   	struct v2f
		   	{
		   	   //目标空间建立“引用”
		   	   float4 pos : SV_POSITION;
		   	   //用REXCOORD做存储信息的“格式”（个人理解）
		   	   float3 worldNormal : TEXCOORD0;
		   	   float3 worldPosition : TEXCOORD1;
		   	   SHADOW_COORDS(2)//阴影计算需要添加的内置宏SHADOW_COORDS
		   	   //参数是下一个可用的插值寄存器的索引值
		   	};

		    //顶点着色器（赋值工作由引擎自自行处理）
		   	v2f vert (a2v v)
		   	{
		   	    //返回的v2f结果
		   	    v2f o;
		   	    //记录信息，这个方法将模型空间转换成了世界坐标
		   	    o.pos = UnityObjectToClipPos( v.vertex);
		   	    //同样，获取世界法线
		   	    o.worldNormal = UnityObjectToWorldNormal(v.normal);
		   	    //同样，获取世界坐标信息
		   	    o.worldPosition = mul(unity_ObjectToWorld , v.vertex) .xyz;
		   	    //宏计算阴影纹理坐标
		   	    TRANSFER_SHADOW(o)
		   	    return o;
		   	}

		   	//片元着色器 （返回值为 fixed4 输出给 SV_Target）
		   	fixed4 frag(v2f i) : SV_Target
		   	{
		   	   //世界法线方向的标准化
		   	   fixed3 worldNormal = normalize (i.worldNormal);
		   	   //世界光照方向标准化
		   	   fixed3 worldLightDir = normalize (_WorldSpaceLightPos0.xyz);
		   	   //获取基础光照信息（平行光）
		   	   fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
		   	   //计算漫反射（半兰伯特光照模型）
		   	   fixed3 diffuse = _LightColor0 .rgb * _Diffuse.rgb * max (0,dot (worldNormal, worldLightDir));
		   	   //高光Blinn-phong光照模型
			   fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPosition.xyz);
			   fixed3 halfDir = normalize(worldLightDir + viewDir);
		   	   fixed3 specular = _LightColor0.rgb *_Specular .rgb * pow(max (0, dot(worldNormal , halfDir)), _Gloss);
		   	   //额外增加的衰减（平行光没有衰减）
		   	   fixed atten =1.0;
		   	   //计算阴影值
		   	   fixed shadow = SHADOW_ATTENUATION(i);
		   	   //返回，也就是输出
		   	   //不可以取消掉来自平行光的漫反射和高光，会使整体效果消失
		   	   return fixed4 (ambient +(specular + diffuse) * atten * shadow ,1.0 );
		   	}

			ENDCG
		}

		pass
		{
		  Tags {"LightMode" = "forwardAdd"}
		  Blend One One

		  CGPROGRAM
		    //后面的pass需要跟前面的叠加在一起，所以需要使用 multi_compile_fwdadd
		    #pragma multi_compile_fwdadd	
		  	#pragma vertex vert
			#pragma fragment frag
	
		    #include "Lighting.cginc"
		    //需要新加入的头文件，否则下面的unity_WorldToLight无法使用
		    #include "AutoLight.cginc"

		   	fixed4 _Diffuse;
		   	fixed4 _Specular;
		   	float _Gloss;


		   	struct a2v
		   	{
		   	   float4 vertex : POSITION;
		   	   float3 normal : NORMAL;
		   	};

		   	struct v2f
		   	{
		   	   float4 position : SV_POSITION;
		   	   float3 worldNormal : TEXCOORD0;
		   	   float3 worldPosition : TEXCOORD1;
		   	};


		   	v2f vert (a2v v)
		   	{
		   	    v2f o;
		   	    o.position = UnityObjectToClipPos( v.vertex);
		   	    o.worldNormal = UnityObjectToWorldNormal(v.normal);
		   	    o.worldPosition = mul(unity_ObjectToWorld , v.vertex) .xyz;
		   	    return o;
		   	}


		   	fixed4 frag(v2f i) : SV_Target
		   	{
		   	   fixed3 worldNormal = normalize (i.worldNormal);
		   	   fixed3 worldLightDir;
		   	   //在这里使用了if-else的结构，写法如下 （USING_DIRECTIONAL_LIGHT 这个pass处理平行光）
		   	   #ifdef USING_DIRECTIONAL_LIGHT
		   	    worldLightDir = normalize (_WorldSpaceLightPos0.xyz);
		   	   #else
		   	   //平行光无视相对位置，但是其他光源不是
		   	    worldLightDir = normalize (_WorldSpaceLightPos0.xyz - i.worldPosition.xyz );
		   	    #endif
		   	   //fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;（不用计算基础光照了，前面算过直接叠加）
		   	   fixed3 diffuse = _LightColor0 .rgb * _Diffuse.rgb * max (0,dot (worldNormal, worldLightDir));

			   fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPosition.xyz);
			   fixed3 halfDir = normalize(worldLightDir + viewDir);
		   	   fixed3 specular = _LightColor0.rgb *_Specular .rgb * pow(max (0, dot(worldNormal , halfDir)), _Gloss);
		   	   fixed atten ;
		   	   //非平行光的光源需要计算衰减
		   	   #ifdef USING_DIRECTIONAL_LIGHT
		   	    atten = 1.0;
		   	   #else
		   	   //现在只能说明这个衰减的方式是在这里自行编写的，但这段计算的过程现在还不够明朗
		   	   //获取光源空间中的相应位置
		   	    float3 lightCoord = mul( unity_WorldToLight ,  float4 (i.worldPosition , 1) ).xyz;
		   	    //用查找表做的衰减计算，纹理采样（这个纹理是引擎维护的）
		   	    atten = tex2D (_LightTexture0 , dot (lightCoord , lightCoord).rr ).UNITY_ATTEN_CHANNEL;
		   	   #endif 
		   	   //因为是叠加效果返回额外的漫反射个高光就可以
		   	   return fixed4 ((specular + diffuse) * atten ,1.0 );
		   	}

			ENDCG
		}


		Pass
		{
		  Tags {"LightMode" = "ShadowCaster" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            struct v2f 
            {
              V2F_SHADOW_CASTER;
            };

            v2f vert (appdata_base v)
            {
              v2f o;
              TRANSFER_SHADOW_CASTER_NORMALOFFSET(o);
              return o;
            }

            float4 frag (v2f i): SV_Target
            {
              SHADOW_CASTER_FRAGMENT(i);
            }

			ENDCG
		}

	}

//	FallBack "Standard"
}
