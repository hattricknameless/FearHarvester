Shader "Unlit/LightBlockShader"
{
    Properties
    {
        
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            ZWrite On
            ColorMask 0
        }
    }
}
