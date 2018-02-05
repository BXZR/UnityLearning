Shader "Suck/shader4"
{

   Properties 
   {
      //声明变量 color类型
      _COLOR ("ColorUse",Color) = (1.0, 1.0, 1.0, 1.0)
   }
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex verts//声明顶点着色器的方法代码
			#pragma fragment frag//声明片元着色器的方法代码

			//需要定义一个与属性名称和类型都匹配的变量
			fixed4 _COLOR;

			struct A2v
			{
			    //用模型空间的顶点坐标填充vertex变量
				float4 vertex : POSITION;
				//用模型空间的发现方向填充normal变量
				float3 normal : NORMAL;
				//用模型的第一套纹理坐标填充texcoord变量
				float4 texcoord : TEXCOORD0;
			};

			struct V2f
			{
			    // SV_POSITION赋值pos,包含顶点在剪裁空间的位置信息
				float4 pos : SV_POSITION;
				// COLOR0用于存储颜色信息
				fixed3 colors : COLOR0;
			};

			V2f verts(A2v v) 
			{
			    //声明输出结构
				V2f o;
				//这个与书上的代码是不一致的，版本更新之后，这个代码会自动规正成这样
				//原来的代码  mul(UNITY_MATRIX_MVP,v.vertex);
			 	o.pos = UnityObjectToClipPos(v.vertex);
			 	//v.normal *0.5似乎是额外颜色的亮度
			 	//fixed3(0.5,0.5,0.5)是基础颜色

			 	//v.normal里面包含顶点法线方向，分量范围[-1.0,1.0]
			 	//下面这句话将分量范围映射到[0.0,1.0]
			 	//将信息存到color属性里面传给片元着色器
			 	o.colors = fixed3(0.5,0.5,0.5) +v.normal *0.2 ;
			    return o;
			}

			//SV_Target SV含义为系统数值，这个是directX 10以后的新的语义模型
			fixed4 frag(V2f i) : SV_Target
			{
			  fixed3 c =  i.colors;
			  c *= _COLOR.rgb;
			  return fixed4(c,1.0);
			}
 
			ENDCG
		}
	}
}
