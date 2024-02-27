using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct JobPrueba : IJobParallelFor
{
    [ReadOnly] public NativeList<float3> trisJ;

    [WriteOnly] public NativeArray<float3> sideTrisJob;
    
    public void Execute(int index)
    {
        float3 triJ = trisJ.ElementAt(index);

        float3 vec = new float3();

        int v1 = (int)triJ.x;
        int v2 = (int)triJ.y;
        int v3 = (int)triJ.z;

        // first check if we have it already
        bool firstIsSmaller = v1 < v2;
        long smallerIndex = firstIsSmaller ? v1 : v2;
        long greaterIndex = firstIsSmaller ? v2 : v1;
        long key = (smallerIndex << 32) + greaterIndex;
        //Debug.Log(key);
        int indexTri = 0;
        foreach (var triangle in trisJ)
        {
            if (!triangle.Equals(triJ))
            {
                firstIsSmaller = (int)triangle.x < (int)triangle.y;
                smallerIndex = firstIsSmaller ? (int)triangle.x : (int)triangle.y;
                greaterIndex = firstIsSmaller ? (int)triangle.y : (int)triangle.x;
                long key2 = (smallerIndex << 32) + greaterIndex;

                if (key == key2)
                {
                    vec.x = indexTri;
                }
                firstIsSmaller = (int)triangle.x < (int)triangle.z;
                smallerIndex = firstIsSmaller ? (int)triangle.x : (int)triangle.z;
                greaterIndex = firstIsSmaller ? (int)triangle.z : (int)triangle.x;
                long key3 = (smallerIndex << 32) + greaterIndex;

                if (key == key3)
                {
                    vec.x = indexTri;
                }
                firstIsSmaller = (int)triangle.z < (int)triangle.y;
                smallerIndex = firstIsSmaller ? (int)triangle.z : (int)triangle.y;
                greaterIndex = firstIsSmaller ? (int)triangle.y : (int)triangle.z;
                long key4 = (smallerIndex << 32) + greaterIndex;

                if (key == key4)
                {
                    vec.x = indexTri;
                }
            }
            indexTri++;
        }

        // first check if we have it already
        firstIsSmaller = v1 < v3;
        smallerIndex = firstIsSmaller ? v1 : v3;
        greaterIndex = firstIsSmaller ? v3 : v1;
        key = (smallerIndex << 32) + greaterIndex;

        indexTri = 0;
        foreach (var triangle in trisJ)
        {
            if (!triangle.Equals(triJ))
            {
                firstIsSmaller = (int)triangle.x < (int)triangle.y;
                smallerIndex = firstIsSmaller ? (int)triangle.x : (int)triangle.y;
                greaterIndex = firstIsSmaller ? (int)triangle.y : (int)triangle.x;
                long key2 = (smallerIndex << 32) + greaterIndex;

                if (key == key2)
                {
                    vec.y = indexTri;
                }
                firstIsSmaller = (int)triangle.x < (int)triangle.z;
                smallerIndex = firstIsSmaller ? (int)triangle.x : (int)triangle.z;
                greaterIndex = firstIsSmaller ? (int)triangle.z : (int)triangle.x;
                long key3 = (smallerIndex << 32) + greaterIndex;

                if (key == key3)
                {
                    vec.y = indexTri;
                }
                firstIsSmaller = (int)triangle.z < (int)triangle.y;
                smallerIndex = firstIsSmaller ? (int)triangle.z : (int)triangle.y;
                greaterIndex = firstIsSmaller ? (int)triangle.y : (int)triangle.z;
                long key4 = (smallerIndex << 32) + greaterIndex;

                if (key == key4)
                {
                    vec.y = indexTri;
                }
            }
            indexTri++;
        }

        // first check if we have it already
        firstIsSmaller = v2 < v3;
        smallerIndex = firstIsSmaller ? v2 : v3;
        greaterIndex = firstIsSmaller ? v3 : v2;
        key = (smallerIndex << 32) + greaterIndex;

        indexTri = 0;
        foreach (var triangle in trisJ)
        {
            if (!triangle.Equals(triJ))
            {
                firstIsSmaller = (int)triangle.x < (int)triangle.y;
                smallerIndex = firstIsSmaller ? (int)triangle.x : (int)triangle.y;
                greaterIndex = firstIsSmaller ? (int)triangle.y : (int)triangle.x;
                long key2 = (smallerIndex << 32) + greaterIndex;

                if (key == key2)
                {
                    vec.z = indexTri;
                }
                firstIsSmaller = (int)triangle.x < (int)triangle.z;
                smallerIndex = firstIsSmaller ? (int)triangle.x : (int)triangle.z;
                greaterIndex = firstIsSmaller ? (int)triangle.z : (int)triangle.x;
                long key3 = (smallerIndex << 32) + greaterIndex;

                if (key == key3)
                {
                    vec.z = indexTri;
                }
                firstIsSmaller = (int)triangle.z < (int)triangle.y;
                smallerIndex = firstIsSmaller ? (int)triangle.z : (int)triangle.y;
                greaterIndex = firstIsSmaller ? (int)triangle.y : (int)triangle.z;
                long key4 = (smallerIndex << 32) + greaterIndex;

                if (key == key4)
                {
                    vec.z = indexTri;
                }
            }
            indexTri++;
        }        
        sideTrisJob[index] = vec;
    }
}
