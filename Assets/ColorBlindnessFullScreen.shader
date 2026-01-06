Shader "Hidden/ColorBlindnessFullScreen"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Name "ColorBlindnessFullScreen"
            ZWrite Off
            ZTest Always
            Cull Off
            Blend Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _M0;
            float4 _M1;
            float4 _M2;
            float _Intensity; // 0..1

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            Varyings Vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            float3 ApplyMatrix(float3 c)
            {
                float3 r;
                r.x = dot(_M0.xyz, c);
                r.y = dot(_M1.xyz, c);
                r.z = dot(_M2.xyz, c);
                return r;
            }

            half4 Frag(Varyings i) : SV_Target
            {
                float3 src = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).rgb;
                float3 sim = saturate(ApplyMatrix(src));
                float3 outc = lerp(src, sim, saturate(_Intensity));
                return half4(outc, 1);
            }
            ENDHLSL
        }
    }
}