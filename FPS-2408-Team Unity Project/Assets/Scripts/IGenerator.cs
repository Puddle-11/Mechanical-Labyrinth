using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class IGenerator :MonoBehaviour
{
    [SerializeField] protected bool enclose;
    public virtual int PlaceTile(Vector3Int _pos)
    {
        if(_pos.y == 0)return 1;
        return 0;
    }
    public virtual void SetGeneratorBounds(ChunkGrid.GridBounds _bounds)
    {

    }
}
