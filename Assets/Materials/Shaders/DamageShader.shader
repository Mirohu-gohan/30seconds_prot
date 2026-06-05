Shader "Custom/DamageShader"
{
    Properties
    {
        _MainTex   ("テクスチャ 1",     2D)          = "white" {}
        _MainTex2  ("テクスチャ 2",     2D)          = "white" {}
        _NoiseTex  ("ノイズテクスチャ", 2D)          = "white" {}
        _Damage    ("ダメージ量",       Range(0,1))  = 0
        _BurnColor ("焦げ色",           Color)       = (0.1, 0.05, 0.0, 1)
        _VertNoise ("頂点ゆらぎ強度",   Float)       = 0.05
    }

    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);  SAMPLER(sampler_MainTex);
            TEXTURE2D(_MainTex2); SAMPLER(sampler_MainTex2);
            TEXTURE2D(_NoiseTex); SAMPLER(sampler_NoiseTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _MainTex2_ST;
                float4 _BurnColor;
                float  _Damage;
                float  _VertNoise;
            CBUFFER_END

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float2 uv2         : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                float noise = SAMPLE_TEXTURE2D_LOD(
                    _NoiseTex, sampler_NoiseTex, IN.uv * 3.0, 0).r;
                IN.positionOS.xyz += IN.normalOS * noise * _Damage * _VertNoise;

                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv  = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.uv2 = TRANSFORM_TEX(IN.uv, _MainTex2);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 col1  = SAMPLE_TEXTURE2D(_MainTex,  sampler_MainTex,  IN.uv);
                half4 col2  = SAMPLE_TEXTURE2D(_MainTex2, sampler_MainTex2, IN.uv2);
                float noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.uv).r;

                // 2枚のテクスチャをアルファで合成
                half4 col = lerp(col1, col2, col2.a);

                clip(noise - _Damage);

                float burnEdge = 1 - saturate((noise - _Damage) * 8.0);
                col.rgb = lerp(col.rgb, _BurnColor.rgb, burnEdge);

                return col;
            }
            ENDHLSL
        }
    }
}