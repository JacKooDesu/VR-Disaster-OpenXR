Shader "Custom/Water"
{
    Properties
    {
        _DepthGradientShallow("Depth Gradient Shallow",Color) = (.325,.807,.971,.725)
        _DepthGradientDeep("Deep",Color) = (.086,.407,1,.749)
        _DepthMaxDistance("Depth Maximum Distance",Float) = 1

        _SurfaceNoise("Noise",2D) = "white" {}

        _SurfaceNoiseCutoff("Noise Cutoff", Range(0,1)) = 0.777
        _FoamMaxDistance("Foam Maximum Distance", Float) = 0.4
        _FoamMinDistance("Foam Minimum Distance", Float) = 0.04

        _SurfaceNoiseScroll("Surface Noise Scroll Amount", Vector) = (.03,.03,0,0)

        _SurfaceDistortion("Surface Distortion", 2D) = "white" {}
        _SurfaceDistortionAmount("Surface Distortion Amount", Range(0,1)) = .27
    }
    SubShader
    {
        Tags { 
            "RenderType"="Transparency" 
            "Queue" = "Transparent"
        }
        //LOD 200
        Pass{
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #define SMOOTHSTEP_AA 0.01

            #pragma vertex vertFunc
            #pragma fragment fragFunc

            #include "UnityCG.cginc"

            float4 _DepthGradientShallow;
            float4 _DepthGradientDeep;

            float _DepthMaxDistance;

            sampler2D _CameraDepthTexture;
            sampler2D _CameraNormalsTexture;

            struct appdata{
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f{
                float4 vertex : SV_POSITION;
                float4 screenPosition : TEXCOORD2;

                float2 noiseUV : TEXCOORD0;
                float2 distortUV : TEXCOORD1;

                float3 viewNormal : NORMAL;
            };

            sampler2D _SurfaceNoise;
            float4 _SurfaceNoise_ST;

            sampler2D _SurfaceDistortion;
            float4 _SurfaceDistortion_ST;

            float _SurfaceNoiseCutoff;

            float _FoamMaxDistance;
            float _FoamMinDistance;

            float2 _SurfaceNoiseScroll;

            v2f vertFunc (appdata _in)
            {
                v2f output;
                output.vertex = UnityObjectToClipPos(_in.vertex);

                output.screenPosition = ComputeScreenPos(output.vertex);
                output.noiseUV = TRANSFORM_TEX(_in.uv, _SurfaceNoise);

                output.distortUV = TRANSFORM_TEX(_in.uv, _SurfaceDistortion);

                output.viewNormal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, _in.normal));
                return output;
            }

            float _SurfaceDistortionAmount;

            float4 fragFunc(v2f _in) : SV_TARGET {
                float existingDepth01 = tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(_in.screenPosition)).r;
                float existingDepthLinear = LinearEyeDepth(existingDepth01);

                float depthDifference = existingDepthLinear - _in.screenPosition.w;
                float waterDepthDifference01 = saturate(depthDifference / _DepthMaxDistance);
                float4 waterColor = lerp(_DepthGradientShallow,_DepthGradientDeep,waterDepthDifference01);
                
                float2 distortSample = (tex2D(_SurfaceDistortion, _in.distortUV).xy * 2 - 1) * _SurfaceDistortionAmount;

                float2 noiseUV = float2(
                                    (_in.noiseUV.x + _Time.y * _SurfaceNoiseScroll.x) + distortSample.x, 
                                    (_in.noiseUV.y + _Time.y * _SurfaceNoiseScroll.y) + distortSample.y);
                float surfaceNoiseSample = tex2D(_SurfaceNoise, noiseUV).r;

                float3 existingNormal = tex2Dproj(_CameraNormalsTexture, UNITY_PROJ_COORD(_in.screenPosition));
                float3 normalDot = saturate(dot(existingNormal, _in.viewNormal));

                float foamDistance = lerp(_FoamMaxDistance, _FoamMinDistance, normalDot);
                float foamDiepthDifference01 = saturate(depthDifference / foamDistance);
                float surfaceNoiseCutoff = foamDiepthDifference01 * _SurfaceNoiseCutoff;
                //float surfaceNoise = surfaceNoiseSample > surfaceNoiseCutoff ? 1 : 0;
                float surfaceNoise = smoothstep(surfaceNoiseCutoff - SMOOTHSTEP_AA, surfaceNoiseCutoff + SMOOTHSTEP_AA, surfaceNoiseSample);

                return waterColor + surfaceNoise;
            }

            ENDCG
        }       
    }
}
