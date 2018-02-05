// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SUCKSUCKS/OutLine1" 
{
    Properties
    {
        _MainTex("main tex", 2D) = ""{ }
        _OutLineColor("outline color",Color) = (0,0,0,1)//描边颜色
    }
 
    SubShader
    {
        //描边
        pass
        {
            Cull Front
            Offset -5,-1 //深度偏移
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            sampler2D _MainTex;
            half4 _OutLineColor;
 
            struct v2f
            {
                float4  pos : SV_POSITION;
            };
 
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
 
            float4 frag(v2f i) : COLOR
            {
                return _OutLineColor;
            }
            ENDCG
        }
 
        //正常渲染物体
        pass
        {
            //Cull Back
            //Offset 5,-1
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
 
            struct v2f
            {
                float4  pos : SV_POSITION;
                float2  uv : TEXCOORD0;
            };
 
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
                return o;
            }
 
            float4 frag(v2f i) : COLOR
            {
                float4 c = tex2D(_MainTex,i.uv);
                return c;
            }
            ENDCG
        }
    }
}