#ifndef FUNCTIONSHLSL_INCLUDED
#define FUNCTIONSHLSL_INCLUDED

void raySphere_float(float3 sphereCentre, float sphereRadius, float3 rayOrigin, float3 rayDir, out float2 Out) {
    float3 offset = rayOrigin - sphereCentre;
    float a = 1; // set to dot(rayDir, rayDir) if rayDir might be unnormalized
    float b = 2 * dot(offset, rayDir);
    float c = dot(offset, offset) - sphereRadius * sphereRadius;
    float discriminant = b * b - 4 * a * c;

    if (discriminant > 0) {
        float s = sqrt(discriminant);
        float dstToSphereNear = max(0, (-b - s) / (2 * a));
        float dstToSphereFar = (-b + s) / (2 * a);
        if (dstToSphereFar >= 0) {
            Out = float2(dstToSphereNear, dstToSphereFar - dstToSphereNear);
        }
        else Out = float2(0, 0);
    }
    else Out = float2(0, 0);
}


float numInScatteringPoints;
float3 planetCentre;
float atmosphereRadius;
float3 dirToSun;
float numOpticalDepthPoints;
float planetRadius;
float densityFalloff;
float3 scatteringCoefficients;

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

float2 raySphere(float3 sphereCentre, float sphereRadius, float3 rayOrigin, float3 rayDir) {
    float3 offset = rayOrigin - sphereCentre;
    float a = 1; // set to dot(rayDir, rayDir) if rayDir might be unnormalized
    float b = 2 * dot(offset, rayDir);
    float c = dot(offset, offset) - sphereRadius * sphereRadius;
    float discriminant = b * b - 4 * a * c;

    if (discriminant > 0) {
        float s = sqrt(discriminant);
        float dstToSphereNear = max(0, (-b - s) / (2 * a));
        float dstToSphereFar = (-b + s) / (2 * a);
        if (dstToSphereFar >= 0) {
            return float2(dstToSphereNear, dstToSphereFar - dstToSphereNear);
        }
    }
    return float2(0, 0);
}

void calculateLight_float(float3 scatteringCoefficientsShader, float densityFalloffShader, float planetRadiusShader, float numOpticalDepthPointsShader, float3 dirToSunShader, float atmosphereRadiusShader, float3 planetCentreShader, float numInScatteringPointsShader, float3 rayOrigin, float3 rayDir, float rayLength, out float Out) {
    numInScatteringPoints = numInScatteringPointsShader;
    planetCentre = planetCentreShader;
    atmosphereRadius = atmosphereRadiusShader;
    dirToSun = dirToSunShader;
    numOpticalDepthPoints = numOpticalDepthPointsShader;
    planetRadius = planetRadiusShader;
    densityFalloff = densityFalloffShader;
    scatteringCoefficients = scatteringCoefficientsShader;

	float3 inScatterPoint = rayOrigin;
	float stepSize = rayLength / (numInScatteringPoints - 1);
	float3 inScatteredLight = 0;
	float viewRayOpticalDepth = 0;

	for (int i = 0; i < numInScatteringPoints; i ++) {
		float sunRayLength = raySphere(planetCentre, atmosphereRadius, inScatterPoint, dirToSun).y;
		float sunRayOpticalDepth = opticalDepth(inScatterPoint, dirToSun, sunRayLength);
		float localDensity = densityAtPoint(inScatterPoint);
		float3 transmittance = exp(-(sunRayOpticalDepth + viewRayOpticalDepth) * scatteringCoefficients);
		
		inScatteredLight += localDensity * transmittance * stepSize;
		inScatterPoint += rayDir * stepSize;
	}
    Out = inScatteredLight;
}

#endif //FUNCTIONSHLSL_INCLUDED