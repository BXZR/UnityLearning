// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Suck_Turn2/makeImpress"
{
	Properties
	{
		 _Texture1 ("Texture1" , 2D) = "white"{}
		 _Texture2 ("Texture2" , 2D) = "white"{}
		 _Texture3 ("Texture3" , 2D) = "white"{}
		 _Speed1 ("Speed1" , float) = 1.0
		 _Speed2 ("Speed2" , float) = 1.0
	}
	SubShader
	{
		cull off //取消剪裁
	 
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"//必须引用的头文件，否则无法记录UV
			//贴图信息
			sampler2D _Texture1;
			sampler2D _Texture2;
			sampler2D _Texture3;
			//必须加上的偏移量信息
			float4 _Texture1_ST;
			float4 _Texture2_ST;
			float4 _Texture3_ST;
			//速度
			float _Speed1;
			float _Speed2;

			struct a2v
			{
			 //最基础的就是模型的顶点信息，这个是必须要有的
			 float4 vertex : POSITION;
			 //记录寄存器1的数值
			 float4 texcoord :TEXCOORD0;
			};

            struct v2f
            {
               //获取CV坐标用来处理到片元
				float4 pos : SV_POSITION;
				//定义两个变量并分配寄存器，分别用来记录三张贴图的UV信息
				float4 uv : TEXCOORD0;
				float4 uv2 : TEXCOORD1;
            };

             v2f vert (a2v v)
             {
               v2f o;
               //最经典的定点信息变换
               o.pos = UnityObjectToClipPos( v.vertex);
               //frac返回的是小数部分
               //获取贴图的偏移量
               //带变换的地方可以使用时间变量加成
               o.uv.xy = TRANSFORM_TEX(v.texcoord, _Texture1) + frac(float2(_Speed1, 0.0) * _Time.y);
			   o.uv.zw = TRANSFORM_TEX(v.texcoord, _Texture2) + frac(float2(_Speed2, 0.0) * _Time.y);
			   o.uv2.xy = TRANSFORM_TEX(v.texcoord, _Texture3);
               return o;
             }

             fixed4 frag(v2f i) : SV_Target
             {
                //分别获取贴图的2D贴图信息
                fixed4 layer1 = tex2D (_Texture1 , i.uv.xy);
                fixed4 layer2 = tex2D (_Texture2 , i.uv.zw);
                fixed4 layer3 = tex2D (_Texture3 , i.uv2.xy);
                //插值法将这些贴图绘制在一起
                //参数内容： 源，目标，插值速度
                fixed4 shower = lerp (layer1 , layer2 , layer2.a);
                //这句插值很有意思，具体在于最终需要显示的在于shower之上，做以目标是shower
                //如果源和目标反过来效果不同！
                fixed4 showerFinal = lerp (layer3 , shower , layer3.a);
                //返回给屏幕
                return showerFinal;
             }
			ENDCG
		}
	}
}
