using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class Testing : MonoBehaviour
{
    public Camera camera;
    public Mesh mesh;
    public GameObject goPrefab;
    public LayerMask layerMask;

    bool createMesh = false;
    Transform ant;
    GameObject obj;

    Vector2[] newUV;
    int[] triangles;

    List<int>[] Grid;
    int[] trisIndex;

    Vector2[] oldUV;

    Planet planet;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        triangles = mesh.triangles;
        trisIndex = ProceduralPlanetGeneration.Instance.trisIndex;
        Grid = ProceduralPlanetGeneration.Instance.Grid;
        oldUV = mesh.uv;
        newUV = new Vector2[oldUV.Length];
        planet = GetComponent<Planet>();
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            oldUV.CopyTo(newUV, 0);
            int inu = trisIndex[hit.triangleIndex];
            List<int> jeje = Grid[inu];
            foreach (var vert in jeje)
            {
                for (int i = 0; i < 3; i++)
                {
                    newUV[triangles[vert * 3 + i]] = new Vector2(1, 1);
                }
            }
            mesh.uv = newUV;
            Debug.DrawLine(camera.transform.position, hit.point, Color.green);          
        } 
        else createMesh = false;
    }
}