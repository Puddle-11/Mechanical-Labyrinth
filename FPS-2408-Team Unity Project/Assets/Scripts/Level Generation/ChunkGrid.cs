using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.XR;

public class ChunkGrid : MonoBehaviour
{
    public float VoxelSize;
    public int CubicChunkSize;

    [HideInInspector] public Vector3Int ChunkSize;
    public Vector3Int GridSize;
    private GameObject[,,] GridObj;
    private float CellScale;
    public GameObject MeshPrefab;
    public IGenerator iGen;
    public GridBounds bounds;
    public int textureAtlasSize;
    private MeshCell[,,] GridCells;
    private delegate void ChunkGenProgress();
    private ChunkGenProgress StartLoad;
    private ChunkGenProgress EndLoad;
    public struct GridBounds
    {
        public Vector3Int min;
        public Vector3Int max;
    }
    private void Awake()
    {
        GridObj = new GameObject[GridSize.x, GridSize.y, GridSize.x];
    }
    private void OnEnable()
    {
        BootLoadManager.instance.sceneChangeEvent += SpawnEndDoor;
    }
    private void OnDisable()
    {
        BootLoadManager.instance.sceneChangeEvent -= SpawnEndDoor;

    }
    private void Start()
    {

        bounds.min = Vector3Int.zero;
        bounds.max = GridSize * CubicChunkSize - Vector3Int.one;
        iGen.SetGeneratorBounds(bounds);
        iGen.GenerateMap();
        Vector2Int UPos = iGen.GetStartingPoint();
        Vector3Int FPos = new Vector3Int(UPos.x, 2, UPos.y);

        //FAULTY CODE
        //============================
        if (GameManager.instance != null)
        {
            if (GameManager.instance.playerControllerRef != null)
            {
                GameManager.instance.playerControllerRef.SetPlayerSpawnPos(GridToWorld(FPos));
                GameManager.instance.playerControllerRef.spawnPlayer();
            }
        }
            //============================
            InstantiateGrid();
        GenerateGrid();
        StartCoroutine(RenderGrid());

    }

    private void SpawnEndDoor()
    {
        Vector2Int UPos = iGen.GetStartingPoint();
        Vector3Int _gridPos = new Vector3Int(UPos.x, 2, UPos.y);
        Instantiate(iGen.EndDoorPrefab, GridToWorld(_gridPos) + new Vector3(0, 0, -2), Quaternion.LookRotation(new Vector3(0,0,1)));
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
                        meshGenRef.UpdateShape();
                        yield return null;
                    }
                }
            }
        }
    }


    #endregion

    #region Convertions
    private Vector3 GridToWorld(Vector3Int _pos)
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
    private Vector3Int GetChunkPos(GameObject _chunk)
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
