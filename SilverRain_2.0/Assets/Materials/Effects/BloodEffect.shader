Shader "URP/Particles/Blood Effect"
{
    Properties
    {
        [Header (Color Controls)]
        [HDR] _BaseColor ("Base Color Mult", Color) = (1,1,1,1)
        _LightStr ("Lighting Strength", float) = 0.85
        _AlphaMin ("Alpha Clip Min", Range (-0.01, 1.01)) = 0.1
        _AlphaSoft ("Alpha Clip Softness", Range (0,1)) = 0.022
        _EdgeDarken ("Edge Darkening", float) = 1.0
        _ProcMask ("Procedural Mask Strength", float) = 1.0

        [Header (Mask Controls)]
        _MainTex ("Mask Texture", 2D) = "white" {}
        _MaskStr ("Mask Strength", float) = 0.7
        _Columns ("Flipbook Columns", Int) = 1
        _Rows ("Flipbook Rows", Int) = 1
        _ChannelMask ("Channel Mask", Vector) = (1,0,0,0)
        [Toggle] _FlipU("Flip U Randomly", float) = 0
        [Toggle] _FlipV("Flip V Randomly", float) = 0

        [Header (Noise Controls)]
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseAlphaStr ("Noise Strength", float) = 0.8
        _ChannelMask2 ("Channel Mask",Vector) = (1,0,0,0)
        _Randomize ("Randomize Noise", float) = 1.0

        [Header (Reflections  Normals  Warp)]
        _SpecularColor ("Specular Color (RGB) + Control (A)", Color) = (1,1,1,0.5)
        _ReflectionTex ("Reflection Texture", 2D) = "gray" {}
        _ReflectionSat ("Reflection Saturation", float) = 1.0
        _NoiseColorStr ("Noise Color Influence", float) = 0.0
        _Normal ("Normal Map", 2D) = "bump" {}
        _FlattenNormal ("Flatten Normal Near Edge", float) = 1.0
        _WarpTex ("Warp Texture", 2D) = "gray" {}
        _WarpStr ("Warp Strength", float) = 1.0

        [Header (Vertex Physics)]
        _FallOffset ("Gravity Offset", range(-1,0)) = -1.0
        _FallRandomness ("Gravity Randomness", float) = 0.25
    }

    SubShader
    {
        Tags
        {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "FORWARD"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // Textures
            TEXTURE2D(_MainTex);        SAMPLER(sampler_MainTex);
            TEXTURE2D(_NoiseTex);       SAMPLER(sampler_NoiseTex);
            TEXTURE2D(_WarpTex);        SAMPLER(sampler_WarpTex);
            TEXTURE2D(_Normal);         SAMPLER(sampler_Normal);
            TEXTURE2D(_ReflectionTex);  SAMPLER(sampler_ReflectionTex);

            // Color controls
            half4 _BaseColor;
            half4 _SpecularColor;
            half  _LightStr;
            half  _AlphaMin;
            half  _AlphaSoft;
            half  _EdgeDarken;
            half  _ProcMask;

            // Mask controls
            float4 _MainTex_ST;
            half   _MaskStr;
            half   _Columns;
            half   _Rows;
            half4  _ChannelMask;
            half   _FlipU;
            half   _FlipV;

            // Reflection / normals / warp
            float4 _ReflectionTex_ST;
            half   _ReflectionSat;
            float4 _NoiseTex_ST;
            half   _NoiseAlphaStr;
            half   _NoiseColorStr;
            half4  _ChannelMask2;
            half   _FlattenNormal;
            float4 _WarpTex_ST;
            half   _WarpStr;

            // Noise randomization
            half   _Randomize;

            // Vertex physics
            half   _FallOffset;
            half   _FallRandomness;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 texcoord0  : TEXCOORD0; // Z = Random, W = Lifetime
                float3 texcoord1  : TEXCOORD1; // X = Pan Offset, Y = UV Warp Strength, Z = Gravity
                float4 color      : COLOR;
                float4 tangentOS  : TANGENT;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 uv          : TEXCOORD0; // xy = main, zw = random/panned
                float4 color       : COLOR;
                float3 normalWS    : TEXCOORD1;
                float3 viewDirWS   : TEXCOORD2;
                float4 vertLight   : TEXCOORD3;
                float3 customData  : TEXCOORD4; // x = pan offset, y = warp strength, z = stable random
                float3x3 t2w       : TEXCOORD5; // tangent-to-world
            };

            Varyings vert (Attributes v)
            {
                Varyings o;

                // Lifetime-based fall
                float lifetime = v.texcoord0.w;
                lifetime = lifetime * lifetime +
                           (_FallOffset + ((v.texcoord0.z - 0.5) * _FallRandomness)) * lifetime;

                float4 fallPos = lifetime * float4(0, v.texcoord1.z, 0, 0);

                // UV flip based on random
                float2 UVflip = round(frac(float2(v.texcoord0.z * 13.0, v.texcoord0.z * 8.0)));
                UVflip = UVflip * 2.0 - 1.0;
                UVflip = lerp(float2(1.0, 1.0), UVflip, float2(_FlipU, _FlipV));

                float3 worldPos = TransformObjectToWorld(v.positionOS.xyz) + fallPos.xyz;
                o.positionHCS   = TransformWorldToHClip(worldPos);

                o.color = v.color;
                o.color.a *= o.color.a;
                o.color.a += _AlphaMin;

                float3 normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.normalWS = normalWS;

                o.customData = float3(v.texcoord1.xy, v.texcoord0.z);

                // Main UV
                float2 uvMain = v.texcoord0.xy * UVflip;
                uvMain = uvMain * _MainTex_ST.xy + _MainTex_ST.zw;
                o.uv.xy = uvMain;

                // Randomized / panned UV (for noise/warp)
                float2 uvRand = uvMain * float2(_Columns, _Rows) +
                                v.texcoord0.z * float2(3.0, 8.0) * _Randomize;
                o.uv.zw = uvRand;

                // Tangent-to-world for normal mapping
                float3 tangentWS = TransformObjectToWorldDir(v.tangentOS.xyz);
                float3 binormalWS = normalize(cross(normalWS, tangentWS) * v.tangentOS.w);
                float3x3 rotation = float3x3(tangentWS, binormalWS, normalWS);
                o.t2w = rotation;

                // View direction
                float3 viewDirWS = normalize(GetWorldSpaceViewDir(worldPos));
                o.viewDirWS = viewDirWS;

                // SH lighting approximation
                half3 shade = SampleSH(normalWS);
                shade = max(shade, half3(0.15, 0.15, 0.15));
                o.vertLight.xyz = lerp(half3(1,1,1), shade, _LightStr);
                o.vertLight.w   = 0;

                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                // UV warp
                float2 warpUV = i.uv.zw * _WarpTex_ST.xy +
                                _WarpTex_ST.zw * (i.customData.x + 1.0) +
                                float2(5.0, 8.0) * i.customData.z;

                float4 uvWarp = SAMPLE_TEXTURE2D(_WarpTex, sampler_WarpTex, warpUV);
                float2 warp = (uvWarp.xy * 2.0 - 1.0) * (_WarpStr * i.customData.y);

                // Mask
                float2 mainUV = i.uv.xy * _MainTex_ST.xy + warp;
                half4 mask = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, mainUV);
                mask = saturate(lerp(half4(1,1,1,1), mask, _MaskStr));

                // Edge mask (prevent spill off quad)
                half2 tempUV = frac(i.uv.xy * half2(_Columns, _Rows)) - 0.5;
                tempUV *= tempUV * 4.0;
                half edgeMask = saturate(tempUV.x + tempUV.y);
                edgeMask *= edgeMask;
                edgeMask = 1.0 - edgeMask;
                edgeMask = lerp(1.0, edgeMask, _ProcMask);
                mask *= edgeMask;

                half4 col = max(half4(0.001,0.001,0.001,0.001), i.color);
                col.a = saturate(dot(mask, _ChannelMask));

                // Noise
                float2 noiseUV = i.uv.zw * _NoiseTex_ST.xy +
                                 _NoiseTex_ST.zw * i.customData.x + warp;
                half4 noise4 = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseUV);
                half noise = dot(noise4, _ChannelMask2);
                noise = saturate(lerp(half(1.0), noise, _NoiseAlphaStr));

                // Alpha clip
                col.a *= noise;
                half preClipAlpha = col.a;
                half clippedAlpha = saturate((preClipAlpha * i.color.a - _AlphaMin) / _AlphaSoft);
                col.a = clippedAlpha;

                // Base lighting
                half3 baseLighting = max(half3(0.01,0.01,0.01), i.vertLight.xyz);

                // Specular / reflections
                half3 spec = 0;
                {
                    // Normal map
                    half3 normalTex = UnpackNormal(SAMPLE_TEXTURE2D(_Normal, sampler_Normal, noiseUV));

                    // Flatten normals near alpha edge
                    half flatten = _FlattenNormal;
                    half edgeFactor = saturate((preClipAlpha * i.color.a - _AlphaMin) / (_AlphaSoft + 0.2)) - 0.1;
                    normalTex.z = flatten * edgeFactor * 1.2;
                    normalTex = normalize(normalTex);

                    // Transform to world
                    float3 normalWS = normalize(mul(i.t2w, normalTex));
                    float3 combinedNormals = normalize(i.normalWS + normalWS);

                    // Reflection vector
                    float3 reflectionVector = reflect(-i.viewDirWS, combinedNormals);
                    float angle = atan2(reflectionVector.x, reflectionVector.z) * 0.31831;
                    reflectionVector.x = angle;
                    reflectionVector *= 0.5;

                    float2 reflectionUV = reflectionVector.xy * _ReflectionTex_ST.xy +
                                          _ReflectionTex_ST.zw * (_Time.x + i.customData.z);

                    float3 reflectionTex = SAMPLE_TEXTURE2D(_ReflectionTex, sampler_ReflectionTex, reflectionUV).rgb;

                    float desatReflection = dot(reflectionTex, float3(0.333,0.333,0.333));
                    float3 refl = lerp(desatReflection.xxx, reflectionTex, _ReflectionSat);

                    float3 spec0 = refl;
                    float3 spec1 = spec0 * spec0 * spec0 * spec0;
                    spec = clamp(lerp(spec0, spec1, _SpecularColor.a * preClipAlpha), 0.0, 10.0);

                    float fresnel = 1.0 - dot(i.viewDirWS, combinedNormals) * _SpecularColor.a;
                    spec *= clamp(fresnel, 0.2, 1.0);
                }

                // Edge
                half edge = 1.0 - saturate(preClipAlpha * clippedAlpha);
                edge *= edge;
                edge = 1.0 - edge;
                edge = edge + lerp(half(0.0), noise - 0.5, _NoiseColorStr);

                // Edge darken
                edge = saturate(lerp(0.71, edge * edge, _EdgeDarken));

                // Edge alpha
                col.a *= saturate(lerp(1.25, _BaseColor.a, edge));

                // Non-specular branch in original doubled edge; we always keep spec, so skip that.

                // Prevent crazy overbright
                col.rgb *= lerp(min(col.rgb * col.rgb * col.rgb * 0.3, 1.0), 0.71, edge);

                // Tint + lighting + spec
                col.rgb *= max(half3(0,0,0), baseLighting * _BaseColor.rgb);
                col.rgb += baseLighting * spec * _SpecularColor.rgb;

                return col;
            }

            ENDHLSL
        }
    }
}