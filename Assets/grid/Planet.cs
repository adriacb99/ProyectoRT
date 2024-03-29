using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using static UnityEditor.Searcher.SearcherWindow.Alignment;


public class Planet : MonoBehaviour
{
    [SerializeField] bool construir = false;
    [SerializeField] bool gancho = false;
    [SerializeField] bool cinta = false;
    public bool canEdit = true;


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
    [SerializeField] ConstructionData[] objetosPrueba;
    [SerializeField] Mineral[] mineralesDisponibles;

    [SerializeField] AtmosphereSettings atm;
    [SerializeField] Material atmMaterial;


    [System.Serializable]
    public struct Mineral {
        public ConstructionData mineral;
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
        MoveWaterTiles();
        //GenerateGrass();
    }

    private void MoveWaterTiles()
    {
        Vector3[] vertices = new Vector3[mesh.vertexCount];
        mesh.vertices.CopyTo(vertices, 0);

        for (int i = 0; i < Grid.Length; i++)
        {
            Tile tile = grid.GetValue(Grid[i][0]);
            if (tile.type == (TileType)3)
            {
                List<int> tris = tile.GetTrisIndex();
                List<Tile> side = tile.GetSideTiles();
                foreach (int index in tris)
                {
                    vertices[mesh.triangles[index * 3]] = vertices[mesh.triangles[index * 3]].normalized * 9.85f;
                    vertices[mesh.triangles[index * 3 + 1]] = vertices[mesh.triangles[index * 3 + 1]].normalized * 9.85f;
                    vertices[mesh.triangles[index * 3 + 2]] = vertices[mesh.triangles[index * 3 + 2]].normalized * 9.85f;

                    foreach (var s in side)
                    {
                        if (s.type != (TileType)3) {
                            List<int> sTris = s.GetTrisIndex();
                            foreach (int sIndex in sTris)
                            {
                                for (int a = 0; a < 3; a++)
                                {
                                    if (mesh.vertices[mesh.triangles[index * 3 + a]].Equals(vertices[mesh.triangles[sIndex * 3]])) vertices[mesh.triangles[sIndex * 3]] = vertices[mesh.triangles[sIndex * 3]].normalized * 9.85f;
                                    else if (mesh.vertices[mesh.triangles[index * 3 + a]].Equals(vertices[mesh.triangles[sIndex * 3 + 1]])) vertices[mesh.triangles[sIndex * 3 + 1]] = vertices[mesh.triangles[sIndex * 3 + 1]].normalized * 9.85f;
                                    else if (mesh.vertices[mesh.triangles[index * 3 + a]].Equals(vertices[mesh.triangles[sIndex * 3 + 2]])) vertices[mesh.triangles[sIndex * 3 + 2]] = vertices[mesh.triangles[sIndex * 3 + 2]].normalized * 9.85f;
                                }
                            }
                        }
                    }
                }               
            }
        }
        mesh.vertices = vertices;
    }

    public HexGrid<Tile> GetPlanetGrid()
    {
        return grid;
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

        //public BezierKnot knot;
        public Construction tileObject;
        public TileType type;

        public Tile(HexGrid<Tile> grid, Vector3 localPosition, List<int> tris, int tileTypeIndex) 
        {
            this.tris = tris;
            this.grid = grid;
            this.localPosition = localPosition;
            this.type = (TileType)tileTypeIndex;
        }

        public void SetObject(Construction tileObject)
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

        public List<Construction> GetSideTilesObjects()
        {
            List<Construction> result = new List<Construction>();
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

    public HexGrid<Tile> GetGrid()
    {
        return grid;
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

                Construction T_T = Construction.PlaceConstruction(tile, this.transform, mineralesDisponibles[b].mineral, tile.GetPosition(), Quaternion.LookRotation((tile.GetPosition() - gameObject.transform.position).normalized));
                tile.tileObject = T_T;

                List<Tile> sideTiles = tile.GetSideTiles();
                for (int i = 0; i < 3; i++)
                {
                    if (sideTiles[i].CanBuild()) {
                        Construction owo = ConstructionMineral.PlaceConstruction(sideTiles[i], this.transform, mineralesDisponibles[b].mineral, sideTiles[i].GetPosition(), Quaternion.LookRotation((sideTiles[i].GetPosition() - gameObject.transform.position).normalized));
                        sideTiles[i].tileObject = owo;
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
        Vector3 n = o - transform.position;
        n = n.normalized;

        float ux = n.y;
        float uy = -n.x;
        float uz = (-n.x * ux - n.y * uy) / (n.z == 0 ? 1 : n.z);

        float vx = n.y * uz - n.z * uy;
        float vy = n.z * ux - n.x * uz;
        float vz = n.x * uy - n.y * ux;

        Vector3 uVec = new Vector3(ux, uy, uz).normalized;
        Vector3 vVec = new Vector3(vx, vy, vz).normalized;

        for (int i = 0; i < 8; i++)
        {
            var radians = 2 * Mathf.PI / 8 * i;

            var spawnDir = Vector3.zero;

            spawnDir.x = o.x + 0.5f * (vVec.x * Mathf.Cos(radians) + uVec.x * Mathf.Sin(radians));
            spawnDir.y = o.y + 0.5f * (vVec.y * Mathf.Cos(radians) + uVec.y * Mathf.Sin(radians));
            spawnDir.z = o.z + 0.5f * (vVec.z * Mathf.Cos(radians) + uVec.z * Mathf.Sin(radians));


            Ray ray = new Ray(spawnDir, transform.position - spawnDir);

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
            Debug.Log(hit.point + " " + grid.GetValue(hit.triangleIndex).GetPosition());

            oldUV.CopyTo(newUV, 0);
            Tile tile = grid.GetValue(hit.triangleIndex);

            // Visuals tile on cursor

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

            // Contruction

            mesh.uv = newUV;
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.green);

            if (Input.GetMouseButtonDown(0) && construir && canEdit)
            {
                Debug.Log(tile.ToString());
                if (tile.CanBuild())
                {
                    Construction T_T = ContructionMining.PlaceConstruction(tile, transform, objetosPrueba[0], transform.TransformPoint(tile.GetPosition()), Quaternion.LookRotation(hit.normal));
                    tile.SetObject(T_T);
                }
                else if (tile.tileObject.CompareTag("Construction"))
                {
                }
            }
            else if (Input.GetMouseButtonDown(0) && gancho && canEdit)
            {
                Debug.Log(tile.ToString());
                if (tile.CanBuild())
                {
                    Construction T_T = Hook.PlaceConstruction(tile, transform, objetosPrueba[1], transform.TransformPoint(tile.GetPosition()), Quaternion.LookRotation(hit.normal));
                    tile.SetObject(T_T);
                }
                else if (tile.tileObject.CompareTag("Construction"))
                {
                }
            }
            else if (Input.GetMouseButtonDown(0) && cinta && canEdit)
            {
                if (tile.CanBuild())
                {

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

        //3160

        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            Vector3 o = transform.TransformPoint(grid.GetValue(hit.triangleIndex).GetPosition());
            Vector3 n = o - transform.position;
            n = n.normalized;

            Debug.Log(n);
            Debug.DrawRay(Vector3.zero, n * 100, Color.green);

            // Calcular circulo perpendicular a la direccion de la Tile central

            float ux = n.y;
            float uy = -n.x;
            float uz = (-n.x * ux - n.y * uy) / ( n.z == 0 ? 1 : n.z);

            float vx = n.y * uz - n.z * uy;
            float vy = n.z * ux - n.x * uz;
            float vz = n.x * uy - n.y * ux;

            Vector3 uVec = new Vector3(ux, uy, uz).normalized;
            Vector3 vVec = new Vector3(vx, vy, vz).normalized;

            Debug.DrawRay(o, new Vector3(ux, uy, uz).normalized, Color.green);
            Debug.DrawRay(o, new Vector3(vx, vy, vz).normalized, Color.green);


            for (int i = 0; i < 8; i++)
            {
                var radians = 2 * Mathf.PI / 8 * i;

                var spawnDir = Vector3.zero;

                spawnDir.x = o.x + 0.5f * (vVec.x * Mathf.Cos(radians) + uVec.x * Mathf.Sin(radians));
                spawnDir.y = o.y + 0.5f * (vVec.y * Mathf.Cos(radians) + uVec.y * Mathf.Sin(radians));
                spawnDir.z = o.z + 0.5f * (vVec.z * Mathf.Cos(radians) + uVec.z * Mathf.Sin(radians));

                ray = new Ray(spawnDir*1.2f, transform.position - spawnDir);

                Debug.DrawRay(Vector3.zero, spawnDir - transform.position, Color.green);
            }
        }
    }
}
