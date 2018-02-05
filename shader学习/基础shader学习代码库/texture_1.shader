// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Suck/texture_1"
{
//纹理lv1
//个人认为反射率 albedo 的计算最重要

	Properties
	{
	    //_Color控制整体的色调
		_Color ("Color use", color) = (1,1,1,1)
		//"white" 内置纹理的名字
		_MainTex("MainTex", 2D) = "white" {}
		//高光颜色
		_Specular ("Specular", Color) = (1,1,1,1)
		//高光范围
		_Gloss ("Gloss",Range(8.0,256)) = 20
	}
	SubShader
	{
		 
		Pass
		{
		   tags {"LightMode" = "ForwardBase"}
		   cull off
			CGPROGRAM
			//用于设定编译的时候用于不同着色器的方法
			#pragma vertex vert
			#pragma fragment frag
            #include "Lighting.cginc"

            //设置变量，要求为名称和类型都需要和属性设定相同
            fixed4 _Color;
            sampler2D _MainTex;
            //Unity规定的额外信息记录结构，命名规则为 纹理名_ST
            //其中_ST表示scale和transform
            //其中，XX_ST.xy缩放  XX_ST.zw偏移
            float4 _MainTex_ST;
            fixed4 _Specular;
            float _Gloss;

            //输入结构体
            struct a2v 
            {
              float4 vertex : POSITION;//顶点位置
              float3 normal : NORMAL;//顶点法线
              float4 texcoord : TEXCOORD0;//保留第一组纹理的信息
            };
            //输出结构体
            struct v2f
            {
                float4 pos : SV_POSITION;//输出目标位置
                float3 worldNormal : TEXCOORD0;//第一张纹理信息
                float3 worldPos : TEXCOORD1;//第二张纹理信息
                float2 uv : TEXCOORD2;//记录纹理采样的信息
            };

            //第一步的方法，顶点着色器计算“插口”
            v2f vert(a2v v)
            {
               v2f o;
               o.pos = UnityObjectToClipPos (v.vertex);//坐标转换
               o.worldNormal = UnityObjectToWorldNormal(v.normal);//世界法线
               o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;//世界坐标
               o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;//记录偏移量
               return o;//返回“内部调用”
            }

            //第二步方法，片元着色器
            fixed4 frag(v2f i) : SV_Target//将计算结果输出到 SV_Target也就是屏幕缓存
            {
                 fixed3 worldNormal = normalize (i.worldNormal);//世界坐标下的法线坐标
                 fixed3 worldLightDir = normalize (UnityWorldSpaceLightDir(i.worldPos));//世界坐标下的光照方向
                 //tex2D (_MainTex, i.uv) 参数 纹理，纹理坐标  返回计算得到的纹素值
                 //这一步是最为关键的，计算的反射率用来显示纹理
                 fixed3 albedo = tex2D (_MainTex, i.uv).rgb * _Color.rgb;
                 // ambient 基本光照  UNITY_LIGHTMODEL_AMBIENT.xyz 系统内部直接调用的环境光
                 fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;//也就是说，其实我们看到的纹理都是光照得到的。用模拟自然的方式实现

                 //使用兰伯特光照模型做漫反射，中间加上了反射率作为加成
                 fixed3 diffuse = _LightColor0 .rgb * albedo *max (0,dot(worldNormal , worldLightDir));

                 //使用Blinn-Phong模型做高光反射模型
                 fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                 fixed3 halfDir = normalize (worldLightDir + viewDir);
                 //pow函数式指数方法 pow(x,y);x的y次方
                 fixed specular = _LightColor0.rgb * _Specular.rgb* albedo * pow (max(0,dot(worldNormal , halfDir)), _Gloss);

                 //最后将这三个矩阵相加来实现颜色的整合
                 return fixed4 (ambient + diffuse + specular , 1.0);
            }

			ENDCG
		}
	}


	FallBack "Standard"
}
