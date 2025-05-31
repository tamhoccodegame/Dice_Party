Shader "Unlit/AlphaReveal"
{
        Properties
    {
        [HideInInspector]_MainTex("Font Atlas", 2D) = "white" {}
        _FaceColor("Face Color", Color) = (1,1,1,1)
        _FadeX("Fade Progress", Range(0,1)) = 0
        _FadeDirection("Fade Direction", Float) = 0 // 0 = Center, 1 = LeftToRight, 2 = RightToLeft
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _FaceColor;
            float _FadeX;
            float _FadeDirection;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _FaceColor;

                float mask = 1;

                if (_FadeDirection == 0) // Center
                {
                    float dist = abs(i.uv.x - 0.5);
                    mask = smoothstep(_FadeX * 0.5, 0.0, dist);
                }
                else if (_FadeDirection == 1) // Left to Right
                {
                    mask = smoothstep(_FadeX, i.uv.x, i.uv.x);
                }
                else if (_FadeDirection == 2) // Right to Left
                {
                    mask = smoothstep(_FadeX, 1.0 - i.uv.x, 1.0 - i.uv.x);
                }

                col.a *= mask;
                return col;
            }
            ENDCG
        }
    }
}
