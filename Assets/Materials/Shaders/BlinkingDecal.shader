Shader "Custom/URP/BlinkingDecal"
{
    Properties
    {
        // 点滅する2色
        _ColorA ("Color A (Yellow)", Color) = (1.0, 0.9, 0.0, 1.0)
        _ColorB ("Color B (Red)",    Color) = (1.0, 0.1, 0.0, 1.0)

        // 点滅速度（Hz）
        _BlinkSpeed ("Blink Speed", Float) = 2.0

        // なめらかに切り替えるか (0=パキっと / 1=グラデーション)
        _SmoothBlink ("Smooth Blink (0 or 1)", Range(0,1)) = 0.0

        // デカール全体の不透明度
        _Opacity ("Opacity", Range(0, 1)) = 0.25

        // メインテクスチャ（省略可：白テクスチャで全面塗り）
        _MainTex ("Mask Texture (optional)", 2D) = "white" {}

        // URP Decal 内部プロパティ（触らない）
        [HideInInspector] _DecalColorScaleAndBias ("DecalColorScaleAndBias", Vector) = (1, 1, 1, 0)
        [HideInInspector] _DecalNormalBlend ("DecalNormalBlend", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType"     = "Decal"
            "Queue"          = "Transparent"
        }

        // ---- Pass 1: DBuffer (シーン色に書き込むデカール標準パス) ----
        Pass
        {
            Name "DBufferProjector"
            Tags { "LightMode" = "DBufferProjector" }

            // デカールの標準ブレンド設定
            Blend 0 SrcAlpha OneMinusSrcAlpha
            Blend 1 SrcAlpha OneMinusSrcAlpha
            Blend 2 SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual
            Cull Back

            HLSLPROGRAM
            #pragma vertex   Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing

            // URP コアインクルード
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            // ---- 頂点入力 ----
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            // ---- 頂点→フラグメント ----
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 positionSS  : TEXCOORD1; // スクリーンスペース座標
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // ---- CBUFFER ----
            CBUFFER_START(UnityPerMaterial)
                float4 _ColorA;
                float4 _ColorB;
                float  _BlinkSpeed;
                float  _SmoothBlink;
                float  _Opacity;
                float4 _MainTex_ST;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // ---- Vertex ----
            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv          = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.positionSS  = ComputeScreenPos(OUT.positionHCS);
                return OUT;
            }

            // ---- Fragment ----
            // DBuffer は MRT(複数レンダーターゲット) に書き込む
            struct FragOutput
            {
                half4 GBuffer0 : SV_Target0; // albedo + smoothness
                half4 GBuffer1 : SV_Target1; // metal + ao
                half4 GBuffer2 : SV_Target2; // normal
            };

            FragOutput Frag(Varyings IN)
            {
                // --- 点滅カラー計算 ---
                float t = _Time.y * _BlinkSpeed * 6.2831853; // 2π掛けてsin波に
                float blend;
                if (_SmoothBlink > 0.5)
                    blend = saturate(sin(t) * 0.5 + 0.5); // なめらか
                else
                    blend = step(0.5, frac(_Time.y * _BlinkSpeed)); // パキっと

                half4 blinkColor = lerp(_ColorA, _ColorB, blend);

                // --- マスクテクスチャ ---
                half4 mask = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half  finalAlpha = mask.a * blinkColor.a * _Opacity;

                // --- MRT 出力 ---
                FragOutput o;
                o.GBuffer0 = half4(blinkColor.rgb, finalAlpha); // albedo
                o.GBuffer1 = half4(0, 0, 0, finalAlpha);        // metallic=0
                o.GBuffer2 = half4(0.5, 0.5, 1.0, finalAlpha);  // normal (up)
                return o;
            }
            ENDHLSL
        }

        // ---- Pass 2: DecalProjectorPreview (シーンビュー用プレビュー) ----
        Pass
        {
            Name "DecalProjectorPreview"
            Tags { "LightMode" = "DecalProjectorPreview" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest Always
            Cull Back

            HLSLPROGRAM
            #pragma vertex   Vert
            #pragma fragment FragPreview

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings   { float4 positionHCS : SV_POSITION; float2 uv : TEXCOORD1; };

            CBUFFER_START(UnityPerMaterial)
                float4 _ColorA;
                float4 _ColorB;
                float  _BlinkSpeed;
                float  _SmoothBlink;
                float  _Opacity;
                float4 _MainTex_ST;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv          = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 FragPreview(Varyings IN) : SV_Target
            {
                float blend;
                if (_SmoothBlink > 0.5)
                    blend = saturate(sin(_Time.y * _BlinkSpeed * 6.2831853) * 0.5 + 0.5);
                else
                    blend = step(0.5, frac(_Time.y * _BlinkSpeed));

                half4 blinkColor = lerp(_ColorA, _ColorB, blend);
                half4 mask = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                blinkColor.a = mask.a * blinkColor.a * _Opacity;
                return blinkColor;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
