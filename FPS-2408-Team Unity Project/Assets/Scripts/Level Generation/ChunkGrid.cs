using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
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
    private void InstantiateGrid()
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
                }
            }
        }
    }
    private void GenerateGrid()
    {
        GridCells = new MeshCell[GridObj.GetLength(0) * CubicChunkSize, GridObj.GetLength(1) * CubicChunkSize, GridObj.GetLength(2) * CubicChunkSize];
        Debug.Log("Grid Size: " + new Vector3Int( GridCells.GetLength(0), GridCells.GetLength(1), GridCells.GetLength(2)));
        //This method simply updates the internal grid values, it does not propogate these values to the renderer
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
        iGen.SetGeneratorBounds(bounds);
        return iGen.PlaceTile(_pos);
    }

    #endregion
}
