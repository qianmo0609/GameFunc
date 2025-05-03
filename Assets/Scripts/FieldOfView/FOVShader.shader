Shader "Custom/FOVShader" {
    Properties {
        _MainColor ("Main Color", Color) = (0, 0.5, 1, 0.3)
        _EdgeColor ("Edge Color", Color) = (0, 1, 1, 1)
        _EdgeWidth ("Edge Width", Range(0,1)) = 0.1
    }

    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 _MainColor;
            fixed4 _EdgeColor;
            float _EdgeWidth;

            fixed4 frag(v2f i) : SV_Target {
                // 计算边缘强度
                float edge = smoothstep(0.8 - _EdgeWidth, 0.8, 1 - length(i.uv));
                fixed4 col = lerp(_MainColor, _EdgeColor, edge);
                col.a = _MainColor.a * (1 - edge) + edge;
                return col;
            }
            ENDCG
        }
    }
}
