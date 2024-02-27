using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AtmosphereSettings : ScriptableObject
{
	public Shader atmosphereShader;
	public int inScatteringPoints = 10;
	public int opticalDepthPoints = 10;
	public float densityFalloff = 4f;

    [Range(0.0f, 1.0f)]
    public float atmosphereScale;

	public Vector3 wavelengths = new Vector3 (700, 530, 460);
	public float scatteringStrength = 20;

    public void SetProperties(Material material, float bodyRadius) {
        float scatterR = Mathf.Pow(400/wavelengths.x, 4) * scatteringStrength;
        float scatterG = Mathf.Pow(400/wavelengths.y, 4) * scatteringStrength;
        float scatterB = Mathf.Pow(400/wavelengths.z, 4) * scatteringStrength;
        Vector4 scatteringCoefficients =new Vector3(scatterR,scatterG,scatterB);

        material.SetVector("scatteringCoefficients", scatteringCoefficients);
        material.SetInt("numInScatteringPoints", inScatteringPoints);
        material.SetInt("numOpticalDepthPoints", opticalDepthPoints);
        material.SetFloat("atmosphereRadius", (1+atmosphereScale) * bodyRadius);
        material.SetFloat("planetRadius", bodyRadius);
        material.SetFloat("densityFalloff", densityFalloff);
    }
}