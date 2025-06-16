Shader "Unlit/WaterFall"
{
    Properties
    {
        _CellSize ("Cell Size", Range(0,2)) = 1

        _WaterColor ("Water Color", Color) = (0,0,0)
        _FoamColor ("Foam Color", Color) = (0,0,0)

        _FoamMaxDistant("Max Dis",Range(0,1)) = .5
        _FoamMinDistant("Min Dis",Range(0,1)) = .01

        _SurfaceNoiseCutOff("Cut Off",Range(0,1)) = .3 

        _ScrollSpeed("Scroll Speed",Vector) = (0,0,0,0)

        _SurfaceNoise ("Noise", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            #define SMOOTHSTEP_AA 0.01

            float _CellSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;

            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD2;

                float2 noiseUV : TEXCOORD0;
                float3 viewNormal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _SurfaceNoise;
            float4 _SurfaceNoise_ST;

            float _FoamMaxDistant;
            float _FoamMinDistant;

            float _SurfaceNoiseCutOff;

            float2 _ScrollSpeed;

            sampler2D _CameraDepthTexture;
            sampler2D _CameraNormalsTexture;

            fixed4 _WaterColor;

            float2 random2d(float p){
                return frac(sin(float2(dot(p,float2(117.12,341.7)),dot(p,float2(269.5,123.3))))*43458.5453);
            }

            float voronoiNoise(float2 value){
                float2 cell = floor(value);
                float2 cellPos = cell + random2d(cell);
                float2 toCell = cellPos - value;
                float distToCell = length(toCell);
                return distToCell;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.noiseUV = TRANSFORM_TEX(v.uv, _SurfaceNoise);
                o.viewNormal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV,v.normal));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float existingDepth01 = tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(i.screenPos)).r;
                fixed4 col = fixed4(0,0,0,1);
                //float2 uv = i.uv;

                float2 noiseUV = float2(
                    i.noiseUV.x + _Time.y * _ScrollSpeed.x,
                    i.noiseUV.y + _Time.y * _ScrollSpeed.y
                );
                float surfaceNoiseSample = tex2D(_SurfaceNoise, noiseUV).r;

                float3 existingNormal = tex2Dproj(_CameraNormalsTexture, UNITY_PROJ_COORD(i.screenPos));
                float3 normalDot = saturate(dot(existingNormal,i.viewNormal));

                float foamDistance = lerp(_FoamMaxDistant, _FoamMinDistant, normalDot);
                float surfaceNoise = smoothstep(_SurfaceNoiseCutOff - SMOOTHSTEP_AA ,_SurfaceNoiseCutOff + SMOOTHSTEP_AA,surfaceNoiseSample);
                return _WaterColor + surfaceNoise;
            }
            ENDCG
        }
    }
}
