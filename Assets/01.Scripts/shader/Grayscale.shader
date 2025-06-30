Shader "PostEffect/GrayScale" {
    Properties
    {
        [HideInspector] _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Tags { "RenderType"="Opaque" "Renderpipeline"="UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            half4 frag (Varyings i) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearRepeat, i.texcoord);
                half gray = dot(color.rgb, half3(0.2126, 0.7152, 0.0722));
                return half4(gray, gray, gray, color.a);
            }
            ENDHLSL
        }
    }
}