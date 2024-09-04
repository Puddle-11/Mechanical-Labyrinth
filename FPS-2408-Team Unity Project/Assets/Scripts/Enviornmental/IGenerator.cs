using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//====================================
//REWORKED
//====================================
public class IGenerator : MonoBehaviour
{
    [Header("IGENERATOR")]
    [Header("_______________________________")]
    [Space]
    // \/ Determins wether or not the algorithm will draw \/
    // \/ a bounding wall around the map or leave it open \/
    [SerializeField] protected bool enclose; 


    [SerializeField] protected GameObject EndDoorPrefab;
    [SerializeField] protected Vector3 endDoorOffset;

    protected List<Vector3> groundPositions;
    protected List<Vector3> allPositions;
    protected Vector2Int startPos;

    #region Custom Structs and Enums
    public struct RoomMarker
    {
        public Vector2Int R_Size;
        public Vector2Int R_Pos;
    }
    #endregion

    #region Getters and Setters
    public virtual Vector3 GetDoorOffset() { return endDoorOffset; }
    public virtual GameObject GetDoorPrefab() { return EndDoorPrefab; }
    public virtual Vector2Int GetStartPos() { return startPos; }
    public virtual Texture2D GetRoomTexture() {return null;}
    public virtual void SetGeneratorBounds(ChunkGrid.GridBounds _bounds){ }
    #endregion

    #region Virtual Methods
    public virtual void GenerateAllPositions(Texture2D _texture) { }
    public virtual List<Vector3> GetPositions()
    {
        return null;
    }
    public virtual void GenerateMap(){ }
    public virtual int PlaceTile(Vector3Int _pos) { return _pos.y == 0 ? 1 : 0;}
    #endregion
}
