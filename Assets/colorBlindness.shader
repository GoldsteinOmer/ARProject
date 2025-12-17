Shader "Custom/ColorblindLinear"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [Enum(Normal,0,Deuteranopia,1,Protanopia,2,Tritanopia,3)] _Mode ("Mode", Float) = 0
        
        // UI Required
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _Mode;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.color = v.color * _Color;
                return OUT;
            }

            float3 ApplyColorblindness(float3 rgb, float mode)
            {
                float3 result = rgb;
                
                if (mode == 1) // Deuteranopia (Green-blind)
                {
                    // Simplified Deuteranopia
                    result.r = 0.367322 * rgb.r + 0.860646 * rgb.g + -0.227968 * rgb.b;
                    result.g = 0.280085 * rgb.r + 0.672501 * rgb.g + 0.047413 * rgb.b;
                    result.b = -0.011820 * rgb.r + 0.042940 * rgb.g + 0.968881 * rgb.b;
                }
                else if (mode == 2) // Protanopia (Red-blind)
                {
                    // Simplified Protanopia
                    result.r = 0.152286 * rgb.r + 1.052583 * rgb.g + -0.204868 * rgb.b;
                    result.g = 0.114503 * rgb.r + 0.786281 * rgb.g + 0.099216 * rgb.b;
                    result.b = -0.003882 * rgb.r + -0.048116 * rgb.g + 1.051998 * rgb.b;
                }
                else if (mode == 3) // Tritanopia (Blue-blind)
                {
                    // Simplified Tritanopia
                    result.r = 1.255528 * rgb.r + -0.076749 * rgb.g + -0.178779 * rgb.b;
                    result.g = -0.078411 * rgb.r + 0.930809 * rgb.g + 0.147602 * rgb.b;
                    result.b = 0.004733 * rgb.r + 0.691367 * rgb.g + 0.303900 * rgb.b;
                }
                
                return saturate(result);
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                // Apply colorblind simulation
                color.rgb = ApplyColorblindness(color.rgb, _Mode);

                return color;
            }
            ENDCG
        }
    }
}