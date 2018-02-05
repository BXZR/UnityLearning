// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sucks/glass"
{
	Properties
	{

	   //主纹理
       _MainTex("MainTexture" , 2D) = "white"{}
       //法线纹理
       _BumpMap("BumpMap" , 2D) = "bump"{}
       //立方体纹理，用来获取反射和折射的颜色
       _CubeMap("cubeMap" , cube) = "_Skybox"{}
       //扭曲，折射的时候的扭曲（偏移程度）
       _Distortion ("Distortion" , range(0,100)) = 10
       //菲涅尔的反射和折射的百分比（看下面代码就明白了）
       _RefracAmount ("RefractionAmount" , range(0.0,1.0)) = 1.0
	}
	SubShader
	{
	  Tags {"Queue" = "Transparent"  "RenderType" = "Opaque"}
	  GrabPass { "_RefractionTex" }
	  //"_RefractionTex"作为一个字符串被赋值使用，这个字符串是一个pass的texture的名字
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

				sampler2D _MainTex;//2D的贴图
				float4 _MainTex_ST;//偏移量矩阵
				sampler2D _BumpMap;//法线贴图
				float4 _BumpMap_ST;//法线贴图的偏移矩阵

				samplerCUBE _CubeMap;//立方体纹理

				float _Distortion;//折射扭曲
				fixed _RefracAmount;//菲涅尔反射和折射的多少

				//下面这两个应该是特殊的渲染通道GrabPass需要的内容（暂定）
				//获取摄像机渲染的内容并可以被所有的pass访问，达到高效的复用性
				//缺点:所有的物物体都会使用同一个图像，因为摄像机内容抓取只会使用一次
				//优化和常用实现手段
				//非优化手段就是不指定"_RefractionTex" 直接访问_GrabTexture(内置)
				sampler2D _RefractionTex;
				//可以获取贴图的大小
				float4 _RefractionTex_TexelSize;

				struct a2v 
				{
				   float4 vertex : POSITION; //套路，获取模型坐标
				   float3 normal : NORMAL; //套路，获取模型法线
				   float4 tangent : TANGENT;//获取引擎中的切线方向
				   float3 texcoord : TEXCOORD0;//获取贴图信息（实际上是将这个信息放到寄存器0里面）
				};

				struct v2f 
				{
				   float4 pos : SV_POSITION;//套路，生命目标坐标
				   float4 scrPos : TEXCOORD0;//屏幕图像采样坐标
				   float4 uv : TEXCOORD1; //图像的UV记录信息
				   //从切线空间到世界空间的变换矩阵，每一行分别存到一个寄存器里面
				   float4 TtoW0 : TEXCOORD2;
				   float4 TtoW1 : TEXCOORD3;
				   float4 TtoW2 : TEXCOORD4;
				};


				v2f vert (a2v v)
				{
				   v2f o;
				   //套路：模型坐标转剪裁坐标
				   o.pos = UnityObjectToClipPos (v.vertex);
				   //获取摄像机的抓屏图片信息
				   o.scrPos = ComputeGrabScreenPos(o.pos);
				   //记录主要贴图的点的坐标
				   o.uv .xy = TRANSFORM_TEX(v.texcoord , _MainTex);
				   //记录法线贴图的点的坐标
				   o.uv .zw = TRANSFORM_TEX(v.texcoord , _BumpMap);
				   //世界坐标获取
				   float3 worldPos = mul(unity_ObjectToWorld , v.vertex).xyz;
				   //API 获取世界法线坐标
				   fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				   //获取世界坐标中的切线方向
				   fixed3 worldTangent = UnityObjectToWorldDir (v.tangent.xyz);
				   //获取世界坐标中的顶点法线方向
				   fixed3 worldBinormal = cross (worldNormal ,worldTangent)* v.tangent.w;
				   //用上面的两个值计算真正的世界法线法相
				   //记录切线空间到世界空间的转换矩阵
				   o.TtoW0 = float4(worldTangent.x , worldBinormal.x , worldNormal.x , worldPos.x);
				   o.TtoW1 = float4(worldTangent.y , worldBinormal.y , worldNormal.y , worldPos.y);
				   o.TtoW2 = float4(worldTangent.z , worldBinormal.z , worldNormal.z , worldPos.z);

				   return o;

				}

			
				fixed4 frag(v2f i) : SV_Target
				{
				   //根据传过来的格式获取世界坐标
				   float3 worldPos = float3 (i.TtoW0.w , i.TtoW1.w, i.TtoW2.w );
				   //根据世界坐标获取到世界观察坐标（API）
				   fixed3 worldViewDir = normalize (UnityWorldSpaceViewDir(worldPos));
				   //法线纹理采样,似乎是一个向量
				   fixed3 bump = UnpackNormal (tex2D (_BumpMap , i.uv.zw));
				   //计算法线贴图的点的偏移量
				   float2 offset = bump .xy * _Distortion * _RefractionTex_TexelSize.xy;
				   //反过来给抓屏信息（一张图的XY信息做赋值）
				   i.scrPos.xy = offset * i.scrPos.z + i.scrPos.y;

				   //折射颜色计算
				   fixed3 refrCol = tex2D (_RefractionTex , i.scrPos.xy / i.scrPos.w).rgb;
				   //法线纹理重新更新
				   bump = normalize (half3 (dot(i.TtoW0.xyz,bump),dot(i.TtoW1.xyz,bump), dot(i.TtoW2.xyz,bump)));

				   //反射方向计算
				   fixed3 reflDir = reflect (-worldViewDir , bump);
				   //基础纹理的颜色（个人理解）
				   fixed4 texColor = tex2D(_MainTex,i.uv.xy);
				   //计算出反射的颜色（空间纹理颜色采样，基础颜色加成）
				   fixed3 reflCol = texCUBE (_CubeMap , reflDir).rgb * texColor .rgb;
				   //最终返回颜色计算，菲涅耳反射+折射
				   fixed3 finalColor = reflCol *(1- _RefracAmount) + refrCol * _RefracAmount;
				   return fixed4 (finalColor,1.0);
				}

			ENDCG
		}
	}
}
