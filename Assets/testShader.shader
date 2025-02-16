Shader "Custom/GasEffect" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Range(0.1, 10)) = 1
        _NoiseSpeed ("Noise Speed", Range(0.1, 10)) = 1
        _Color1 ("Color 1", Color) = (0.2, 0.2, 0.8, 1)
        _Color2 ("Color 2", Color) = (0.8, 0.8, 0.2, 1)
        _BlendFactor ("Blend Factor", Range(0, 1)) = 0.5
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _NoiseScale;
            float _NoiseSpeed;
            fixed4 _Color1;
            fixed4 _Color2;
            float _BlendFactor;
            
            int floorToInt(float x){
                return int(floor(x));
            }

            // 简单的 Perlin 噪声函数（简化版）
            float fade(float t) { return t * t * t * (t * (t * 6 - 15) + 10); }
            float lerp(float a, float b, float t) { return a + t * (b - a); }
            float grad(int hash, float x, float y) {
                int h = hash & 3;
                float u = h < 2 ? x : y;
                float v = h < 2 ? y : x;
                return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
            }
            float perlinNoise(float2 P) {
                int ix0 = floorToInt(P.x);
                int iy0 = floorToInt(P.y);
                float fx0 = frac(P.x);
                float fy0 = frac(P.y);
                float fx1 = fx0 - 1.0;
                float fy1 = fy0 - 1.0;
                ix0 = ix0 & 255;
                iy0 = iy0 & 255;
                int ix1 = (ix0 + 1) & 255;
                int iy1 = (iy0 + 1) & 255;

                int h0 = iy0 * 1619;
                int h1 = iy1 * 1619;
                int g00 = (h0 + ix0) * 31337;
                int g10 = (h0 + ix1) * 31337;
                int g01 = (h1 + ix0) * 31337;
                int g11 = (h1 + ix1) * 31337;

                float n00 = grad(g00, fx0, fy0);
                float n10 = grad(g10, fx1, fy0);
                float n01 = grad(g01, fx0, fy1);
                float n11 = grad(g11, fx1, fy1);

                float u = fade(fx0);
                float v = fade(fy0);

                return lerp(lerp(n00, n10, u), lerp(n01, n11, u), v);
            }

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 获取纹理颜色
                fixed4 col = tex2D(_MainTex, i.uv);

                // 添加时间变量实现流动效果
                float time = _Time.y * _NoiseSpeed;

                // 多层噪声叠加
                float noise1 = perlinNoise(i.uv * _NoiseScale + float2(time, time));
                float noise2 = perlinNoise(i.uv * _NoiseScale * 2 + float2(-time, time));
                float noise3 = perlinNoise(i.uv * _NoiseScale * 4 + float2(time, -time));
                float combinedNoise = (noise1 + noise2 + noise3) / 3;

                // 颜色过渡
                fixed4 finalColor = lerp(_Color1, _Color2, combinedNoise * _BlendFactor);

                // 混合纹理颜色和气体颜色
                col.rgb = lerp(col.rgb, finalColor.rgb, finalColor.a);
                col.a = finalColor.a;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
