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
    [SerializeField] private float divisiones = 1f;

    private MeshFilter meshFilter;
    Mesh mesh;

    public LayerMask layerMask;
    private float3 position;
    private float3 upVector;
    private float3 tangent;

    Vector3[] tempVert;

    // Start is called before the first frame update
    void Awake()
    {
        container = GetComponent<SplineContainer>();
        spline = container.Splines[0];

        meshFilter = GetComponent<MeshFilter>();
        mesh = ExtrudeMesh();
        {
            name = "Extruded Mesh";
        };
        meshFilter.mesh = mesh;
    }

    private Mesh ExtrudeMesh()
    {
        Mesh mesh = new Mesh();

        //ontainer.Evaluate(1, (float)i / divisiones, out position, out tangent, out upVector);
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> tris = new List<int>();


        GetSideEdgesIndices(meshToExtrude, out int[] ForwardEdge, out int[] BackEdge);
        Vector3[] tempVert = JuntarEdges(meshToExtrude);

        for (int i = 0; i < divisiones; i++)
        {
            container.Evaluate(1, (float)i / divisiones, out position, out tangent, out upVector);
        }

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = tris.ToArray();

        return mesh;
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

    private void JuntarMeshes(List<Vector3> vertices, List<int> triangles, List<Vector3> normals, List<Vector2> uvs,
        Vector3 firstPos, Vector3 firstTangent,
        Vector3 secondPos, Vector3 secondTangent,
        int[] firstEdgeIndicies, int[] secondEdgeIndicies)
    {
        Vector3[] newVertices = new Vector3[tempVert.Length];
        Vector3[] newNormals = new Vector3[meshToExtrude.normals.Length];

    }
}
