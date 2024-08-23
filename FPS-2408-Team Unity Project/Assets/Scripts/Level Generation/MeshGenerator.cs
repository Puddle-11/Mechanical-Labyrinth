using Palmmedia.ReportGenerator.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.AI;
using static UnityEngine.GraphicsBuffer;
using System.Reflection;
using Unity.AI.Navigation;


[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    private ChunkGrid chunkRef;
    private List<Vector3> Verticies;
    private List<int> Triangles;
    private List<Vector2> UVs;
    private MeshFilter mainMesh;
    private MeshCollider colliderMesh;

    //add world to cell function for break and place functions
    public Vector3[] VertexPos = new Vector3[]
    {
        //Top Vertex
        new Vector3(-1, 1, -1),new Vector3(-1, 1, 1),
        new Vector3(1, 1, 1),new Vector3(1, 1, -1),
        //Bottom Vertex
        new Vector3(-1, -1, -1),new Vector3(-1, -1, 1),
        new Vector3(1, -1, 1),new Vector3(1, -1, -1)
    };
    public void SetChunkRef(ChunkGrid _val)
    {
        chunkRef = _val;
    }
    public void UpdateShape()
    {
        mainMesh = GetComponent<MeshFilter>();
        colliderMesh = GetComponent<MeshCollider>();
        Triangles = new List<int>();
        Verticies = new List<Vector3>();
        UVs = new List<Vector2>();
        GenerateFaces();
    }









    private void GenerateFaces()
    {
        for (int x = 0; x < chunkRef.ChunkSize.x; x++)
        {
            for (int y = 0; y < chunkRef.ChunkSize.y; y++)
            {
                for (int z = 0; z < chunkRef.ChunkSize.z; z++)
                {
                        Vector3Int _GridPos = chunkRef.GetChunkStartPos(gameObject) + new Vector3Int(x,y,z);
                    if (chunkRef.GetTile(_GridPos).ID != 0)
                    {
                        int filteredID =chunkRef.GetTile(_GridPos).ID - 1;
                        #region chunkEdge Draws
                        if (_GridPos.y == 0)
                        {
                            //Render BottomFace
                            AddQuad(7, 5, 6, 4, new Vector3(x, y, z) * chunkRef.VoxelSize, Verticies.Count, 1, filteredID);
                        }
                        if (_GridPos.y == chunkRef.bounds.max.y)
                        {
                            AddQuad(0, 1, 2, 3, new Vector3(x, y, z) * chunkRef.VoxelSize, Verticies.Count, 0, filteredID);
                            //Render TopFace

                        }
                        if (_GridPos.x == 0)
                        {
                            AddQuad(1, 0, 5, 4, new Vector3(x, y, z) * chunkRef.VoxelSize, Verticies.Count, 2, filteredID);

                        }
                        if (_GridPos.x == chunkRef.bounds.max.x)
                        {
                            AddQuad(6, 2, 7, 3, new Vector3(x, y, z) * chunkRef.VoxelSize, Verticies.Count, 4, filteredID);

                        }
                        if (_GridPos.z == 0)
                        {

                            AddQuad(4, 0, 6, 2, new Vector3(x, y, z) * chunkRef.VoxelSize, Verticies.Count, 3, filteredID);
                        }
                        if (_GridPos.z == chunkRef.bounds.max.z)
                        {
                            AddQuad(1, 5, 3, 7, new Vector3(x, y, z) * chunkRef.VoxelSize, Verticies.Count, 5, filteredID);
                        }
                        #endregion
                        #region xSide
                        //------------------------------------------
                        //X Side 
                        if (_GridPos.x < chunkRef.bounds.max.x - 1)
                        {
                            if (chunkRef.GetTile(_GridPos + new Vector3Int(1, 0, 0)).ID == 0)
                            {
                                AddQuad(6, 2, 7, 3, new Vector3(x, y, z) * chunkRef.VoxelSize, Verticies.Count, 4, filteredID);
                            }
                        }
                        if (_GridPos.x > 0)
                        {
                            if (chunkRef.GetTile(_GridPos + new Vector3Int(-1, 0, 0)).ID == 0)
                            {
                                AddQuad(1, 0, 5, 4, new Vector3(x, y, z) * chunkRef.VoxelSize, Verticies.Count, 2, filteredID);

                            }
                        }
                        //------------------------------------------
                        #endregion
                        #region ySide
                        //------------------------------------------
                        //Y Side 
                        if (_GridPos.y < chunkRef.bounds.max.y - 1)
                        {
                            if (chunkRef.GetTile(_GridPos + new Vector3Int(0, 1, 0)).ID == 0)
                            {
                                AddQuad(0, 1, 2, 3, new Vector3(x, y, z) * chunkRef.VoxelSize, Verticies.Count, 0, filteredID);

                            }
                        }
                        if (_GridPos.y > 0)
                        {
                            if (chunkRef.GetTile(_GridPos + new Vector3Int(0, -1, 0)).ID == 0)
                            {
                                AddQuad(7, 5, 6, 4, new Vector3(x, y, z) * chunkRef.VoxelSize, Verticies.Count, 1, filteredID);

                            }
                        }
                        //------------------------------------------
                        #endregion
                        #region zSide
                        //------------------------------------------
                        //Z Side 
                        if (_GridPos.z < chunkRef.bounds.max.z)
                        {
                            if (chunkRef.GetTile(_GridPos + new Vector3Int(0, 0, 1)).ID == 0)
                            {
                                AddQuad(1, 5, 3, 7, new Vector3(x, y, z) * chunkRef.VoxelSize, Verticies.Count, 5, filteredID);

                            }
                        }
                        if (_GridPos.z > 0)
                        {
                            if (chunkRef.GetTile(_GridPos + new Vector3Int(0,0,-1)).ID == 0)
                            {
                                AddQuad(4, 0, 6, 2, new Vector3(x, y, z) * chunkRef.VoxelSize, Verticies.Count, 3, filteredID);

                            }
                        }
                        //------------------------------------------
                        #endregion
                    }
                }
            }
        }
        UpdateMesh();
    }
  
    //Run from generate face, Worst case run 6 times per tile
    private void AddQuad(int _ip1, int _ip2, int _ip4, int _ip3, Vector3 _origin, int _startIndex, int _side, int _ID)
    {
        int fx = _ID % chunkRef.textureAtlasSize;
        int fy = Mathf.FloorToInt(_ID / (float)chunkRef.textureAtlasSize);
        Vector2 UVAnchor = new Vector2(fx, fy) / chunkRef.textureAtlasSize;
        _side = Mathf.Clamp(_side, 0, 5);
        Vector3[] tempVertex = new Vector3[4];
        Vector2[] tempUV = new Vector2[4];
        #region Map UV's
        switch (_side)
        {
            case 0:
                tempUV = new Vector2[] 
                { 
                 UVAnchor + new Vector2(0,0) / (chunkRef.textureAtlasSize),
                 UVAnchor + new Vector2(0,0.5f) / (chunkRef.textureAtlasSize),
                 UVAnchor + new Vector2(0.5f,0.5f) / (chunkRef.textureAtlasSize),
                 UVAnchor + new Vector2(0.5f,0) / (chunkRef.textureAtlasSize)
                };

                break;
            case 1:
                tempUV = new Vector2[]
                     {
                 UVAnchor + new Vector2(0.5f,0.5f) / (chunkRef.textureAtlasSize),
                 UVAnchor + new Vector2(0.5f,1) / (chunkRef.textureAtlasSize),
                UVAnchor + new Vector2(1,1) / (chunkRef.textureAtlasSize),
                 UVAnchor + new Vector2(1,0.5f) / (chunkRef.textureAtlasSize),
                     };
                break;
            case 2:
                tempUV = new Vector2[]
                       {
                 UVAnchor + new Vector2(0,1) / (chunkRef.textureAtlasSize),
                 UVAnchor + new Vector2(0.5f,1) / (chunkRef.textureAtlasSize),
                 UVAnchor + new Vector2(0.5f,0.5f) / (chunkRef.textureAtlasSize),
                 UVAnchor + new Vector2(0,0.5f) / (chunkRef.textureAtlasSize),
                };
                break;
            case 3:
            case 4:
                tempUV = new Vector2[]
                         {
                 UVAnchor + new Vector2(0,0.5f) / (chunkRef.textureAtlasSize),
                 UVAnchor + new Vector2(0,1) / (chunkRef.textureAtlasSize),
                 UVAnchor + new Vector2(0.5f,1) / (chunkRef.textureAtlasSize),
                 UVAnchor + new Vector2(0.5f,0.5f) / (chunkRef.textureAtlasSize),
                         };
                break;
            case 5:
                tempUV = new Vector2[]
                       {
                       UVAnchor + new Vector2(0.5f,1) / (chunkRef.textureAtlasSize),
                       UVAnchor + new Vector2(0.5f,0.5f) / (chunkRef.textureAtlasSize),
                       UVAnchor + new Vector2(0,0.5f) / (chunkRef.textureAtlasSize),
                       UVAnchor + new Vector2(0,1) / (chunkRef.textureAtlasSize),
                       };
                break;

        }
        #endregion
        tempVertex[0] = _origin + (VertexPos[_ip1] * (chunkRef.VoxelSize / 2));
        tempVertex[1] = _origin + (VertexPos[_ip2] * (chunkRef.VoxelSize / 2));
        tempVertex[2] = _origin + (VertexPos[_ip3] * (chunkRef.VoxelSize / 2));
        tempVertex[3] = _origin + (VertexPos[_ip4] * (chunkRef.VoxelSize / 2));
        int[] tempTriangles = new int[] 
        { 
        _startIndex, _startIndex + 1, _startIndex + 3,
        _startIndex + 1, _startIndex + 2, _startIndex + 3,
        };
        Triangles.AddRange(tempTriangles);
        Verticies.AddRange(tempVertex);
        UVs.AddRange(tempUV);
    }
    //Run after all triangels and verticies have been added to the arrays
    private void UpdateMesh()
    {
        Vector2[] tempUV = new Vector2[Verticies.Count];
        for (int i = 0; i < tempUV.Length; i++)
        {
            if(i >= UVs.Count)
            {
                break;
            }
            else
            {
                tempUV[i] = UVs[i];
            }
        }
        gameObject.name = "Mesh Renderer: " + chunkRef.GetChunkPos(gameObject);
        if (Verticies.Count <= 0) Destroy(gameObject);
        mainMesh.mesh.vertices = Verticies.ToArray();
        mainMesh.mesh.triangles = Triangles.ToArray();
        mainMesh.mesh.uv = UVs.ToArray();
        mainMesh.mesh.RecalculateNormals();
        colliderMesh.sharedMesh = mainMesh.mesh;
    }
}
