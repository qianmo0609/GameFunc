Shader "Custom/YAxisCloth" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Tension ("Tension", Range(0,1)) = 0.5
        _WindEffect ("Wind Effect", Range(0,2)) = 0.5
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard vertex:vert addshadow
        #pragma target 3.0

        sampler2D _MainTex;
        float _Tension;
        float _WindEffect;

        struct Input {
            float2 uv_MainTex;
            float3 worldNormal;
        };

        void vert(inout appdata_full v)
        {
            // 根据法线张力调整顶点颜色
            float3 normal = v.normal;
            float tension = length(normal) * _Tension;
            v.color = lerp(float4(1,1,1,1), float4(0.5,0.8,1,1), tension);
            
            // 添加风力偏移
            float wind = sin(_Time.y + v.vertex.x) * _WindEffect;
            v.vertex.xyz += float3(0,0,1) * wind * 0.1;
        }

        void surf (Input IN, inout SurfaceOutputStandard o) {
            half4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb * IN.worldNormal.y;
            o.Metallic = _Tension * 0.3;
            o.Smoothness = 0.5;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
