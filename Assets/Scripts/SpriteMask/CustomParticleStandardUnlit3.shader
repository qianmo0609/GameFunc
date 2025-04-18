// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Particles/CustomParticleStandardUnlit3"
{
    Properties {
        _MainTex("Particle Texture", 2D) = "white" {}
        _InvFade("Soft Particles Factor", Range(0.01,3.0)) = 1.0
        //-------------------add----------------------
        _Rect("Rect",Vector) = (-20,20,-20,20)
        //-------------------add----------------------
    }

    Category {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend One OneMinusSrcAlpha
        ColorMask RGB
        Cull Off Lighting Off ZWrite Off

        SubShader {
            Pass {

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_particles

                #include "UnityCG.cginc"

                sampler2D _MainTex;
                fixed4 _TintColor;

                struct appdata_t {
                    float4 vertex : POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f {
                    float4 vertex : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                    #ifdef SOFTPARTICLES_ON
                    float4 projPos : TEXCOORD1;
                    #endif
                    //-------------------add----------------------
                    float3 posWs : TEXCOORD2;
                    //-------------------add----------------------
                };

                float4 _MainTex_ST;
                //-------------------add----------------------
                float4 _Rect;
                //-------------------add----------------------
                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    #ifdef SOFTPARTICLES_ON
                    o.projPos = ComputeScreenPos(o.vertex);
                    COMPUTE_EYEDEPTH(o.projPos.z);
                    #endif
                    //-------------------add----------------------
                    o.posWs = mul(unity_ObjectToWorld, v.vertex).xyz;
                    //-------------------add----------------------
                    o.color = v.color;
                    o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                    return o;
                }

                sampler2D_float _CameraDepthTexture;
                float _InvFade;

                fixed4 frag(v2f i) : SV_Target
                {
                    #ifdef SOFTPARTICLES_ON
                    float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ
                        (_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                    float partZ = i.projPos.z;
                    float fade = saturate(_InvFade * (sceneZ-partZ));
                    i.color.a *= fade;
                    #endif

                    fixed4 col =  i.color * tex2D(_MainTex, i.texcoord) * i.color.a;
                    //-------------------add----------------------
                    col.a *= step(_Rect.x,i.posWs.x) * step(i.posWs.x,_Rect.y);
                    col.a *= step(_Rect.z,i.posWs.y) * step(i.posWs.y,_Rect.w);
                    col.rgb *= col.a;
                    //-------------------add----------------------
                    return col;
                }
                ENDCG
            }
        }
    }
}
