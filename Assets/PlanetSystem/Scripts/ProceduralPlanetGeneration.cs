using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralPlanetGeneration : MonoBehaviour
{
    private float radius = 1.0f;
    [SerializeField] private int frequency = 1;
    [SerializeField] Material material;

    public static ProceduralPlanetGeneration Instance { get; private set; }

    private Dictionary<Int64, int> middlePointIndexCache;

    // GRID
    public List<int>[] Grid;
    public int[] trisIndex;
    public Vector3[] midFacePoints;
    public int[] tileTypeArray;

    int index;
    int indexPos;
    int indexFace;

    private void Awake()
    {
        Instance = this;
    }

    // Crear Mesh usando Advanced Mesh API
    public void CreatePlanet(float radi, GameObject gObj, int size, out List<int>[] GridPlanet, out int[] trisIndexPlanet, out Vector3[] midFacePoints, out int[] tileTypeArrayPlanet)
    {
        radius = radi;
        middlePointIndexCache = new Dictionary<Int64, int>();
        index = 11;
        indexFace = 0;
        indexPos = 0;

        frequency = size;

        int vertexAttributeCount = 3;

        int trisCount2 = 3 * 20 * (int)Mathf.Pow(4, frequency);
        int vertCount2 = 2 + ((3 * trisCount2/3)/2) - trisCount2/3;
        int trisCount = ((vertCount2 * 4)-12)*3;
        int vertCount = (vertCount2*6)-12;

        tileTypeArray = new int[vertCount2];
        trisIndex = new int[trisCount];
        midFacePoints = new Vector3[vertCount2];
        Grid = new List<int>[vertCount2];
        for (int i = 0; i < vertCount2; i++) Grid[i] = new List<int>();


        Debug.Log(vertCount);
        Debug.Log(trisCount);

        Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
        Mesh.MeshData meshData = meshDataArray[0];

        var vertexAttributes = new NativeArray<VertexAttributeDescriptor>(
            vertexAttributeCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory
        );
        vertexAttributes[0] = new VertexAttributeDescriptor(dimension: 3);
        vertexAttributes[1] = new VertexAttributeDescriptor(
            VertexAttribute.Normal, dimension: 3, stream: 1
        );
        //vertexAttributes[2] = new VertexAttributeDescriptor(
        //    VertexAttribute.Tangent, dimension: 4, stream: 2
        //);
        vertexAttributes[2] = new VertexAttributeDescriptor(
            VertexAttribute.TexCoord0, dimension: 2, stream: 2
        );

        meshData.SetVertexBufferParams(vertCount, vertexAttributes);
        vertexAttributes.Dispose();

        // The golden ratio
        //float t = (1.0f + (float)Math.Sqrt(5.0)) / 2.0f;
        float phi = (1.0f + Mathf.Sqrt(5.0f)) * 0.5f; // golden ratio
        float a = 1.0f;
        float b = 1.0f / phi;


        NativeArray<Vector3> positions = meshData.GetVertexData<Vector3>();
        positions[0] = new Vector3(0, b, -a).normalized * radius;
        positions[1] = new Vector3(b, a, 0f).normalized * radius;
        positions[2] = new Vector3(-b, a, 0f).normalized * radius;
        positions[3] = new Vector3(0f, b, a).normalized * radius;
        positions[4] = new Vector3(0f, -b, a).normalized * radius;
        positions[5] = new Vector3(-a, 0f, b).normalized * radius;
        positions[6] = new Vector3(0f, -b, -a).normalized * radius;
        positions[7] = new Vector3(a, 0, -b).normalized * radius;
        positions[8] = new Vector3(a, 0f, b).normalized * radius;
        positions[9] = new Vector3(-a, 0f, -b).normalized * radius;
        positions[10] = new Vector3(b, -a, 0).normalized * radius;
        positions[11] = new Vector3(-b, -a, 0).normalized * radius;

        NativeArray<Vector3> normals = meshData.GetVertexData<Vector3>(1);
        normals[0] = normals[1] = normals[2] = normals[3] = Vector3.back;

        NativeArray<Vector2> texCoords = meshData.GetVertexData<Vector2>(2); 

        meshData.SetIndexBufferParams(trisCount, IndexFormat.UInt32);
        NativeArray<UInt32> triangleIndices = meshData.GetIndexData<UInt32>();

        // crea 20 triangulos del icosaedro
        List<Vector3> tris = new List<Vector3>();
        tris.Add(new Vector3(2,1,0));
        tris.Add(new Vector3(1,2,3));
        tris.Add(new Vector3(5,4,3));
        tris.Add(new Vector3(4,8,3));
        tris.Add(new Vector3(7,6,0));
        tris.Add(new Vector3(6,9,0));
        tris.Add(new Vector3(11,10,4));
        tris.Add(new Vector3(10,11,6));
        tris.Add(new Vector3(9,5,2));
        tris.Add(new Vector3(5,9,11));
        tris.Add(new Vector3(8,7,1));
        tris.Add(new Vector3(7,8,10));
        tris.Add(new Vector3(2,5,3));
        tris.Add(new Vector3(8,1,3));
        tris.Add(new Vector3(9,2,0));
        tris.Add(new Vector3(1,7,0));
        tris.Add(new Vector3(11,9,6));
        tris.Add(new Vector3(7,10,6));
        tris.Add(new Vector3(5,11,4));
        tris.Add(new Vector3(10,8,4));


        for (int i = 0; i < frequency; i++)
        {
            List<Vector3> tris2 = new List<Vector3>();
            foreach (var tri in tris)
            {
                // replace triangle by 4 triangles
                int a1 = getMiddlePointIndex((int)tri.x, (int)tri.y, positions);
                int b1 = getMiddlePointIndex((int)tri.y, (int)tri.z, positions);
                int c1 = getMiddlePointIndex((int)tri.z, (int)tri.x, positions);

                tris2.Add(new Vector3(tri.x, a1, c1));
                tris2.Add(new Vector3(tri.y, b1, a1));
                tris2.Add(new Vector3(tri.z, c1, b1));
                tris2.Add(new Vector3(a1, b1, c1));

            }
            tris = tris2;
            if (true) {
                for (int j = 0; j < vertCount2; j++) {
                    Vector3 tem = (Vector3)positions[j];
                    tem = Vector3.Normalize(tem) * radius;
                    positions[j] = tem;
                }
            }
        }
        
        // the new dual mesh
        List<Vector3> tmp = new List<Vector3>();

        Vector3[] faceVerts = new Vector3[tris.Count];
        Vector3[] sideTris = new Vector3[tris.Count];

        List<int>[] trisOfVert = new List<int>[vertCount2];
        for (int i = 0; i < trisOfVert.Length; i++) { trisOfVert[i] = new List<int>(); }

        // for each face add the centroid to the dual mesh
        int f = 0;
        foreach (var tri in tris) {
            faceVerts[f] = GetCenterTriangle(positions[(int)tri.x], positions[(int)tri.y], positions[(int)tri.z]);
            sideTris[f] = GetSideTris(tri, tris);
            trisOfVert[(int)tri.x].Add(f);
            trisOfVert[(int)tri.y].Add(f);
            trisOfVert[(int)tri.z].Add(f);
            f++;
        }
        for (int i = 0; i < vertCount2; i++) { midFacePoints[i] = positions[i]; }
        for (int i = 0; i < vertCount2; i++) {
            List<Vector3> vertices = new List<Vector3>();
            
            foreach (var face in trisOfVert[i]) {
                vertices.Add(faceVerts[face]);
            }
            List<int> v = OrderVertices(i, vertices, sideTris, trisOfVert);

            vertices = new List<Vector3>();
            foreach (var ve in v)
            {
                vertices.Add(faceVerts[ve]);
            }
            AddFace(tmp, vertices, positions, normals, texCoords);           
        }
        Debug.Log(trisCount);
        Debug.Log(tmp.Count);
        tris = tmp;

        for (int i = 0; i < tris.Count; i++)
        {
            triangleIndices[0 + i * 3] = (UInt32)tris[i].x;
            triangleIndices[1 + i * 3] = (UInt32)tris[i].y;
            triangleIndices[2 + i * 3] = (UInt32)tris[i].z;
        }

        meshData.subMeshCount = 1;
        meshData.SetSubMesh(0, new SubMeshDescriptor(0, trisCount));

        var mesh = new Mesh
        {
            name = "Procedural Mesh"
        };

        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);

        gObj.GetComponent<MeshFilter>().mesh = mesh;
        gObj.GetComponent<MeshCollider>().sharedMesh = mesh;

        GridPlanet = this.Grid;
        trisIndexPlanet = this.trisIndex;
        tileTypeArrayPlanet = this.tileTypeArray;
    }

    // return index of point in the middle of p1 and p2
    private int getMiddlePointIndex(int p1, int p2, NativeArray<Vector3> positions)
    {
        // first check if we have it already
        bool firstIsSmaller = p1 < p2;
        Int64 smallerIndex = firstIsSmaller ? p1 : p2;
        Int64 greaterIndex = firstIsSmaller ? p2 : p1;
        Int64 key = (smallerIndex << 32) + greaterIndex;

        int ret;
        if (middlePointIndexCache.TryGetValue(key, out ret))
        {
            return ret;
        }


        // not in cache, calculate it
        Vector3 point1 = positions[p1];
        Vector3 point2 = positions[p2];
        Vector3 middle = new Vector3(
            (point1.x + point2.x) / 2.0f,
            (point1.y + point2.y) / 2.0f,
            (point1.z + point2.z) / 2.0f);

        //The golden ratio
        float t = (1 + Mathf.Sqrt(5)) / 2;

        //middle.Normalize();
        //middle *= radius;

        index++;
        // add vertex to positions array
        positions[index] = middle;

        // store it, return index
        middlePointIndexCache.Add(key, index);
        return index;
    }

    Vector3 GetCenterTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        return new Vector3((v1.x+v2.x+v3.x)/3, (v1.y + v2.y + v3.y)/3, (v1.z + v2.z + v3.z)/3);
    }

    void AddFace(List<Vector3> tmp, List<Vector3> vertices, NativeArray<Vector3> positions, NativeArray<Vector3> normals, NativeArray<Vector2> texCoords) {

        int colorIndex = 0;
        float noise = PerlinNoise.Perlin3D(vertices[0].x*1.5f / radius, vertices[0].y *1.5f / radius, vertices[0].z*1.5f / radius);
        noise += 0.5f;
        if (noise < 0.35) { colorIndex = 0; }
        else if(noise < 0.65) { colorIndex = 1; }
        else if(noise < 0.8) { colorIndex = 2; }
        else { colorIndex = 3; }

        tileTypeArray[indexFace] = colorIndex;

        Vector2 colorUV = Vector2.zero;
        switch(colorIndex)
        {
            case 0:
                colorUV = new Vector2(0,0);
                break;
            case 1:
                colorUV = new Vector2(0,1);
                break;
            case 2:
                colorUV = new Vector2(1,0);
                break;
            case 3:
                colorUV = new Vector2(1,1);
                break;
        }        
        if (vertices.Count == 5)
        {
            int triIndex = tmp.Count;
            for (int i = 0; i < 3; i++) {
                trisIndex[triIndex + i] = indexFace;
                Grid[indexFace].Add(triIndex + i);
            }
            
            positions[indexPos] = vertices[0];
            positions[indexPos + 1] = vertices[1];
            positions[indexPos + 2] = vertices[2];
            positions[indexPos + 3] = vertices[3];
            positions[indexPos + 4] = vertices[4];

            Vector3 vec1 = positions[indexPos + 1] - positions[indexPos];
            Vector3 vec2 = positions[indexPos + 4] - positions[indexPos];
            Vector3 cross = Vector3.Cross(vec1, vec2);
            vec1 = (Vector3)positions[indexPos] - gameObject.transform.position;

            bool up = true;
            up = (Vector3.Dot(vec1, cross) > 0);

            if (up)
            {
                //if (colorIndex == 3) for (int i = 0; i < 5; i++) { normals[indexPos + i] = (vertices[i] - gameObject.transform.position).normalized; }
                //else normals[indexPos] = normals[indexPos+1] = normals[indexPos+2] = normals[indexPos+3] = normals[indexPos+4] = Vector3.Normalize(cross);
                for (int i = 0; i < 5; i++) { normals[indexPos + i] = (vertices[i] - gameObject.transform.position).normalized; }
                tmp.Add(new Vector3(indexPos, indexPos + 1, indexPos + 2));
                tmp.Add(new Vector3(indexPos + 2, indexPos + 3, indexPos + 4));
                tmp.Add(new Vector3(indexPos + 4, indexPos, indexPos + 2));
            }
            else {
                //if (colorIndex == 3) for (int i = 0; i < 5; i++) { normals[indexPos + i] = (vertices[i] - gameObject.transform.position).normalized; }
                //else normals[indexPos] = normals[indexPos+1] = normals[indexPos+2] = normals[indexPos+3] = normals[indexPos+4] = Vector3.Normalize(-cross);
                for (int i = 0; i < 5; i++) { normals[indexPos + i] = (vertices[i] - gameObject.transform.position).normalized; }
                tmp.Add(new Vector3(indexPos+2, indexPos + 1, indexPos));
                tmp.Add(new Vector3(indexPos + 4, indexPos + 3, indexPos + 2));
                tmp.Add(new Vector3(indexPos + 2, indexPos, indexPos + 4));
            }

            texCoords[indexPos] = colorUV;
            texCoords[indexPos + 1] = colorUV;
            texCoords[indexPos + 2] = colorUV;
            texCoords[indexPos + 3] = colorUV;
            texCoords[indexPos + 4] = colorUV;
            indexPos += 5;
        }
        if (vertices.Count == 6)
        {
            int triIndex = tmp.Count;
            for (int i = 0; i < 4; i++) {
                trisIndex[triIndex + i] = indexFace;
                Grid[indexFace].Add(triIndex + i);
            }

            positions[indexPos] = vertices[0];
            positions[indexPos + 1] = vertices[1];
            positions[indexPos + 2] = vertices[2];
            positions[indexPos + 3] = vertices[3];
            positions[indexPos + 4] = vertices[4];
            positions[indexPos + 5] = vertices[5];

            Vector3 vec1 = positions[indexPos + 1] - positions[indexPos];
            Vector3 vec2 = positions[indexPos + 4] - positions[indexPos];
            Vector3 cross = Vector3.Cross(vec1, vec2);
            vec1 = (Vector3)positions[indexPos] - gameObject.transform.position;

            bool up = true;
            up = (Vector3.Dot(vec1, cross) > 0);

            if (up)
            {
                //if (colorIndex == 3) for (int i = 0; i < 6; i++) { normals[indexPos + i] = (vertices[i] - gameObject.transform.position).normalized; }
                //else normals[indexPos] = normals[indexPos + 1] = normals[indexPos + 2] = normals[indexPos + 3] = normals[indexPos + 4] = normals[indexPos + 5] = Vector3.Normalize(cross);
                for (int i = 0; i < 6; i++) { normals[indexPos + i] = (vertices[i] - gameObject.transform.position).normalized; }
                tmp.Add(new Vector3(indexPos, indexPos + 1, indexPos + 2));
                tmp.Add(new Vector3(indexPos, indexPos + 2, indexPos + 5));
                tmp.Add(new Vector3(indexPos + 2, indexPos + 3, indexPos + 5));
                tmp.Add(new Vector3(indexPos + 3, indexPos + 4, indexPos + 5));
            }
            else
            {
                //if (colorIndex == 3) for (int i = 0; i < 6; i++) { normals[indexPos + i] = (vertices[i] - gameObject.transform.position).normalized; }
                //else normals[indexPos] = normals[indexPos + 1] = normals[indexPos + 2] = normals[indexPos + 3] = normals[indexPos + 4] = normals[indexPos + 5] = Vector3.Normalize(-cross);
                for (int i = 0; i < 6; i++) { normals[indexPos + i] = (vertices[i] - gameObject.transform.position).normalized; }
                tmp.Add(new Vector3(indexPos + 2, indexPos + 1, indexPos));
                tmp.Add(new Vector3(indexPos + 5, indexPos + 2, indexPos));
                tmp.Add(new Vector3(indexPos + 5, indexPos + 3, indexPos + 2));
                tmp.Add(new Vector3(indexPos + 5, indexPos + 4, indexPos + 3));
            }

            texCoords[indexPos] = colorUV; 
            texCoords[indexPos+1] = colorUV;
            texCoords[indexPos+2] = colorUV;
            texCoords[indexPos+3] = colorUV; 
            texCoords[indexPos+4] = colorUV;
            texCoords[indexPos + 5] = colorUV;
            indexPos += 6;
        }
        indexFace++;
    }

    Vector3 GetSideTris(Vector3 tri, List<Vector3> tris)
    {
        Vector3 vec = new Vector3();

        int v1 = (int)tri.x;
        int v2 = (int)tri.y;
        int v3 = (int)tri.z;

        // first check if we have it already
        bool firstIsSmaller = v1 < v2;
        long smallerIndex = firstIsSmaller ? v1 : v2;
        long greaterIndex = firstIsSmaller ? v2 : v1;
        long key = (smallerIndex << 32) + greaterIndex;
        //Debug.Log(key);
        int indexTri = 0;
        foreach (var triangle in tris)
        {
            if (triangle != tri)
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
        foreach (var triangle in tris)
        {
            if (triangle != tri)
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
        foreach (var triangle in tris)
        {
            if (triangle != tri)
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
        return vec;
    }

    List<int> OrderVertices(int i, List<Vector3> vertices , Vector3[] sideTris, List<int>[] trisOfVert)
    {
        List<int> order = new List<int>()
        {
            trisOfVert[i][0]
        };

        int antTri = trisOfVert[i][0];
        int m = 0;
        while (order.Count < vertices.Count)
        {
            for (int a = 0; a < 3; a++) {              
                if (trisOfVert[i].Contains((int)sideTris[antTri][a]) && !order.Contains((int)sideTris[antTri][a]))
                {
                    
                    order.Add((int)sideTris[antTri][a]);
                    antTri = (int)sideTris[antTri][a];
                }
            }
        }
        return order;
    }

    void GetSideFaces() {
        
    }

    void OnDisable()
    {
        Destroy(gameObject.GetComponent<MeshFilter>().mesh);
    }

    private void OnApplicationQuit()
    {
        Destroy(gameObject.GetComponent<MeshFilter>().mesh);
        Destroy(gameObject);
    }
}
