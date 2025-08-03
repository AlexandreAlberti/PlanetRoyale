Shader "Custom/OutlineFresnelAnimatedShader"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        
        // Outline properties
        _OutlineColor("Outline Color", Color) = (1,0,0,1)
        _OutlineWidth("Outline Width", float) = 0.03
        
        // Fresnel properties
        _FresnelPower("Fresnel Power", float) = 1.0
        _FresnelColor("Fresnel Color", Color) = (1,1,1,1)
        
        // Toggle outline on/off
        _OutlineEnabled("Outline Enabled", float) = 1.0

        // Gradient textures for animated color/intensity
        _OutlineGradient("Outline Gradient", 2D) = "white" {}
        _OutlineGradientSpeed("Outline Gradient Speed", float) = 1.0
        _FresnelGradient("Fresnel Gradient", 2D) = "white" {}
        _FresnelGradientSpeed("Fresnel Gradient Speed", float) = 1.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        // Main lit pass with animated Fresnel effect
        Pass
        {
            Name "FORWARD"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            // Main texture
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            
            float4 _Color;
            float _FresnelPower;
            float4 _FresnelColor;
            
            // Fresnel gradient animation
            TEXTURE2D(_FresnelGradient);
            SAMPLER(sampler_FresnelGradient);
            float _FresnelGradientSpeed;
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float3 normal     : NORMAL;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 worldNormal: TEXCOORD1;
                float3 worldPos   : TEXCOORD2;
            };
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.worldNormal = TransformObjectToWorldNormal(IN.normal);
                OUT.worldPos = TransformObjectToWorld(IN.positionOS).xyz;
                return OUT;
            }
            
            half4 frag(Varyings IN) : SV_Target
            {
                // Sample the main texture and ensure an alpha of 1
                half4 texColor = half4(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb, 1.0);
                half4 col = _Color * texColor;
                
                // Calculate Fresnel term based on view angle
                float3 viewDir = normalize(_WorldSpaceCameraPos - IN.worldPos);
                float fresnel = pow(1.0 - saturate(dot(normalize(IN.worldNormal), viewDir)), _FresnelPower);
                
                // Sample the Fresnel gradient for animated color/intensity
                float fresnelTime = frac(_Time.y * _FresnelGradientSpeed);
                float4 animatedFresnelColor = SAMPLE_TEXTURE2D(_FresnelGradient, sampler_FresnelGradient, float2(fresnelTime, 0.5));
                
                // Apply animated Fresnel color
                col.rgb += fresnel * animatedFresnelColor.rgb;
                return col;
            }
            ENDHLSL
        }
        
        // Outline pass with animated outline gradient
        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode"="UniversalForward" }
            Cull Front 
            Blend SrcAlpha OneMinusSrcAlpha
            
            HLSLPROGRAM
            #pragma vertex vertOutline
            #pragma fragment fragOutline
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            float _OutlineWidth;
            float4 _OutlineColor;
            float _FresnelPower;
            float4 _FresnelColor;
            float _OutlineEnabled;
            
            // Outline gradient animation
            TEXTURE2D(_OutlineGradient);
            SAMPLER(sampler_OutlineGradient);
            float _OutlineGradientSpeed;
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normal     : NORMAL;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 worldNormal: TEXCOORD0;
                float3 worldPos   : TEXCOORD1;
            };
            
            Varyings vertOutline(Attributes IN)
            {
                Varyings OUT;
                float3 norm = TransformObjectToWorldNormal(IN.normal);
                float3 worldPos3 = TransformObjectToWorld(IN.positionOS);
                float4 worldPos = float4(worldPos3, 1.0);
                // Expand vertex along its normal for the outline offset
                worldPos.xyz += norm * _OutlineWidth;
                OUT.positionCS = TransformWorldToHClip(worldPos);
                OUT.worldNormal = norm;
                OUT.worldPos = worldPos.xyz;
                return OUT;
            }
            
            half4 fragOutline(Varyings IN) : SV_Target
            {
                // If outline is disabled, discard the fragment
                if (_OutlineEnabled < 0.5)
                    discard;
                
                float3 viewDir = normalize(_WorldSpaceCameraPos - IN.worldPos);
                float fresnel = pow(1.0 - saturate(dot(normalize(IN.worldNormal), viewDir)), _FresnelPower);
                
                // Sample the outline gradient to animate its color/intensity
                float outlineTime = frac(_Time.y * _OutlineGradientSpeed);
                float4 animatedOutlineColor = SAMPLE_TEXTURE2D(_OutlineGradient, sampler_OutlineGradient, float2(outlineTime, 0.5));
                
                return animatedOutlineColor * fresnel;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Forward"
}
