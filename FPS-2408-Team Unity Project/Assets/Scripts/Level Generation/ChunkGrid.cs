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
    public struct GridBounds
    {
        public Vector3Int min;
        public Vector3Int max;
    }

    private Vector3Int[] cornerPos = new Vector3Int[]
    {
        new Vector3Int(-1,-1,-1),
        new Vector3Int(1,-1,-1),
        new Vector3Int(1,-1,1),
        new Vector3Int(-1,-1,1),
        new Vector3Int(-1,1,-1),
        new Vector3Int(1,1,-1),
        new Vector3Int(1,1,1),
        new Vector3Int(-1,1,1),
    };
    public Vector3Int ChunkToGrid(Vector3Int _blockPos, Vector3Int _chunkPos)
    {
        if (_chunkPos == Vector3Int.one * -1) return Vector3Int.one * -1;
        return new Vector3Int(_blockPos.x + _chunkPos.x * CubicChunkSize, _blockPos.y + _chunkPos.y * CubicChunkSize, _blockPos.z + _chunkPos.z * CubicChunkSize);
    }
    public Vector3Int ChunkToGrid(Vector3Int _blockPos, GameObject _chunk)
    {
        return ChunkToGrid(_blockPos, GetChunkPos(_chunk));
    }
    public Vector3Int GetChunkPos(GameObject _chunk)
    {
        for (int x = 0; x < GridObj.GetLength(0); x++)
        {
            for (int y = 0; y < GridObj.GetLength(1); y++)
            {
                for (int z = 0; z < GridObj.GetLength(2); z++)
                {
                    if (GridObj[x, y,z] == _chunk)
                    {
                        return new Vector3Int(x,y,z);
                    }
                }
            }
        }
        return Vector3Int.one * -1;
    }
    public int PlaceTile(Vector3Int _pos)
    {
        iGen.SetGeneratorBounds(bounds);
        return iGen.PlaceTile(_pos);
    }
    public int PlaceTile(Vector3Int _pos, GameObject _chunk)
    {
        return PlaceTile(ChunkToGrid(_pos, _chunk));
    }
    private void Awake()
    {
        GridObj = new GameObject[GridSize.x, GridSize.y, GridSize.x];
    }
    private void Start()
    {
        
        GenerateGrid();
    }
    public void GenerateGrid()
    {
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
                    if (GridObj[x,y,z] != null)
                    {
                        Destroy(GridObj[x, y, z]);
                    }
                    GridObj[x, y, z] = Instantiate(MeshPrefab, transform.position, Quaternion.identity, transform);
                    GridObj[x, y, z].transform.position = GetWorldPos(new Vector3Int(x, y, z));
                    GridObj[x, y, z].GetComponent<MeshGenerator>().SetChunkRef(this);
                    GridObj[x, y, z].GetComponent<MeshGenerator>().CreateShape();
                }
            }
        }

    }
 
    public Vector3 GetWorldPos(Vector3Int _cellpos)
    {
        return (Vector3)_cellpos * CellScale;
    }



}
