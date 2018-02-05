// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Suck/shader7DiffusePixelLevel"
{
  //这个shader实现的是漫反射效果--逐像素漫反射
    Properties
    {
       _Diffuse ("Diffuse", Color) = (0.2,1,1,1)
    }
	SubShader
	{
	    cull off
		pass
		{
		   Tags {"LightMode" = "ForwardBase"}
		   CGPROGRAM
		      //预编译命令，指定哪一个方法针对哪一件事情
		     #pragma vertex verts
		     #pragma fragment frags
		     //预编译命令，引入cginc文件，个人理解就是shader的外部引用方式
		     #include "Lighting.cginc"

		     fixed4 _Diffuse;//变量，需要跟前面的属性名字一样并且类型一致 

		     //通过Unity内置的字段来赋值一个结构体
		     //顶点着色器机选需要的信息
		     struct a2v
		     {
		       float4 vertex  : POSITION;
		       float3 normal  : NORMAL;
		     };
		     //片元着色器机选需要的信息
		     struct v2f
		     {
		        //SV_POSITION比较跨平台，效果与POSITION相同
		         float4 pos : SV_POSITION;
		         //逐像素的用这种方式
		         fixed3 worldNormal : TEXCOORD0;
		     };

		     //顶点着色器计算方法，这个是#pragma  vertex vert标记的
		     //输入是a2v v,将值输出给v2f 
		     //其实个惹别人为值的管理Unity内部已经做了
		     v2f verts(a2v v)
		     {
		        v2f o;//用于返回的对象，毕竟返回值是v2f类型
		        //做矩阵变换UNITY_MATRIX_MVP
		        o.pos = UnityObjectToClipPos(v.vertex);
                //计算饰界空间之下的发现坐标就可以了,存放在o.wordNormal里面
                o.worldNormal = mul ( v.normal , (float3x3)unity_WorldToObject);
		        return o;
		     }
		     //片元着色器调用方法，#pragma  fragment frag设定的
		     //SV_Target 返回的片面，接收返回的fixed4 
		     //逐像素漫反射在片元着色器计算的时候进行计算
		     fixed4 frags (v2f c) :SV_Target
		     {
		      //获取全局光照信息
		      fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
		      //世界坐标的法线
		      fixed3 worldNormal = normalize (c.worldNormal);
		      //世界坐标光照方向
		      fixed3 worldLightDir = normalize (_WorldSpaceLightPos0.xyz);
		      fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal ,worldLightDir));
		      fixed3 theColor = ambient + diffuse;
		      return fixed4(theColor,1.0);
		     }

		   ENDCG
		}
	}
	FallBack "Standard"
}
