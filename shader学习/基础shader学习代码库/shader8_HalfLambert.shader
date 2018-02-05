
Shader "Suck/shader7_HalfLambert"
{
  //这个shader实现的是漫反射效果--逐顶点漫反射
  //为了消除漫反射背面3D效果缺失的问题，这个例子使用了半兰伯特模型做了修改
    Properties
    {
       _Diffuse ("Diffuse", Color) = (0.2,1,1,1)
    }
	SubShader
	{
	  cull off
		pass
		{
		   Tags {"LightMode" = "ForwardBAse"}
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
		         fixed3 color : COLOR;
		     };

		     //顶点着色器计算方法，这个是#pragma  vertex vert标记的
		     //输入是a2v v,将值输出给v2f 
		     //其实个惹别人为值的管理Unity内部已经做了
		     v2f verts(a2v v)
		     {
		        v2f o;//用于返回的对象，毕竟返回值是v2f类型
		        //做矩阵变换UNITY_MATRIX_MVP
		        o.pos = UnityObjectToClipPos(v.vertex);
		        //通过UNITY_LIGHTMODEL_AMBIENT获取Unity里面的全局光照信息
		        fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
		        //私有坐标转换成为世界坐标 
		        //v.normal 顶点法线 
		        //mul顶点变换方法
		        //_World2Object模型空间到世界空间的逆矩阵
		        //float3x3是一个切分，切出来三行三列转成float3类型
		        fixed3 worldNormal = normalize (mul (v.normal, (float3x3)unity_WorldToObject));
		        //获取世界光照方向
		        fixed3 worldLight = normalize (_WorldSpaceLightPos0.xyz);
		        //计算反射颜色
		        //_LightColor0 默认参数，给出当前的光照强度和颜色
		        //_Diffuse 这个是自己定义的颜色变量
		        //saturate方法，限制非负数，取 0和里面计算结果中比较大的值
		        //dot 点积
		        //这个小算法的计算计算公式 Cdiffuse = (Clight 。 Mdiffuse) max (0,N^ 。 L^);
		        //其中 Cdiffuse为结果 Clight是光照强度和颜色 Mdiffuse是反射系数 N^是表面法线 L^是指向光源的单位矢量
		        fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb *  dot (worldNormal,worldLight) * 0.5 +0.5;//这里就是半兰伯特模型的小修改
		        //计算返回值
		        o.color = ambient + diffuse;
		        return o;
		     }
		     //片元着色器调用方法，#pragma  fragment frag设定的
		     //SV_Target 返回的片面，接收返回的fixed4 
		     fixed4 frags (v2f c) :SV_Target
		     {
		      //返回片面颜色
		       return fixed4(c.color,1.0);
		     }

		   ENDCG
		}
	}
	FallBack "Standard"
}
