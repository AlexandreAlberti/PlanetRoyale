Shader "Custom/OutlineFresnelShader"
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
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        // Main lit pass
        Pass
        {
            Name "FORWARD"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            // Use URP macros for texture and sampler
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            
            float4 _Color;
            float _FresnelPower;
            float4 _FresnelColor;
            
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
                half4 texColor = half4(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb, 1.0);
                half4 col = _Color * texColor;
                
                // Calculate Fresnel term for a subtle edge effect
                float3 viewDir = normalize(_WorldSpaceCameraPos - IN.worldPos);
                float fresnel = pow(1.0 - saturate(dot(normalize(IN.worldNormal), viewDir)), _FresnelPower);
                
                // Blend in the Fresnel effect
                col.rgb += fresnel * _FresnelColor.rgb;
                return col;
            }
            ENDHLSL
        }
        
        // Outline pass
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
                // Transform normal to world space
                float3 norm = TransformObjectToWorldNormal(IN.normal);
                // Convert the object space position to world space (explicit conversion)
                float3 worldPos3 = TransformObjectToWorld(IN.positionOS);
                float4 worldPos = float4(worldPos3, 1.0);
                // Expand vertex along its normal for outline offset
                worldPos.xyz += norm * _OutlineWidth;
                OUT.positionCS = TransformWorldToHClip(worldPos);
                OUT.worldNormal = norm;
                OUT.worldPos = worldPos.xyz;
                return OUT;
            }
            
            half4 fragOutline(Varyings IN) : SV_Target
            {
                if (_OutlineEnabled < 0.5)
                    discard;
                
                float3 viewDir = normalize(_WorldSpaceCameraPos - IN.worldPos);
                float fresnel = pow(1.0 - saturate(dot(normalize(IN.worldNormal), viewDir)), _FresnelPower);
                
                return _OutlineColor * fresnel;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Forward"
}
