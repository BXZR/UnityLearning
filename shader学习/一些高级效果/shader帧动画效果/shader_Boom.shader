// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sucks 2/shader Boom"
{
	Properties
	{
		//属性集合
		_Color("Color" , color) = (1,1,1,1)
		_MainTexture ("MainTexture", 2D) = "white"{}
		//下面这些属性是用来计算
		_HorizontalAmount ("Honrizontal Amount" , float) = 4
		_VerticalAmount ("Vertucal Amount" , float ) = 4
		_Speed ("speed" , Range (1,100)) = 30
	}
	SubShader
	{
	 Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}

		Pass
		{
		   Tags {"LightMode" = "ForwardBase"}
		   //关闭Z缓冲
		   ZWrite off
		   //开启透明度混合
		   Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert  
			#pragma fragment frag
			#include "UnityCG.cginc"

	        //属性对应的字段
	        fixed4 _Color;
	        sampler2D _MainTexture;
	        //因为用到了贴图，所以要用这个作为记录偏移量的变量
	        float4 _MainTexture_ST;
	        //与运动相关的变量
	        float _HorizontalAmount;
	        float _VerticalAmount;
	        float _Speed ;

	        struct a2v 
	        {
	          float4 vertex : POSITION;
	          //不用法线，所以没有写管宇法线的内容
	          float2 texcoord : TEXCOORD0;//因为保存的是图所以用TEXCOORD寄存器格式保存
	        };

			struct v2f 
			{  
			    //记录屏幕坐标和UV信息
			    float4 pos : SV_POSITION;
			    float2 uv : TEXCOORD0;
			};  

	        v2f vert(a2v v)
	        {
	           v2f o;
	           o.pos = UnityObjectToClipPos ( v.vertex);//坐标转换（模型到世界）
	           o.uv = TRANSFORM_TEX(v.texcoord , _MainTexture);//UV坐标和贴图坐标对应
	           return o;
	        }

	        fixed4 frag(v2f i) : SV_Target
	        {
	          //计算一个“时间流逝”
	          float timer = floor (_Time.y*_Speed);
	          float row = floor (timer / _HorizontalAmount);
	          float col = timer - row * _VerticalAmount;
	          //计算额外的偏移量
	          half2 uv = i.uv + half2(col , -row);
	          uv.x /= _HorizontalAmount;
	          uv.y /= _VerticalAmount;
              //直接获取2D贴图当做返回量
	          fixed4 c = tex2D (_MainTexture , uv);
	          c.rgb *= _Color.rgb;//基础颜色加成

	          return c;
	        }
			ENDCG
		}
	}
	FallBack "Transparent/VertexLit"
}
