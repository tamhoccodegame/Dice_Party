Shader "Unlit/OutlineHover"
{
Properties {
        _Color ("Base Color", Color) = (1,1,1,1)
        _ShineColor ("Shine Color", Color) = (0,1,1,1)
        _Thickness ("Line Thickness", Float) = 4
        _ShineWidth ("Shine Width", Float) = 0.2
        _ShinePosition ("Shine Position", Range(0,1)) = 0
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _ShineColor;
            float _Thickness;
            float _ShinePosition;
            float _ShineWidth;

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float edgeDist(float2 uv) {
                float2 edgeDist = min(uv, 1 - uv);
                return min(edgeDist.x, edgeDist.y);
            }

            float outlineMask(float2 uv, float thickness) {
                return smoothstep(thickness, thickness + 0.01, edgeDist(uv));
            }

            float borderPos(float2 uv) {
                if (uv.y <= uv.x && uv.y <= 1 - uv.x) return uv.x; // Bottom
                if (uv.x >= 1 - uv.y && uv.x >= uv.y) return uv.y + 1; // Right
                if (uv.y >= uv.x && uv.y >= 1 - uv.x) return 3 - uv.x; // Top
                return 4 - uv.y; // Left
            }

            fixed4 frag (v2f i) : SV_Target {
                float mask = outlineMask(i.uv, _Thickness / _ScreenParams.y);
                if (mask <= 0.01) discard;

                float pos = borderPos(i.uv) / 4.0;
                float shine = smoothstep(_ShinePosition - _ShineWidth, _ShinePosition, pos) * 
                              (1 - smoothstep(_ShinePosition, _ShinePosition + _ShineWidth, pos));
                float4 col = _Color + _ShineColor * shine;

                col.a *= mask;
                return col;
            }
            ENDCG
        }
    }
}
