using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;


public class Planet : MonoBehaviour
{
    [SerializeField] int size;
    [SerializeField] float radius;
    public LayerMask layerMask;

    private HexGrid<Tile> grid;
    private Mesh mesh;
    private List<int>[] Grid;
    private int[] trisIndex;
    private int[] tileTypeArray;
    private Vector3[] midFacePoints;

    // PRUEBAS
    Vector2[] newUV;
    Vector2[] oldUV;
    int[] triangles;
    [SerializeField] GameObject objetoPrueba;
    [SerializeField] Mineral[] mineralesDisponibles;

    [SerializeField] AtmosphereSettings atm;
    [SerializeField] Material atmMaterial;


    [System.Serializable]
    public struct Mineral {
        public GameObject prefab;
        public int cantidad;
    }

    private void Awake()
    {
        ProceduralPlanetGeneration.Instance.CreatePlanet(radius, this.gameObject, size, out Grid, out trisIndex, out midFacePoints, out tileTypeArray);
        grid = new HexGrid<Tile>(Grid.Length, (HexGrid<Tile> grid, Vector3 localPosition, List<int> tris, int tileTypeIndex) => new Tile(grid, localPosition, tris, tileTypeIndex), midFacePoints, trisIndex, Grid, tileTypeArray);
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.RecalculateBounds();

        triangles = mesh.triangles;
        oldUV = mesh.uv;
        newUV = new Vector2[oldUV.Length];

        // Buscar casillas de alrededor
        for (int i= 0; i < Grid.Length; i++)
        {
            Tile tile = grid.GetValue(Grid[i][0]);
            List<Tile> tiles = GetTiles(tile);
            tile.SideTiles(tiles);
        }
        GenerateMinerals();
    }

    public enum TileType
    {
        WATER = 3,
        SAND = 2,
        GRASS = 1,
        FOREST = 0
    }

    public class Tile
    {
        private HexGrid<Tile> grid;
        private Vector3 localPosition;
        private List<Tile> sideTiles;
        private List<int> tris;

        public GameObject tileObject;
        public TileType type;

        public Tile(HexGrid<Tile> grid, Vector3 localPosition, List<int> tris, int tileTypeIndex) 
        {
            this.tris = tris;
            this.grid = grid;
            this.localPosition = localPosition;
            this.type = (TileType)tileTypeIndex;
        }

        public void SetObject(GameObject tileObject)
        {
            this.tileObject = tileObject;
        }

        public void ClearObject()
        {
            Destroy(tileObject);
            this.tileObject = null;
        }

        public bool CanBuild()
        {
            return tileObject == null;
        }

        public void SideTiles(List<Tile> sideTiles)
        {
            this.sideTiles = sideTiles;
        }

        public List<Tile> GetSideTiles()
        {
            return sideTiles;
        }

        public List<int> GetTrisIndex()
        {
            return tris;
        }

        public List<GameObject> GetSideTilesObjects()
        {
            List<GameObject> result = new List<GameObject>();
            foreach (Tile tile in sideTiles) {
                result.Add(tile.tileObject);
            }
            return result;
        }

        public override string ToString()
        {
            return "Posicion Tile: " + localPosition + tileObject + type.ToString();
        }

        public Vector3 GetPosition() { return localPosition; }
    }

    void GenerateMinerals()
    {
        for (int b = 0; b < mineralesDisponibles.Length; b++) {
            for (int a = 0; a < mineralesDisponibles[b].cantidad; a++)
            {
                int index = Random.Range(0, Grid.Length);
                Tile tile = grid.GetValue(Grid[index][b]);
                while (tile.type != TileType.FOREST && tile.type != TileType.GRASS && tile.CanBuild()) {
                    index = Random.Range(0, Grid.Length);
                    tile = grid.GetValue(Grid[index][b]);
                }

                GameObject T_T = Instantiate(mineralesDisponibles[b].prefab, tile.GetPosition(), Quaternion.identity);
                T_T.transform.parent = transform;
                tile.SetObject(T_T);

                List<Tile> sideTiles = tile.GetSideTiles();
                for (int i = 0; i < 3; i++)
                {
                    if (sideTiles[i].CanBuild()) {
                        GameObject owo = Instantiate(mineralesDisponibles[b].prefab, sideTiles[i].GetPosition(), Quaternion.identity);
                        owo.transform.parent = transform;
                        sideTiles[i].SetObject(owo);
                    }

                    // List<Tile> sideTiles2 = sideTiles[i].GetSideTiles();
                    // for (int j = 0; j < 3; j++)
                    // {
                    //     if (sideTiles2[j].CanBuild())
                    //     {
                    //         GameObject uwu = Instantiate(mineralesDisponibles[0].prefab, sideTiles2[j].GetPosition(), Quaternion.identity);
                    //         uwu.transform.parent = transform;
                    //         sideTiles2[j].SetObject(uwu);
                    //     }
                    // }
                }
            }
        }
    }

    List<Tile> GetTiles(Tile tile)
    {
        List<Tile> tiles = new List<Tile>();
        RaycastHit hit;

        Vector3 o = transform.TransformPoint(tile.GetPosition());
        Vector3 n = o - gameObject.transform.position;
        n = n.normalized;

        // Calcular circulo perpendicular a la direccion de la Tile central
        float s = 1.0f / (n.x * n.x + n.z * n.z);
        float v1x = s * n.z;
        float v1y = 0.0f;
        float v1z = s * -n.x;

        float v2x = n.y * v1z - n.z * v1y;
        float v2y = n.z * v1x - n.x * v1z;
        float v2z = n.x * v1y - n.y * v1x;

        for (int i = 0; i < 8; i++)
        {
            var radians = 2 * Mathf.PI / 8 * i;

            var spawnDir = Vector3.zero;

            spawnDir.x = o.x + 0.5f * (v1x * Mathf.Cos(radians) + v2x * Mathf.Sin(radians));
            spawnDir.y = o.y + 0.5f * (v1y * Mathf.Cos(radians) + v2y * Mathf.Sin(radians));
            spawnDir.z = o.z + 0.5f * (v1z * Mathf.Cos(radians) + v2z * Mathf.Sin(radians));


            Ray ray = new Ray(spawnDir, transform.position - spawnDir*1.1f);

            if (Physics.Raycast(ray, out hit, 5f, layerMask))
            {
                Tile temp = grid.GetValue(hit.triangleIndex);
                if (!tiles.Contains(temp)) tiles.Add(temp);
            }           
        }
        return tiles;
    }

    private void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            oldUV.CopyTo(newUV, 0);
            Tile tile = grid.GetValue(hit.triangleIndex);

            int inu = trisIndex[hit.triangleIndex];
            List<int> jeje = Grid[inu];
            foreach (var vert in jeje)
            {
                for (int i = 0; i < 3; i++)
                {
                    newUV[triangles[vert * 3 + i]] = new Vector2(1, 1);
                }
            }
            List<Tile> m = tile.GetSideTiles();
            foreach (var t in m) {
                List<int> fff = t.GetTrisIndex();
                foreach (var vert in fff)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        newUV[triangles[vert * 3 + i]] = new Vector2(1, 1);
                    }
                }
            }

            mesh.uv = newUV;
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.green);

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log(tile.ToString());
                if (tile.CanBuild())
                {
                    GameObject T_T = ContructionMining.PlaceConstruction(transform, objetoPrueba, transform.TransformPoint(tile.GetPosition()), tile.GetSideTilesObjects(), Quaternion.LookRotation(hit.normal));
                    tile.SetObject(T_T);
                }
                else if (tile.tileObject.CompareTag("Construction")) {
                    tile.tileObject.GetComponent<ContructionMining>().ShowMenu();
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Debug.Log(tile.ToString());
                if (!tile.CanBuild())
                {
                    tile.ClearObject();
                }
            }
        }
        if (Input.anyKeyDown) atm.SetProperties(atmMaterial, 10);


        // Vector3 o = transform.TransformPoint(grid.GetValue(66).GetPosition());
        // Vector3 n = o - gameObject.transform.position;
        // n = n.normalized;

        // // Calculate v1.
        // float s = 1.0f / (n.x * n.x + n.z * n.z);
        // float v1x = s * n.z;
        // float v1y = 0.0f;
        // float v1z = s * -n.x;

        // float v2x = n.y * v1z - n.z * v1y;
        // float v2y = n.z * v1x - n.x * v1z;
        // float v2z = n.x * v1y - n.y * v1x;


        // Vector3 v = new Vector3(0, n.z, n.y);


        // Debug.DrawLine(gameObject.transform.position, o, Color.green);
        // for (int i = 0; i < 8; i++)
        // {
        //     var radians = 2 * Mathf.PI / 8 * i;

        //     var spawnDir = Vector3.zero;

        //     /* Get the vector direction */
        //     spawnDir.x = o.x + 0.5f * (v1x * Mathf.Cos(radians) + v2x * Mathf.Sin(radians));
        //     spawnDir.y = o.y + 0.5f * (v1y * Mathf.Cos(radians) + v2y * Mathf.Sin(radians));
        //     spawnDir.z = o.z + 0.5f * (v1z * Mathf.Cos(radians) + v2z * Mathf.Sin(radians));

        //     Debug.DrawLine(gameObject.transform.position, spawnDir, Color.green);
        // }
    }
}
