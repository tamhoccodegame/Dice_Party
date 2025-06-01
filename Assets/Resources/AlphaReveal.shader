Shader "Unlit/AlphaReveal"
{
        Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Fade ("Fade", Range(0,1)) = 0
        _Direction ("Fade Direction (0=Center, 1=L2R, 2=R2L)", Float) = 0
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Fade;
            float _Direction;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                float mask = 1.0;

                if (_Direction == 0)
                {
                    float center = 0.5;
                    float dist = abs(i.uv.x - center);
                    mask = smoothstep(_Fade, _Fade - 0.1, dist);
                }
                else if (_Direction == 1)
                {
                    mask = smoothstep(_Fade, _Fade - 0.1, i.uv.x);
                }
                else if (_Direction == 2)
                {
                    mask = smoothstep(_Fade, _Fade - 0.1, 1.0 - i.uv.x);
                }

                col.a *= mask;
                return col;
            }
            ENDCG
        }
    }
}
