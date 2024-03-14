using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using static UnityEditor.Progress;

public class ExtrudeMeshSpline : MonoBehaviour
{
    [SerializeField] private SplineContainer container;
    private Spline spline;

    [SerializeField] private Mesh meshToExtrude;
    [SerializeField] private int divisiones = 1;
    [SerializeField] private float escala = 1;
    [SerializeField] float segmentLength = 1;

    [SerializeField] Material material;

    private MeshFilter meshFilter;
    Mesh mesh;

    public LayerMask layerMask;
    private float3[] position;
    private float3[] upVector;
    private float3[] tangent;

    Vector3[] tempVert;

    [SerializeField] Transform planet;
    private HexGrid<Planet.Tile> grid;

    // Start is called before the first frame update
    void Awake()
    {
        position = new float3[divisiones + 1];
        upVector = new float3[divisiones + 1];
        tangent = new float3[divisiones + 1];
    }

    private void Start()
    {
        spline = container.AddSpline();
        grid = planet.GetComponent<Planet>().GetPlanetGrid();

        divisiones = (int)(container.Splines[1].GetLength() / segmentLength);

        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        mesh = new Mesh
        {
            name = "Mesh Cinta Transportadora"
        };

        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        gameObject.GetComponent<MeshRenderer>().material = material;
    }

    private void ExtrudeMesh()
    {
        position = new float3[divisiones + 1];
        upVector = new float3[divisiones + 1];
        tangent = new float3[divisiones + 1];

        for (int i = 0; i <= divisiones; i++)
        {
            float3 p;
            float3 t;
            float3 up;
            container.Evaluate(1, (float)i / divisiones, out p, out t, out up);
            position[i] = p;
            Vector3 tem = (Vector3)position[i];
            tem = Vector3.Normalize(tem) * 10;
            position[i] = tem;


            tangent[i] = t;
            upVector[i] = position[i] - (float3)planet.position;
            upVector[i] = Vector3.Normalize(upVector[i]);
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
            new Vector3(tangent[i].x, tangent[i].y, tangent[i].z), upVector[i]);

            foreach (var index in BackEdge) {
                newVertices[index] = (rotation * tempVert[index]) + (Vector3)position[i];
                newNormals[index] = rotation * meshToExtrude.normals[index];
            }

            rotation = Quaternion.LookRotation(
            new Vector3(tangent[i+1].x, tangent[i + 1].y, tangent[i+1].z), upVector[i+1]);

            foreach (var index in ForwardEdge) {
                newVertices[index] = (rotation * tempVert[index]) + (Vector3)position[i+1];
            }

            int prevVerticiesLength = vertices.Count;

            vertices.AddRange(newVertices);
            normals.AddRange(newNormals);

            for (int a = 0; a < meshToExtrude.triangles.Length/3; a++) {
                tris.Add(prevVerticiesLength + meshToExtrude.triangles[a*3]);
                tris.Add(prevVerticiesLength + meshToExtrude.triangles[a * 3 + 1]);
                tris.Add(prevVerticiesLength + meshToExtrude.triangles[a * 3 + 2]);
            }
         
            uv.AddRange(meshToExtrude.uv);
        }

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = tris.ToArray();
    }

    private void GetSideEdgesIndices(Mesh meshToExtrude, out int[] forward, out int[] back)
    {
        List<int> ForwardIndicies = new List<int>();
        List<int> BackIndicies = new List<int>();
        Vector3[] vertices = meshToExtrude.vertices;

        for (int i =0; i < vertices.Length; i++)
        {
            if (vertices[i].z > 0) ForwardIndicies.Add(i);
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
            temp[i] = Vector3.Scale(vertices[i], new Vector3(1*escala, 1*escala/4, 0));
        }
        return temp;
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 100f, layerMask))
            {
                Planet.Tile tile = grid.GetValue(hit.triangleIndex);

                List<BezierKnot> knots = spline.Knots.ToList();

                BezierKnot knot;
                knot = new BezierKnot(tile.GetPosition());
                       
                knots.Add(knot);

                if (knots.Count > 1) {
                    Quaternion knotRotation = Quaternion.LookRotation((Vector3)knots[1].Position - (Vector3)knots[0].Position, (Vector3)knots[1].Position - gameObject.transform.position);
                    knots[0] = new BezierKnot(knots[0].Position, new Vector3(0, 0, -0.3f), new Vector3(0, 0, 0.3f), knotRotation);

                    for (int i = 1; i < knots.Count - 1; ++i)
                    {
                        knotRotation = Quaternion.LookRotation(
                            (Vector3)knots[i + 1].Position - (Vector3)knots[i - 1].Position, 
                            (Vector3)knots[i].Position - gameObject.transform.position
                            );
                        knots[i] = new BezierKnot(knots[i].Position, new Vector3(0, 0, -0.3f), new Vector3(0, 0, 0.3f), knotRotation);
                    }
                    knotRotation = Quaternion.LookRotation((Vector3)knots[knots.Count - 1].Position - (Vector3)knots[knots.Count - 2].Position, (Vector3)knots[knots.Count - 1].Position - gameObject.transform.position);
                    knots[knots.Count - 1] = new BezierKnot(knots[knots.Count - 1].Position, new Vector3(0, 0, -0.3f), new Vector3(0, 0, 0.3f), knotRotation);
                }
                spline.Knots = knots;
            }
            divisiones = (int)(container.Splines[1].GetLength() / segmentLength);
            if (spline.Count > 1) ExtrudeMesh();
        }
    }
}
