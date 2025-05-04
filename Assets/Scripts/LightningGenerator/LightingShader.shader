Shader "Custom/BuiltinLightning" {
    Properties {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _Emission ("Emission Intensity", Range(0, 10)) = 5
        _NoiseScale ("Noise Scale", Range(0, 5)) = 1
        _Distortion ("Distortion", Range(0, 1)) = 0.3
        _Speed ("Animation Speed", Range(0, 5)) = 1
        _EdgeFeather ("Edge Feather", Range(0, 1)) = 0.2
    }

    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float3 worldPos : TEXCOORD1;
            };

            uniform fixed4 _MainColor;
            uniform float _Emission;
            uniform float _NoiseScale;
            uniform float _Distortion;
            uniform float _Speed;
            uniform float _EdgeFeather;

            // 简易噪声生成函数
            float noise(float2 uv) {
                return frac(sin(dot(uv, float2(12.9898,78.233))) * 43758.5453);
            }

            v2f vert (appdata v) {
                v2f o;
                
                // 动态顶点偏移
                float t = _Time.g * _Speed;
                float2 noiseUV = v.uv * _NoiseScale + t;
                float displacement = noise(noiseUV) * _Distortion;
                
                // 应用顶点动画
                float3 offset = normalize(v.vertex.xyz) * displacement * 0.1;
                v.vertex.xyz += offset;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // UV动画
                float t = _Time.g * _Speed;
                float2 scrollUV = i.uv * _NoiseScale + t;
                
                // 生成噪声
                float n = noise(scrollUV);
                
                // 边缘羽化处理
                float edge = smoothstep(0, _EdgeFeather, i.uv.x) * 
                           smoothstep(0, _EdgeFeather, 1 - i.uv.x);
                
                // 颜色合成
                fixed3 finalColor = _MainColor.rgb * (n + 0.5) * _Emission;
                float alpha = saturate(edge * (n + 0.2)) * _MainColor.a;
                
                // 混合顶点颜色
                finalColor *= i.color.rgb;
                alpha *= i.color.a;

                return fixed4(finalColor, alpha);
            }
            ENDCG
        }
    }
    CustomEditor "LightningShaderGUI"
}
