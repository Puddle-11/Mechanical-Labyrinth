using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

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
    private MeshCell[,,] Gridcells;

    public struct GridBounds
    {
        public Vector3Int min;
        public Vector3Int max;
    }

    private void Awake()
    {
        GridObj = new GameObject[GridSize.x, GridSize.y, GridSize.x];
    }
    private void Start()
    {
        InstantiateGrid();
        GenerateGrid();
        RenderGrid();
    }
  

    #region MainGeneration
    public void InstantiateGrid()
    {
        //This method takes all the values given and instantiates a grid of chunks to work with
        bounds.min = Vector3Int.zero;
        GridObj = new GameObject[GridSize.x, GridSize.y, GridSize.z];
        CellScale = CubicChunkSize * VoxelSize;
        ChunkSize = Vector3Int.one * CubicChunkSize;
        bounds.max = GridSize * CubicChunkSize - Vector3Int.one;

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
                    GridObj[x, y, z].GetComponent<MeshGenerator>().InitializeGrid();
                }
            }
        }
    }
    private void GenerateGrid()
    {
        //This method simply updates the internal grid values, it does not propogate these values to the renderer
        for (int x = 0; x < GridObj.GetLength(0) * ChunkSize.x; x++)
        {
            for (int y = 0; y < GridObj.GetLength(1) * ChunkSize.y; y++)
            {
                for (int z = 0; z < GridObj.GetLength(2) * ChunkSize.z; z++)
                {
                    Vector3Int _pos = new Vector3Int(x, y, z);
                    Vector3Int _chunkPos = GetChunkPos(_pos);
                    MeshGenerator meshGenRef;
                    if (GridObj[_chunkPos.x, _chunkPos.y, _chunkPos.z].TryGetComponent<MeshGenerator>(out meshGenRef))
                    {

                        meshGenRef.UpdateCell(GridToChunk(_pos), PlaceTile(_pos));
                    }
                }
            }
        }
    }

    private void RenderGrid()
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
                    }
                }
            }
        }

    }
    #endregion

    #region Convertions
    private Vector3Int ChunkToGrid(Vector3Int _blockPos, Vector3Int _chunkPos)
    {
        if (_chunkPos == Vector3Int.one * -1) return Vector3Int.one * -1;
        return new Vector3Int(_blockPos.x + _chunkPos.x * CubicChunkSize, _blockPos.y + _chunkPos.y * CubicChunkSize, _blockPos.z + _chunkPos.z * CubicChunkSize);
    }
    private Vector3Int ChunkToGrid(Vector3Int _blockPos, GameObject _chunk)
    {
        return ChunkToGrid(_blockPos, GetChunkPos(_chunk));
    }

    private Vector3Int GridToChunk(Vector3Int _pos)
    {
        int x = _pos.x % ChunkSize.x;
        int y = _pos.y % ChunkSize.y;
        int z = _pos.z % ChunkSize.z;
        return new Vector3Int(x, y, z);
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
    public Vector3Int GetChunkPos(Vector3Int _gridPos)
    {
        int x = Mathf.FloorToInt(_gridPos.x / ChunkSize.x);
        int y = Mathf.FloorToInt(_gridPos.y / ChunkSize.y);
        int z = Mathf.FloorToInt(_gridPos.z / ChunkSize.z);

        //Debug.Log("Global Position: " + _gridPos + "\nChunk Pos: " + new Vector3Int(x, y, z));

        if (x >= GridObj.GetLength(0) || y >= GridObj.GetLength(1) || z >= GridObj.GetLength(2)) return Vector3Int.one * -1;

        return new Vector3Int(x, y, z);
    }

    public Vector3 ChunkToWorld(Vector3Int _gridPos)
    {
        //converts a GRID POSITION to a WORLD POSITION
        return (Vector3)_gridPos * CellScale;
    }
    public Vector3 ChunkToWorld(Vector3Int _chunkBlockPos, Vector3Int _chunkPos)
    {
        return ChunkToWorld(ChunkToGrid(_chunkBlockPos, _chunkPos));
    }
    #endregion

    #region HelperFunctions
    public int PlaceTile(Vector3Int _pos)
    {
        //returns what a tile at a GLOBAL POSITION should be
        iGen.SetGeneratorBounds(bounds);
        return iGen.PlaceTile(_pos);
    }
    public int PlaceTile(Vector3Int _chunkBlockPos, GameObject _chunk)
    {
        //returns what a tile at a CHUNK POSITION should be
        return PlaceTile(ChunkToGrid(_chunkBlockPos, _chunk));
    }

    #endregion
}
