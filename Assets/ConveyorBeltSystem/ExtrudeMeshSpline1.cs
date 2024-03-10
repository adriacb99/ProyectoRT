using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class ExtrudeMeshSpline : MonoBehaviour
{
    private SplineContainer container;
    private Spline spline;

    [SerializeField] private Mesh meshToExtrude;
    [SerializeField] private int divisiones = 1;

    private MeshFilter meshFilter;
    Mesh mesh;

    public LayerMask layerMask;
    private float3[] position;
    private float3[] upVector;
    private float3[] tangent;

    Vector3[] tempVert;

    // Start is called before the first frame update
    void Awake()
    {    
        position = new float3[divisiones];
        upVector = new float3[divisiones];
        tangent = new float3[divisiones];

        container = gameObject.GetComponent<SplineContainer>();
        spline = container.Splines[0];

        mesh = ExtrudeMesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private Mesh ExtrudeMesh()
    {
        Mesh meshE = new Mesh
        {
            name = "Extruded Mesh"
        };

        for (int i = 0; i < divisiones; i++)
        {
            float3 p;
            float3 t;
            float3 up;
            container.Evaluate(0, (float)i / divisiones, out p, out t, out up);
            position[i] = p;
            tangent[i] = t;
            upVector[i] = up;
        }

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> tris = new List<int>();


        GetSideEdgesIndices(meshToExtrude, out int[] ForwardEdge, out int[] BackEdge);
        Vector3[] tempVert = JuntarEdges(meshToExtrude);

        for (int i = 0; i < position.Length - 1; i++)
        {
            Vector3[] newVertices = new Vector3[tempVert.Length];
            Vector3[] newNormals = new Vector3[meshToExtrude.normals.Length];

            Quaternion rotation = Quaternion.LookRotation(
            new Vector3(tangent[i].x, 0, tangent[i].z), Vector3.up);

            foreach (var index in ForwardEdge) {
                newVertices[index] = (rotation * tempVert[index]) + (Vector3)position[i];
            }

            rotation = Quaternion.LookRotation(
            new Vector3(tangent[i+1].x, 0, tangent[i+1].z), Vector3.up);

            foreach (var index in BackEdge) {
                newVertices[index] = (rotation * tempVert[index]) + (Vector3)position[i+1];
            }

            int prevVerticiesLength = vertices.Count;

            vertices.AddRange(newVertices);
            normals.AddRange(newNormals);

            for (int a = 0; a < meshToExtrude.vertices.Length; a++) {
                tris.Add(prevVerticiesLength + a);
            }
         
            uv.AddRange(meshToExtrude.uv);
        }

        meshE.vertices = vertices.ToArray();
        meshE.normals = normals.ToArray();
        meshE.uv = uv.ToArray();
        meshE.triangles = tris.ToArray();

        return meshE;
    }

    private void GetSideEdgesIndices(Mesh meshToExtrude, out int[] forward, out int[] back)
    {
        List<int> ForwardIndicies = new List<int>();
        List<int> BackIndicies = new List<int>();
        Vector3[] vertices = meshToExtrude.vertices;

        for (int i =0; i < vertices.Length; i++)
        {
            if (vertices[i].z < 0) ForwardIndicies.Add(i);
            else BackIndicies.Add(i);
        }

        forward = ForwardIndicies.ToArray();
        back = BackIndicies.ToArray();
    }

    private Vector3[] JuntarEdges(Mesh meshToExtrude)
    {
        Vector3[] vertices = meshToExtrude.vertices;
        Vector3[] temp = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; ++i)
        {
            temp[i] = Vector3.Scale(vertices[i], new Vector3(1, 1, 0));
        }
        return temp;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        for (int i = 0; i < mesh.vertices.Length; i++) {
            Gizmos.DrawSphere(mesh.vertices[i], 0.1f);
        }
    }
}
