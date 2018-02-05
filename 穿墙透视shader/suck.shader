// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/suck" {  
    Properties     
    {    
        _Color("Color", Color) = (1,1,1,1)      
        _MainTex("Albedo", 2D) = "white" {}    
        _AfterTex("_AfterTex", 2D) = "white" {}    
        _AfterColor ("After Color", Color) = (0.435, 0.851, 1, 0.419)    
    }    
        
    SubShader     
    {    
        Tags { "RenderType"="transparent" }    
        LOD 300    
        // 源RGB*源A + 背景RGB*(1-源A)    
        Blend SrcAlpha OneMinusSrcAlpha     
        pass    
        {    
            Tags { "LightMode" = "Vertex" }  
            // 源RGB*1 + 背景RGB*(1-源RGB)    
            Blend One OneMinusSrcColor  
            Cull Off  
            Lighting Off  
            ZWrite Off  
            Ztest Greater  
  
            CGPROGRAM    
            #pragma vertex vert    
            #pragma fragment frag    
            #pragma fragmentoption ARB_precision_hint_fastest  
  
            sampler2D _AfterTex;    
            float4 _AfterColor;    
            struct appdata     
            {    
                float4 vertex : POSITION;  
                float3 normal : NORMAL;  
            };    
            struct v2f    
            {    
                float4 pos : POSITION;   
                float2 uv : TEXCOORD0;  
            };    
            v2f vert (appdata v)    
            {    
                v2f o;    
                o.pos = UnityObjectToClipPos(v.vertex);    
                half2 capCoord;  
                float3 nor = mul(UNITY_MATRIX_IT_MV, v.normal);    
                //capCoord.x = dot(UNITY_MATRIX_IT_MV[0].xyz,v.normal);  
                //capCoord.y = dot(UNITY_MATRIX_IT_MV[1].xyz,v.normal);  
                capCoord.x = nor.x;  
                capCoord.y = nor.y;  
                // 法线并不是真正定义在模型坐标系中，比如非等比缩放模型时  
                // 需要使用MV转换矩阵的逆转置矩阵来把法线转到视角坐标系中  
                // 将法线方向值域【-1，1】变化为贴图uv值域【0，1】     
                o.uv = capCoord * 0.5 + 0.5;  
                return o;    
            }    
            float4 frag (v2f i) : COLOR    
            {    
                float4 col = tex2D(_AfterTex, i.uv);  
                return col * 2.0 * _AfterColor;    
            }    
            ENDCG    
        }    
        pass    
        {    
            ZTest LEqual     
            CGPROGRAM  
            #pragma vertex vert    
            #pragma fragment frag    
            sampler2D _MainTex;    
            float4 _MainTex_ST;    
            struct appdata     
            {    
                float4 vertex : POSITION;    
                float4 texcoord : TEXCOORD0;    
            };    
            struct v2f    
            {    
                float4 pos : POSITION;    
                float4 uv : TEXCOORD0;    
            };    
            v2f vert (appdata v)    
            {    
                v2f o;    
                o.pos = UnityObjectToClipPos(v.vertex);    
                o.uv = v.texcoord;    
                return o;    
            }    
            float4 frag (v2f i) : COLOR    
            {    
                float4 texCol = tex2D(_MainTex, i.uv);    
                return texCol;    
            }    
            ENDCG    
        }    
    }    
}    