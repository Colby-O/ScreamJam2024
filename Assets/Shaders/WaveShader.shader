// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/WaveShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Water Color", Color) = (1,1,1,1)
        _WaveA ("Wave A (dir, steepness, wavelength)", Vector) = (1,0,0.5,10)
        _WaveB ("Wave B", Vector) = (0,1,0.25,20)
        _WaveC ("Wave C", Vector) = (1,1,0.15,10)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
            };

            sampler2D m_MainTex;
            float4 m_MainTex_ST;

            fixed4 _Color;
            float4 _WaveA, _WaveB, _WaveC;

            float myTanh(float x) {
                return (exp(2.0 * x) - 1) / (exp(2.0 * x) + 1);
            }

            float3 CalcGerstnerWave(float4 wave, float3 pt, inout float3 binormal, inout float3 tangent) {
                float smoothness = wave.z;
                float wavelength = wave.w;
                float k = 2 * UNITY_PI / wavelength;
                float speed = sqrt(9.81 / k);
                float2 dir = normalize(wave.xy);
                float f = k * (dot(dir, pt.xz) - speed * _Time.y);

                tangent += float3(-dir.x * dir.x * (exp(-k / smoothness) * sin(f)), dir.x * (exp(-k / smoothness) * cos(f)),-dir.x * dir.y * (exp(-k / smoothness) * sin(f)));
                binormal += float3(-dir.x * dir.y * (exp(-k / smoothness) * sin(f)), dir.y * (exp(-k / smoothness) * cos(f)), -dir.y * dir.y * (exp(-k / smoothness) * sin(f)));

                return float3(dir.x * exp(-k / smoothness) * cos(f) / k, exp(-k / smoothness) * sin(f) / k, dir.y * exp(-k / smoothness) * cos(f) / k);

            }

            void vert(appdata v) 
            {
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float4 worldUV = float4(worldPos.xz, 0, 0);
                float3 gridPt = worldPos;
                float3 pt = v.vertex.xyz;
                //float3 viewVector = _WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz;
                //float viewDist = length(viewVector);
                float3 tangent = float3(1, 0, 0);
                float3 binormal = float3(0, 0, 1);

                pt += CalcGerstnerWave(_WaveA, gridPt, binormal, tangent);
                pt += CalcGerstnerWave(_WaveB, gridPt, binormal, tangent);
                pt += CalcGerstnerWave(_WaveC, gridPt, binormal, tangent);

                float3 normal = normalize(cross(binormal, tangent));

                v.vertex.xyz = pt;

                v2f o;
                o.vertex = v.vertex;
                o.normal = normal;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Color;
                return col;
            }
            ENDCG
        }
    }
}
