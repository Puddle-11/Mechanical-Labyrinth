using System.Collections;
using Unity.AI.Navigation;

using UnityEngine;

public class ChunkGrid : MonoBehaviour
{
    public float VoxelSize;
    public int CubicChunkSize;
    public static ChunkGrid instance;
    [HideInInspector] public Vector3Int ChunkSize;
    [SerializeField] private Vector3Int minGridSize;
    public Vector3Int GridSize;
    private GameObject[,,] GridObj;
    private float CellScale;
    public GameObject MeshPrefab;
    public IGenerator iGen;
    public GridBounds bounds;
    public int textureAtlasSize;
    private MeshCell[,,] GridCells;
    public delegate void ChunkGenProgress();
    public ChunkGenProgress StartLoad;
    public ChunkGenProgress EndLoad;
    public float progress;
    private int chunkLoaded;
    private int totalChunks;
   
    [SerializeField] private NavMeshSurface navMeshSurfaceRef;
    public Texture2D GetRoomTexture()
    {
        return iGen.GetRoomTexture();
    }
    public struct GridBounds
    {
        public Vector3Int min;
        public Vector3Int max;
    }
    public struct WorldBounds
    {
        public Vector3 min;
        public Vector3 max;
    }
    private void Awake()
    {
        if(instance == null) instance = this;
        else
        {
            Debug.LogWarning("Two Chunk Grids found in one scene, please ensure there is only ever one");
            Destroy(this);
        }
        GridObj = new GameObject[GridSize.x, GridSize.y, GridSize.x];
    }
    private void OnEnable()
    {
        BootLoadManager.instance.stopLoadEvent += SpawnEndDoor;
        BootLoadManager.instance.startLoadEvent += removeListeners;
    }
    private void OnDisable()
    {
        removeListeners();
    }
    private void removeListeners() //Since the chunk grid changes from scene to scene we need to make sure we remove the listeners before the next scene is loaded
    {

        BootLoadManager.instance.stopLoadEvent -= SpawnEndDoor;
    }
    private void Start()
    {
        GridSize *= GameManager.instance.GetCurrentLevel();
        GridSize.x = Mathf.Clamp(GridSize.x, minGridSize.x, int.MaxValue);
        GridSize.y = Mathf.Clamp(GridSize.y, minGridSize.y, int.MaxValue);
        GridSize.z = Mathf.Clamp(GridSize.z, minGridSize.z, int.MaxValue);

        bounds.min = Vector3Int.zero;
        bounds.max = GridSize * CubicChunkSize - Vector3Int.one;
        iGen.SetGeneratorBounds(bounds);
        iGen.GenerateMap();


        //FAULTY CODE
        //============================
        if (GameManager.instance != null)
        {
            if (GameManager.instance.playerControllerRef != null)
            {
                GameManager.instance.playerControllerRef.SetPlayerSpawnPos(GetStartingPos());
                GameManager.instance.playerControllerRef.spawnPlayer();
            }
        }
            //============================
            InstantiateGrid();
        GenerateGrid();
        StartCoroutine(RenderGrid());

    }
    public Vector3 GetStartingPos()
    {
        Vector2Int UPos = iGen.GetStartingPoint();
        return GridToWorld( new Vector3Int(UPos.x, 2, UPos.y));
    }
    private void SpawnEndDoor()
    {
        Vector2Int UPos = iGen.GetStartingPoint();
        Vector3Int _gridPos = new Vector3Int(UPos.x, 2, UPos.y);
        Instantiate(iGen.EndDoorPrefab, GridToWorld(_gridPos) + iGen.endDoorOffset, Quaternion.LookRotation(new Vector3(0,0,1)));
    }

    #region MainGeneration
    private void InstantiateGrid()
    {
        //This method takes all the values given and instantiates a grid of chunks to work with
        GridObj = new GameObject[GridSize.x, GridSize.y, GridSize.z];
        CellScale = CubicChunkSize * VoxelSize;
        ChunkSize = Vector3Int.one * CubicChunkSize;

        for (int x = 0; x < GridObj.GetLength(0); x++)
        {
            for (int y = 0; y < GridObj.GetLength(1); y++)
            {
                for (int z = 0; z < GridObj.GetLength(2); z++)
                {
                    if (GridObj[x, y, z] != null) Destroy(GridObj[x, y, z]);
                    GridObj[x, y, z] = Instantiate(MeshPrefab, transform.position, Quaternion.identity, transform);
                    GridObj[x, y, z].transform.position = ChunkToWorld(new Vector3Int(x, y, z));
                    GridObj[x, y, z].GetComponent<MeshGenerator>().SetChunkRef(this);
                }
            }
        }
    }
    private void GenerateGrid()
    {
        GridCells = new MeshCell[GridObj.GetLength(0) * CubicChunkSize, GridObj.GetLength(1) * CubicChunkSize, GridObj.GetLength(2) * CubicChunkSize];

        for (int x = 0; x < GridObj.GetLength(0) * CubicChunkSize; x++)
        {
            for (int y = 0; y < GridObj.GetLength(1) * CubicChunkSize; y++)
            {
                for (int z = 0; z < GridObj.GetLength(2) * CubicChunkSize; z++)
                {
                    if (GridCells[x, y, z] == null) GridCells[x, y, z] = new MeshCell();
                    Vector3Int _pos = new Vector3Int(x, y, z);

                    GridCells[x, y, z].ID = PlaceTile(_pos);
                }
            }
        }
    }

    private IEnumerator RenderGrid()
    {
        totalChunks = GridObj.GetLength(0) * GridObj.GetLength(1) * GridObj.GetLength(2);
        //This method takes the internal grid values and updates the renderer acordingly 
        for (int x = 0; x < GridObj.GetLength(0); x++)
        {
            for (int y = 0; y < GridObj.GetLength(1); y++)
            {
                for (int z = 0; z < GridObj.GetLength(2); z++)
                {

                    Vector3Int _chunkPos = new Vector3Int(x,y,z);

                    MeshGenerator meshGenRef;
                    if (GridObj[_chunkPos.x, _chunkPos.y, _chunkPos.z].TryGetComponent<MeshGenerator>(out meshGenRef))
                    {
                        chunkLoaded++;
                        progress = chunkLoaded / (float)totalChunks;
                        yield return null;
                        meshGenRef.UpdateShape();
                    }
                }
            }
        }
        yield return null;
        navMeshSurfaceRef.BuildNavMesh();
        yield return null;
        EndLoad?.Invoke();
    }


    #endregion

    #region Convertions
    public Vector3Int WorldToGrid(Vector3 _pos)
    {
        Vector3 normalPos = transform.position - _pos;
        return new Vector3Int(Mathf.CeilToInt(normalPos.x / VoxelSize), Mathf.CeilToInt(normalPos.y / VoxelSize), Mathf.CeilToInt(normalPos.z / VoxelSize));
    }

    public Vector3 GridToWorld(Vector3Int _pos)
    {
        return transform.position + (Vector3)_pos * VoxelSize;
    }
    public Vector3Int GetChunkStartPos(GameObject _chunkObj)
    {
       return GetChunkStartPos(GetChunkPos(_chunkObj));
    }
    private Vector3Int GetChunkStartPos(Vector3Int _chunkPos)
    {
        return new Vector3Int(_chunkPos.x * CubicChunkSize, _chunkPos.y * CubicChunkSize,  _chunkPos.z * CubicChunkSize);
    }
    public Vector3Int GetChunkPos(GameObject _chunk)
    {
        for (int x = 0; x < GridObj.GetLength(0); x++)
        {
            for (int y = 0; y < GridObj.GetLength(1); y++)
            {
                for (int z = 0; z < GridObj.GetLength(2); z++)
                {
                    if (GridObj[x, y, z] == _chunk)
                    {
                        return new Vector3Int(x, y, z);
                    }
                }
            }
        }
        return Vector3Int.one * -1;
    }


    private Vector3 ChunkToWorld(Vector3Int _gridPos)
    {
        //converts a GRID POSITION to a WORLD POSITION
        return (Vector3)_gridPos * CellScale;
    }
    #endregion

    #region HelperFunctions
    public MeshCell GetTile(Vector3Int _pos)
    {
        //Debug.Log(_pos);
        return GridCells[_pos.x, _pos.y, _pos.z];
    }
    private int PlaceTile(Vector3Int _pos)
    {
        //returns what a tile at a GLOBAL POSITION should be
        return iGen.PlaceTile(_pos);
    }

    #endregion
}
