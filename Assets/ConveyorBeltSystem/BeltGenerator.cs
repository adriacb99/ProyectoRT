using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.Splines;

public class BeltGenerator : MonoBehaviour
{
    [SerializeField] SplineContainer container;
    Spline spline;

    [SerializeField] Transform planet;
    private HexGrid<Planet.Tile> grid;

    public float anchura = 0;
    public int divisiones = 1;

    public LayerMask layerMask;
    private float3 position;
    private float3 upVector;
    private float3 tangent;

    Mesh mesh;

    Vector3[] newVertices;
    Vector3[] newNormals;
    int[] newTriangles;


    // Start is called before the first frame update
    void Start()
    {
        spline = container.AddSpline();
        grid = planet.GetComponent<Planet>().GetPlanetGrid();

        mesh = new Mesh {
            name = "Conveyor Belt"
        };

        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();


        gameObject.GetComponent<MeshFilter>().mesh = mesh;
    }

    // Update is called once per frame
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

                spline.Knots = knots;
            }
        }
        Debug.Log(spline.Count);
        if (spline.Count > 1) EvaluarSpline();
    }

    private void EvaluarSpline()
    {
        List<Vector3> vert = new List<Vector3>();
        List<Vector3> norm = new List<Vector3>();
        List<int> tris = new List<int>();

        newNormals = new Vector3[divisiones * 2];
        //newTriangles = new int[divisiones * 2];

        for (int i = 0; i <= divisiones; i++)
        {
            container.Evaluate(1, (float)i / divisiones, out position, out tangent, out upVector);
            upVector = position - (float3)planet.position;
            upVector = Vector3.Normalize(upVector);

            float3 point = Vector3.Cross(tangent, upVector).normalized;
            float3 derecha = (point * anchura) + position;
            float3 izquerda = (-point * anchura) + position;

            vert.Add(derecha);
            vert.Add(izquerda);
            norm.Add(upVector);
            norm.Add(upVector);

            if (i != 0)
            {
                tris.Add(i * 2);
                tris.Add(i * 2 + 1);               
                tris.Add((i - 1) * 2);
               
                tris.Add((i - 1) * 2 + 1);
                tris.Add((i - 1) * 2);
                tris.Add(i * 2 + 1);
            }
            DrawPathVertex(derecha, izquerda);
        }
        mesh.vertices = vert.ToArray();
        mesh.normals = norm.ToArray();
        mesh.triangles = tris.ToArray();
    }

    private void DrawPathVertex(float3 derecha, float3 izquerda)
    {
        Debug.DrawLine(derecha, derecha + upVector / 3, Color.green);
        Debug.DrawLine(izquerda, izquerda + upVector / 3, Color.green);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawSphere(planet.TransformPoint(position), 0.1f);
    //}
}
