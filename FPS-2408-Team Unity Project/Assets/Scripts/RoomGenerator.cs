using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class RoomGenerator : IGenerator
{
    [SerializeField] private LayerMask groundLayer;
    public GenerationType generatorType;
    [HideInInspector] public int maxHeight; //This value will cap out at bounds height 
    //Specific to FromTexture && FromGenerator
    [HideInInspector] public Sprite roomMap;
    //Specific to FromVariables
    [HideInInspector] public int RoomSize;
     public int DoorHeight;
    [HideInInspector] public int DoorWidth;
    [HideInInspector] public int maxRopeAnchorDistance;
    [HideInInspector] public GameObject ropePrefab;
    [HideInInspector] public int numberOfRopes;
    [HideInInspector] public int numberOfRopeAnchors;
    [HideInInspector] public Vector2 ropeLength;
    //Specific to FromGenerator
    [HideInInspector] public int maxNumOfPrimaryRooms;
    [HideInInspector] public int maxNumOfSecondaryRooms;
    [HideInInspector] public int ceilingHeight;
    [HideInInspector] public float minRoomDist;
    [HideInInspector] public Vector2Int minRoomSize;
    [HideInInspector] public Vector2Int maxRoomSize;
    //Global
    [HideInInspector] public int baseBoardHeight;
    [HideInInspector] public int topPlateHeight;
    [HideInInspector] public int floorBlockID;
    [HideInInspector] public int wallBlockID;
    [HideInInspector] public int ceilingBlockID;
    [HideInInspector] public int baseBoardBlockID;
    [HideInInspector] public int topPlaceBlockID;
    [HideInInspector] public Texture2D roomTexture;

    private Color roomCol;
    private Vector2Int startPos;
    public override Texture2D GetRoomTexture()
    {
        return roomTexture;
    }
    private struct RoomMarker
    {
        public Vector2Int R_Size;
        public Vector2Int R_Pos;
    }
    private ChunkGrid.GridBounds bounds;
    public enum GenerationType
    {
        FromTexture,
        FromAlgorithm,
    }
    public void Start()
    {
        
        ChunkGrid.instance.EndLoad += GenerateDecorations;
    }

    private void OnDisable()
    {
        ChunkGrid.instance.EndLoad -= GenerateDecorations;

    }
    public override void SetGeneratorBounds(ChunkGrid.GridBounds _bounds)
    {
        bounds = _bounds;
    }
    public override Vector2Int GetStartingPoint()
    {
        return startPos;
    }
    public override void GenerateMap()
    {
        roomCol = new Color((float)ceilingHeight / 10, (float)ceilingHeight / 10, (float)ceilingHeight / 10);
        if (generatorType == GenerationType.FromTexture)
        {
            roomTexture = roomMap.texture;
            return;
        }

        roomTexture = new Texture2D(bounds.max.x + 1, bounds.max.z + 1);
        //================================
        //Set Texture to all black
        for (int x = 0; x < roomTexture.Size().x; x++)
        {
            for (int y = 0; y < roomTexture.Size().y; y++)
            {
                roomTexture.SetPixel(x,y,Color.black);
            }
        }
        //================================

        //================================
        //Draw Rooms
        RoomMarker[] Rooms = GeneratePrimaryRooms(roomTexture);
        startPos = Rooms[0].R_Pos;
        for (int i = 0; i < Rooms.Length; i++)
        {
            Vector2Int offset = new Vector2Int((Rooms[i].R_Size.x / 2), (Rooms[i].R_Size.y / 2));
            for (int x = 0; x < Rooms[i].R_Size.x; x++)
            {
                for (int y = 0; y < Rooms[i].R_Size.y; y++)
                {
                    roomTexture.SetPixel(Rooms[i].R_Pos.x + x - offset.x, Rooms[i].R_Pos.y + y - offset.y, roomCol);
                }
            }
        }
        
       
        //================================

        GenerateSecondaryRooms(roomTexture, 2);

        for (int i = 0; i < Rooms.Length; i++)
        {
            int nextRoom = i == Rooms.Length - 1 ? 0 : i + 1;
            DrawDoubleHallway(Rooms[i].R_Pos, Rooms[nextRoom].R_Pos, DoorWidth, DoorHeight);
        }
        //================================
        
        roomTexture.Apply();
        for (int x = 0; x < roomTexture.width; x++)
        {
            roomTexture.SetPixel(x, roomTexture.height - 1, Color.black);
            roomTexture.SetPixel(x, 0, Color.black);
        }
        for (int y = 0; y < roomTexture.width; y++)
        {
            roomTexture.SetPixel(roomTexture.height - 1, y, Color.black);
            roomTexture.SetPixel(0, y, Color.black);
        }
        //miniMap = GetComponent<Image>();
        //Debug.Log("got component");
        //miniMap.sprite = roomMap;
        //Debug.Log("Not set");
    }
    private void GenerateDecorations()
    {
        roomTexture.wrapMode = TextureWrapMode.Clamp;
        List<Vector2Int> allPositions = new List<Vector2Int>();
        for (int x = 0; x < roomTexture.width; x++)
        {
            for (int y = 0; y < roomTexture.height; y++)
            {
               

                if (roomTexture.GetPixel(x, y) != Color.black)
                {
                    allPositions.Add(new Vector2Int(x, y));
                }
            }
        }
        for (int i = 0; i < numberOfRopes; i++)
        {
            int randIndex = Random.Range(0, allPositions.Count);
            Vector2Int startPos = allPositions[randIndex];


            List<Vector3> anchorPositions = new List<Vector3>();
            Color temp = roomTexture.GetPixel(startPos.x, startPos.y);
            int greyCol = Mathf.RoundToInt((float)temp.grayscale * (float)maxHeight);
            Vector3 rayoriginPos = ChunkGrid.instance.GridToWorld(new Vector3Int(startPos.x, greyCol, startPos.y)) + new Vector3(0, -0.5f, 0);
            for (int j = 0; j < numberOfRopeAnchors; j++)
            {
                Vector3 raycastDir = Random.insideUnitSphere.normalized;
                raycastDir.y = raycastDir.y < 0 ? raycastDir.y * -1 : raycastDir.y;


                RaycastHit hit;
                if (Physics.Raycast(rayoriginPos + Vector3.down, raycastDir, out hit, maxRopeAnchorDistance, groundLayer))
                {
                    anchorPositions.Add(hit.point);
                }
            Debug.DrawRay(rayoriginPos + Vector3.down, raycastDir * maxRopeAnchorDistance, Color.red, 1000);
            }
            //==============================================
            //GET AVERAGE DISTANCE
            float avgDist = 0;
            float avgYPos = 0;
            for (int j = 0; j < anchorPositions.Count - 1; j++)
            {
                avgDist += Vector3.Distance(anchorPositions[j], anchorPositions[j + 1]);
                avgYPos += anchorPositions[j].y;
            }
            avgYPos += anchorPositions[anchorPositions.Count - 1].y;
            avgYPos /= anchorPositions.Count;
            avgDist /= anchorPositions.Count - 1;

            //==============================================
            GameObject rope = Instantiate(ropePrefab, rayoriginPos, Quaternion.identity);
            Rope ropeScrRef = rope.GetComponent<Rope>();


            ropeScrRef.SetRopeLength(avgDist +  avgYPos * Random.Range(ropeLength.x, ropeLength.y));
            ropeScrRef.SetAnchors(anchorPositions.ToArray());
        }
    }
private void GenerateSecondaryRooms(Texture2D _texture, int _padding)
    { 
        int successfulRooms = 0;
        for (int i = 0; i < 1000; i++)
        {
            RoomMarker tempRoom = new RoomMarker();
            tempRoom.R_Size = new Vector2Int((int)Random.Range(minRoomSize.x, maxRoomSize.x), (int)Random.Range(minRoomSize.y, maxRoomSize.y));
            Vector2Int Padding = new Vector2Int((tempRoom.R_Size.x / 2), (tempRoom.R_Size.y / 2));
            tempRoom.R_Pos = new Vector2Int((int)Random.Range(0 + Padding.x, _texture.Size().x - Padding.x), (int)Random.Range(0 + Padding.y, _texture.Size().y - Padding.y));
            Vector2Int offset = new Vector2Int((tempRoom.R_Size.x / 2), (tempRoom.R_Size.y / 2));

            bool success = true;

            for (int x = 0 - _padding; x < tempRoom.R_Size.x + _padding; x++)
            {
                for (int y = 0 - _padding; y < tempRoom.R_Size.y + _padding; y++)
                {
                    Vector2Int _pos = new Vector2Int(tempRoom.R_Pos.x + x - offset.x, tempRoom.R_Pos.y + y - offset.y);
                    if (roomTexture.GetPixel(_pos.x, _pos.y) != Color.black)
                    {
                        success = false;
                    }
                }
                if (!success) break;
            }
            Vector2Int nearestPos;
            if (success)
            {
                if (GetNearestPPoint(out nearestPos, tempRoom.R_Pos, _texture, 500))
                {
                    DrawDoubleHallway(tempRoom.R_Pos, nearestPos, DoorWidth, DoorHeight);



                    for (int x = 0; x < tempRoom.R_Size.x; x++)
                    {
                        for (int y = 0; y < tempRoom.R_Size.y; y++)
                        {

                            roomTexture.SetPixel(tempRoom.R_Pos.x + x - offset.x, tempRoom.R_Pos.y + y - offset.y, roomCol);

                        }
                    }
                successfulRooms++;
                }
            }
            if (successfulRooms >= maxNumOfSecondaryRooms) break;
        }

    }
    private bool GetNearestPPoint(out Vector2Int _res, Vector2Int _origin, Texture2D _texture, int _searchDist)
    {
        for (int xy = 0; xy < _searchDist; xy++)
        {
            //Positive x search
            Vector2Int pxs = new Vector2Int(xy, _origin.y);
            if (pxs.x <= _texture.width - 1)
            {
                if (_texture.GetPixel(pxs.x, pxs.y) != Color.black)
                {
                    _res = new Vector2Int( pxs.x + 1, pxs.y);
                    return true;
                }
            }
            //Negative x search
            Vector2Int nxs = new Vector2Int(-xy, _origin.y);
            if (nxs.x >= 0)
            {
                if (_texture.GetPixel(nxs.x, nxs.y) != Color.black)
                {
                    _res = new Vector2Int(nxs.x - 1, nxs.y);
                    return true;
                }
            }
            //Positive y search
            Vector2Int pys = new Vector2Int(_origin.x, xy);
            if (pys.y <= _texture.height - 1)
            {
                if (_texture.GetPixel(pys.x, pys.y) != Color.black)
                {
                    _res = new Vector2Int(pys.x, pys.y + 1);
                    return true;
                }
            }
            //Negative y search
            Vector2Int nys = new Vector2Int(_origin.x, -xy);
            if (nys.y >= 0)
            {
                if (_texture.GetPixel(nys.x, nys.y) != Color.black)
                {
                    _res = new Vector2Int(nys.x, nys.y - 1);
                    return true;
                }
            }
        }
        _res = Vector2Int.zero;
        return false;
    }
    private RoomMarker[] GeneratePrimaryRooms(Texture2D _texture)
    {
        RoomMarker[] tempRes = new RoomMarker[maxNumOfPrimaryRooms];
        List<RoomMarker> res = new List<RoomMarker>(0);
        for (int i = 0; i < tempRes.Length; i++)
        {
            int safety = 0;
            while (safety < 50)
            {
                tempRes[i].R_Size = new Vector2Int((int)Random.Range(minRoomSize.x, maxRoomSize.x), (int)Random.Range(minRoomSize.y, maxRoomSize.y));
                Vector2Int Padding = new Vector2Int((tempRes[i].R_Size.x / 2), (tempRes[i].R_Size.y / 2));
                tempRes[i].R_Pos = new Vector2Int((int)Random.Range(0 + Padding.x, _texture.Size().x - Padding.x), (int)Random.Range(0 + Padding.y, _texture.Size().y - Padding.y));

                Vector2Int currMinDist = GetMinDist(tempRes[i], res.ToArray());
             
                if (i == 0 || (currMinDist.x > minRoomDist && currMinDist.y > minRoomDist))
                {
                    res.Add(tempRes[i]);
                    break;
                }
                safety++;
            }
        }
        return res.ToArray();
    }
    private Vector2Int GetMinDist(RoomMarker _origin, RoomMarker[] _allRooms)
    {
        if (_allRooms.Length == 0) return Vector2Int.one *  int.MaxValue;

        int xDist = Mathf.Abs(_origin.R_Pos.x - _allRooms[0].R_Pos.x) - ((_origin.R_Size.x/2) + (_allRooms[0].R_Size.x/2));
        int yDist = Mathf.Abs(_origin.R_Pos.y - _allRooms[0].R_Pos.y) - ((_origin.R_Size.y/2) + (_allRooms[0].R_Size.y/2));
        
        for (int i = 0; i < _allRooms.Length; i++)
        {
            int xTempDist = Mathf.Abs(_origin.R_Pos.x - _allRooms[i].R_Pos.x) - ((_origin.R_Size.x / 2) + (_allRooms[i].R_Size.x / 2));
            int yTempDist = Mathf.Abs(_origin.R_Pos.y - _allRooms[i].R_Pos.y) - ((_origin.R_Size.y / 2) + (_allRooms[i].R_Size.y / 2));

            if (xTempDist < xDist) xDist = xTempDist;
            if (yTempDist < yDist) yDist = yTempDist;
        }

        return new Vector2Int(xDist, yDist);
    }
   
    private void DrawDoubleHallway(Vector2Int _p1, Vector2Int _p2, int width, int height)
    {
        bool flipDir = Random.Range(0,2) == 0 ? true : false;
        //==============================
        //Draw Line One
        Vector2Int CursorPos = _p1;
        int safety = 1000;
        float GSC = (float)height / 10;
        //Debug.Log("Width: " + width + "Floor: " + Mathf.FloorToInt(((float)(width - 1) / 2)) + " Ceiling: " + Mathf.CeilToInt(((float)(width - 1) / 2)));
        while (CursorPos != _p2 || safety <= 0)
        {
            for (int x = CursorPos.x - Mathf.FloorToInt((float)(width - 1) / 2) - 1; x < CursorPos.x + Mathf.CeilToInt((float)(width - 1) / 2) + 1; x++)
            {
                for (int y = CursorPos.y - Mathf.FloorToInt(((float)(width - 1) / 2)) - 1; y < CursorPos.y + Mathf.CeilToInt(((float)(width - 1) / 2)) + 1; y++)
                {
                    if (roomTexture.GetPixel(x, y) == Color.black)
                    {
                        roomTexture.SetPixel(x, y, new Color(GSC, GSC, GSC));
                    }
                   
                }
            }

            if (flipDir)
            {
                if (CursorPos.x > _p2.x) CursorPos.x--;
                else if (CursorPos.x < _p2.x) CursorPos.x++;
                else if (CursorPos.y < _p2.y) CursorPos.y++;
                else if (CursorPos.y > _p2.y) CursorPos.y--;
                safety--;

                continue;
            }
            else
            {
                if (CursorPos.x < _p2.x) CursorPos.x++;
                else if (CursorPos.x > _p2.x) CursorPos.x--;
                else if (CursorPos.y > _p2.y) CursorPos.y--;
                else if (CursorPos.y < _p2.y) CursorPos.y++;
            safety--;
                continue;
            }
        }
       


    }
    public override int PlaceTile(Vector3Int _pos)
    {
        return FromTexture(_pos);

    }


  
    private int FromTexture(Vector3Int _pos)
    {
        Color temp = roomTexture.GetPixel(_pos.x, _pos.z);
        int greyCol =  Mathf.RoundToInt((float)temp.grayscale * (float)maxHeight);
        if (roomTexture.GetPixel(_pos.x + 1, _pos.z).grayscale != 0f || roomTexture.GetPixel(_pos.x - 1, _pos.z).grayscale != 0f || roomTexture.GetPixel(_pos.x, _pos.z + 1).grayscale != 0f || roomTexture.GetPixel(_pos.x, _pos.z - 1).grayscale != 0f)
        {

            if (_pos.y == 0) return floorBlockID;
            else if (_pos.y == maxHeight) return ceilingBlockID;
            else if (_pos.y > maxHeight) return 0;

            if (_pos.y >= greyCol)
            {
                if (_pos.y <= baseBoardHeight) return baseBoardBlockID;

                else if (_pos.y >= maxHeight - topPlateHeight) return topPlaceBlockID;
                else
                {
                    if (_pos.y - 1 < greyCol) return ceilingBlockID;
                    return wallBlockID;
                }
            }
        }
        return 0;
    }
}
