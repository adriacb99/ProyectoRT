Shader "Unlit/Prueba"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        planetCentre ("Planet Centre", Vector) = (0.,0.,0.)
        atmosphereRadius ("Atmosphere Radius", Float) = 1.0
        planetRadius ("Planet Radius", Float) = 1.0
        densityFalloff ("Density Falloff", Float) = 1.0
        numOpticalDepthPoints ("NumOpticalDepthPoints", Float) = 1.0
        numInScatteringPoints ("NumInScatteringPoints", Float) = 1.0
        dirToSun ("Dir To Sun", Vector) = (1.,0.,0.)
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 viewVector : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _CameraDepthTexture;
            float3 planetCentre;
            float atmosphereRadius;

            float planetRadius;
            float densityFalloff;

            float numOpticalDepthPoints;
            float numInScatteringPoints;

            float3 dirToSun;
            float4 scatteringCoefficients;



            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);

                float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv.xy * 2 - 1, 0, -1));
				o.viewVector = mul(unity_CameraToWorld, float4(viewVector,0));
                return o;
            }

            float2 raySphere (float3 sphereCentre, float sphereRadius, float3 rayOrigin, float3 rayDir) {
                float3 offset = rayOrigin - sphereCentre;
                const float a = 1; // set to dot(rayDir, rayDir) if rayDir might be unnormalized
                float b = 2 * dot(offset, rayDir);
                float c = dot(offset, offset) - sphereRadius * sphereRadius;
                float discriminant = b * b - 4 * a * c;

                // No intersections: discriminant < 0
                // 1 intersection: discriminant == 0
                // 2 intersections: discriminant > 0
                if (discriminant > 0) {
                    float s = sqrt(discriminant);
                    float dstToSphereNear = max(0, (-b - s) / (2 * a));
                    float dstToSphereFar = (-b + s) / (2 * a);

                    if (dstToSphereFar >= 0) {
                        return float2(dstToSphereNear, dstToSphereFar - dstToSphereNear);
                    }
                }
                return float2(0,0);
            }

            float densityAtPoint(float3 densitySamplePoint) {
				float heightAboveSurface = length(densitySamplePoint - planetCentre) - planetRadius;
				float height01 = heightAboveSurface / (atmosphereRadius - planetRadius);
				float localDensity = exp(-height01 * densityFalloff) * (1 - height01);
				return localDensity;
			}
			
			float opticalDepth(float3 rayOrigin, float3 rayDir, float rayLength) {
				float3 densitySamplePoint = rayOrigin;
				float stepSize = rayLength / (numOpticalDepthPoints - 1);
				float opticalDepth = 0;

				for (int i = 0; i < numOpticalDepthPoints; i ++) {
					float localDensity = densityAtPoint(densitySamplePoint);
					opticalDepth += localDensity * stepSize;
					densitySamplePoint += rayDir * stepSize;
				}
				return opticalDepth;
			}

            float3 calculateLight(float3 rayOrigin, float3 rayDir, float rayLength, float3 originalCol, float sceneDepth)
            {
                float3 inScatterPoint = rayOrigin;
				float stepSize = rayLength / (numInScatteringPoints - 1);
				float3 inScatteredLight = 0;
                float viewRayOpticalDepth = 0;
				for (int i = 0; i < numInScatteringPoints; i ++) {
					float sunRayLength = raySphere(planetCentre, atmosphereRadius, inScatterPoint, dirToSun).y;
					float sunRayOpticalDepth = opticalDepth(inScatterPoint, dirToSun, sunRayLength);
					float localDensity = densityAtPoint(inScatterPoint);
					viewRayOpticalDepth = opticalDepth(inScatterPoint, -rayDir, stepSize * i);
					float3 transmittance = exp(-(sunRayOpticalDepth + viewRayOpticalDepth)*scatteringCoefficients);
					
					inScatteredLight += localDensity * transmittance * scatteringCoefficients * stepSize;
					inScatterPoint += rayDir * stepSize;
				}
                float originalColTransmittance = exp(-viewRayOpticalDepth);
                return originalCol * originalColTransmittance + inScatteredLight;
			}

            fixed4 frag (v2f i) : SV_Target
            {
				float4 originalCol = tex2D(_MainTex, i.uv);
				float sceneDepthNonLinear = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float sceneDepth = LinearEyeDepth(sceneDepthNonLinear) * length(i.viewVector);
                //sceneDepth = LinearEyeDepth(sceneDepthNonLinear);
                //return float4(sceneDepthNonLinear.xxx, 1);
                //if (sceneDepthNonLinear > 0) sceneDepth 

				float3 rayOrigin = _WorldSpaceCameraPos;
				float3 rayDir = normalize(i.viewVector);
						
				float2 hitInfo = raySphere(planetCentre, atmosphereRadius, rayOrigin, rayDir);
				float dstToAtmosphere = hitInfo.x;
				float dstThroughAtmosphere = min(hitInfo.y, sceneDepth - dstToAtmosphere);
                //if (sceneDepthNonLinear > 0.01) dstThroughAtmosphere = 0.3;

                if (dstThroughAtmosphere > 0) {
					const float epsilon = 0.0001;
					float3 pointInAtmosphere = rayOrigin + rayDir * (dstToAtmosphere + epsilon);
                    float3 light = calculateLight(pointInAtmosphere, rayDir, dstThroughAtmosphere - epsilon * 2, originalCol, sceneDepth);
					return float4(light, 0);
				}
				return originalCol; 
            }          
            ENDCG
        }
    }
}
