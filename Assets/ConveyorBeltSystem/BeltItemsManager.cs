using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class BeltItemsManager : MonoBehaviour
{
    [SerializeField] SplineContainer splineContainer;

    [SerializeField]
    [Range(0f, 1f)]
    private float time;

    float3 position;
    float3 tangent;
    float3 upVector;


    // Update is called once per frame
    void Update()
    {
        splineContainer.Evaluate(1, time, out position, out tangent, out upVector);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(position+upVector/6, 0.05f);
    }
}
