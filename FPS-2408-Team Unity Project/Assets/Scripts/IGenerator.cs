using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class IGenerator :MonoBehaviour
{
    [Header("IGENERATOR")]
    [Space]
    [SerializeField] protected bool enclose;
    public GameObject EndDoorPrefab;
    public virtual int PlaceTile(Vector3Int _pos)
    {
        if(_pos.y == 0)return 1;
        return 0;
    }
    public virtual void SetGeneratorBounds(ChunkGrid.GridBounds _bounds)
    {

    }
    public virtual void GenerateMap()
    {

    }
    public virtual Vector2Int GetStartingPoint()
    {
        return Vector2Int.zero;
    }
}
