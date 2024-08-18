using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

public class ChunkGrid : MonoBehaviour
{
    public static ChunkGrid ChunkGridObj;
    public float VoxelSize;
    public int CubicChunkSize;
    public float MountainHeight;
    public float GroundHeight;
    [HideInInspector] public Vector3Int ChunkSize;
    public Vector3Int GridSize;
    private GameObject[,,] GridObj;
    public float CellScale;
    public GameObject MeshPrefab;
    public float Compression;
    public float Density;
    public bool debugMesh;
    public float PerlinNoiseInfluence;
    public float perlinPower;

    public float M_MountainHeight;
    public float M_BaseHeight;
    public float M_scale;
    public generationType GType;
    public MeshGenerator currentChunk;
    public GameObject test;

    public enum generationType
    {
        SNoise,
        PNoise,
        Combine,
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
    public void outlineChunks()
    {
        for (int x = 0; x < GridObj.GetLength(0); x++)
        {
            for (int y = 0; y < GridObj.GetLength(1); y++)
            {
                for (int z = 0; z < GridObj.GetLength(2); z++)
                {
                        MeshGenerator MG = GridObj[x, y, z].GetComponent<MeshGenerator>();
                        Vector3 center = GridObj[x, y, z].transform.position + ((Vector3.one * VoxelSize * (CubicChunkSize - 1)) / 2);
                        MG.chunkBounds = new MeshGenerator.WorldBounds(center, center);
                    for (int i = 0; i < cornerPos.Length; i++)
                    {
                        Vector3 currentCornerPos = (Vector3)cornerPos[i] * (VoxelSize * (CubicChunkSize / 2));
                        Vector3 pos = center + currentCornerPos;
                        if (pos.x > MG.chunkBounds.maxBounds.x) MG.chunkBounds.maxBounds.x = pos.x;
                        if (pos.y > MG.chunkBounds.maxBounds.y) MG.chunkBounds.maxBounds.y = pos.y;
                        if (pos.z > MG.chunkBounds.maxBounds.z) MG.chunkBounds.maxBounds.z = pos.z;

                        if (pos.x < MG.chunkBounds.minBounds.x) MG.chunkBounds.minBounds.x = pos.x;
                        if (pos.y < MG.chunkBounds.minBounds.y) MG.chunkBounds.minBounds.y = pos.y;
                        if (pos.z < MG.chunkBounds.minBounds.z) MG.chunkBounds.minBounds.z = pos.z;

                    }
       
                }
            }
        }
    }
   
    private void Awake()
    {
        if(ChunkGridObj == null)
        {
            ChunkGridObj = this;
        }
        else
        {
            Destroy(gameObject);
        }
        GridObj = new GameObject[GridSize.x, GridSize.y, GridSize.x];
    }
    private void Start()
    {
        
        GenerateGrid();
    }
    private MeshGenerator TryFindChunk()
    {
        MeshGenerator res = null;
        if (GameManager.instance.playerRef == null) return res;
        for (int x = 0; x < GridObj.GetLength(0); x++)
        {
            for (int y = 0; y < GridObj.GetLength(1); y++)
            {
                for (int z = 0; z < GridObj.GetLength(2); z++)
                {
                    MeshGenerator targetChunk = GridObj[x, y, z].GetComponent<MeshGenerator>();
                    Vector3 PPos = GameManager.instance.playerRef.transform.position;
                    if (PPos.x > targetChunk.chunkBounds.maxBounds.x || PPos.y > targetChunk.chunkBounds.maxBounds.y || PPos.z > targetChunk.chunkBounds.maxBounds.z)
                    {
                        continue;
                    }
                    else if (PPos.x < targetChunk.chunkBounds.minBounds.x || PPos.y < targetChunk.chunkBounds.minBounds.y || PPos.z < targetChunk.chunkBounds.minBounds.z)
                    {
                        continue;
                    }
                    res = targetChunk;
                    break;
                }
            }
        }
        return res;
    }
    private void Update()
    {
        if (debugMesh)
        {
          GenerateGrid();
            debugMesh = false;
        }
        if (currentChunk != null)
        {
            if (GameManager.instance.playerRef.transform.position.x > currentChunk.chunkBounds.maxBounds.x || GameManager.instance.playerRef.transform.position.y > currentChunk.chunkBounds.maxBounds.y || GameManager.instance.playerRef.transform.position.z > currentChunk.chunkBounds.maxBounds.z)
            {

                currentChunk = TryFindChunk();
            }
            else if (GameManager.instance.playerRef.transform.position.x < currentChunk.chunkBounds.minBounds.x || GameManager.instance.playerRef.transform.position.y < currentChunk.chunkBounds.minBounds.y || GameManager.instance.playerRef.transform.position.z < currentChunk.chunkBounds.minBounds.z)
            {
                currentChunk = TryFindChunk();

            }
        }


    }
    public void GenerateGrid()
    {
        CellScale = CubicChunkSize * VoxelSize;
        ChunkSize = Vector3Int.one * CubicChunkSize;
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
                    GridObj[x, y, z].GetComponent<MeshGenerator>().CreateShape();
                }
            }
        }
        outlineChunks();
       currentChunk = TryFindChunk();
    }
 
    public Vector3 GetWorldPos(Vector3Int _cellpos)
    {
        return (Vector3)_cellpos * CellScale;
    }



}
