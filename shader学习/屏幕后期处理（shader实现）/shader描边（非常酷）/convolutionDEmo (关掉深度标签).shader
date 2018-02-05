// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "suckScreen/convolutionDEmo"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_EdgeOnly ("Edge Only", Float) = 1.0
		_EdgeColor ("Edge Color", Color) = (0, 0, 0, 1)
		_BackgroundColor ("Background Color", Color) = (1, 1, 1, 1)
	  
	}
	SubShader
	{
		Pass
		{
          //  ZTest Always Cull Off ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			 
			#include "UnityCG.cginc"


			sampler2D _MainTex ;
			half4 _MainTex_TexelSize;//注意，这里记录的不是偏移量ST
			fixed  _EdgeOnly ; 
			fixed4 _EdgeColor ;
			fixed4 _BackgroundColor;

			//似乎不需要a2v的变换，根据面向过程的思想，没有必要写了


			struct v2f
			{
			    float4 pos : SV_POSITION;
			    half2 uv[9] : TEXCOORD0;
			};
 
			v2f vert( appdata_img v) 
			{
			  v2f o;
			  o.pos = UnityObjectToClipPos( v.vertex);

			  half2 uv = v.texcoord;

			  //这是一个纹理数组，对应使用Sobel方法进行卷积的时候需要的9个领域纹理坐标
			  //对应一个像素，求取这个像素的九宫格邻域，正好对应下面要给出的九宫格的卷积核
			  //也正因为如此，这个方法其实仅仅是一个例子，具体使用中有待优化和斟酌

			  //邻域坐标
			  o.uv[0] = uv + _MainTex_TexelSize.xy * half2 (-1,-1);
			  o.uv[1] = uv + _MainTex_TexelSize.xy * half2 (0,-1);
			  o.uv[2] = uv + _MainTex_TexelSize.xy * half2 (1,-1);
			  o.uv[3] = uv + _MainTex_TexelSize.xy * half2 (-1,0);
			  o.uv[4] = uv + _MainTex_TexelSize.xy * half2 (0,0);
			  o.uv[5] = uv + _MainTex_TexelSize.xy * half2 (1,0);
			  o.uv[6] = uv + _MainTex_TexelSize.xy * half2 (-1,1);
			  o.uv[7] = uv + _MainTex_TexelSize.xy * half2 (0,1);
			  o.uv[8] = uv + _MainTex_TexelSize.xy * half2 (1,1);

			   return o;
			}

				//返回饱和度颜色加权值
			fixed lum (fixed4 theColor)
			{
			  return  0.2125*theColor .r + 0.7154 *theColor.g + 0.0721*theColor.b;
			}


			half Sobel(v2f i)
			{
			  //横向卷积核
			  const half Gy[9] = {-1,-2,-1,
			                       0, 0, 0,
			                       1, 2, 1
			                     };
			  //纵向卷积核
			  const half Gx[9] = {-1, 0, 1,
			                      -2, 0, 2,
			                      -1, 0, 1
			                     };

			  //卷积过程，这个月卷积神经网络的卷积方法是一样的

			  half colorUse;//提取到的颜色饱和度加成值
			  half X;//横向卷积结果
			  half Y;//纵向卷积结果

			  for(int j ; j<9 ;j++)
			  {
			     colorUse = lum(tex2D (_MainTex , i.uv[j]));//提取图片上邻域的颜色
			     X+= colorUse*Gx[j];
			     Y+= colorUse*Gy[j];
			  }
			  half edge = 1- abs(X) - abs(Y);//用简化的求梯度的方法来计算梯度
			  return edge;

			}

 
		

			//用这个方法来输出到屏幕上，也就是SV_Targht
		    //这种方法似乎还是很灵活的

			fixed4 frag(v2f i) : SV_Target
			{
			    half edge = Sobel(i);

			    fixed4 withEdgeColor = lerp (_EdgeColor , tex2D (_MainTex , i.uv[4]) , edge);
			    fixed4 onlyEdgeColor = lerp (_EdgeColor , _BackgroundColor , edge);
			    return lerp (withEdgeColor , onlyEdgeColor , _EdgeOnly);
			}
 
 
			ENDCG
		}
	}
	FallBack Off//关掉递归查找（很节约资源）
}
