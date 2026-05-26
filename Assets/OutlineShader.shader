Shader "UI/Outline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", Float) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float _OutlineWidth;
 
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float w = _OutlineWidth * _MainTex_TexelSize.x;
float h = _OutlineWidth * _MainTex_TexelSize.y;

                float a = col.a;
                a = max(a, tex2D(_MainTex, i.uv + float2(w, 0)).a);
                a = max(a, tex2D(_MainTex, i.uv - float2(w, 0)).a);
                a = max(a, tex2D(_MainTex, i.uv + float2(0, h)).a);
                a = max(a, tex2D(_MainTex, i.uv - float2(0, h)).a);
                a = max(a, tex2D(_MainTex, i.uv + float2(w, h)).a);
                a = max(a, tex2D(_MainTex, i.uv - float2(w, h)).a);
                a = max(a, tex2D(_MainTex, i.uv + float2(-w, h)).a);
                a = max(a, tex2D(_MainTex, i.uv - float2(-w, h)).a);

                fixed4 outline = _OutlineColor * i.color;
                outline.a *= (a - col.a);

                return col + outline * (1 - col.a);
            }
            ENDCG
        }
    }
}