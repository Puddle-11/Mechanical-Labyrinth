using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : IGenerator
{

    [SerializeField] private int wallHeight; //This value will cap out at bounds height 
    [SerializeField] private int ceilingHeight; //make sure not to set it higher than y max bounds other wise it wont be generated
    [SerializeField] private int roomSize;
    [SerializeField] private int doorHeight;
    [SerializeField] private int doorWidth;
    [SerializeField] private int baseBoardHeight;
    [SerializeField] private int floorBlockID;
    [SerializeField] private int wallBlockID;
    [SerializeField] private int ceilingBlockID;
    [SerializeField] private int baseBoardBlockID;



    private ChunkGrid.GridBounds bounds;

    public override void SetGeneratorBounds(ChunkGrid.GridBounds _bounds)
    {
        bounds = _bounds;
    }

    public override int PlaceTile(Vector3Int _pos)
    {
        //ceiling
        if (_pos.y == ceilingHeight - 1)
        {
            return ceilingBlockID;
        }

        //Flor
        if (_pos.y == 0)
        {
            return floorBlockID;
        }
        //Outer walls
        if (enclose)
        {
            if (_pos.x == 0 || _pos.x == bounds.max.x || _pos.z == 0 || _pos.z == bounds.max.z)
            {
                return _pos.y <= baseBoardHeight ? baseBoardBlockID : wallBlockID;
            }
        }
        //Doors
        if (_pos.y < doorHeight)
        {
            if ((float)_pos.x % roomSize >= roomSize / 2 - doorWidth / 2 && (float)_pos.x % roomSize <= roomSize / 2 + doorWidth / 2)
            {
                return 0;
            }
            if ((float)_pos.z % roomSize >= roomSize / 2 - doorWidth / 2 && (float)_pos.z % roomSize <= roomSize / 2 + doorWidth / 2)
            {
                return 0;
            }
        }


        //walls
        if (_pos.y < wallHeight)
        {
            if (_pos.x % roomSize == 0 || _pos.z % roomSize == 0)
            {

                return _pos.y <= baseBoardHeight ? baseBoardBlockID : wallBlockID;

            }
        }

        return 0;
    }

}
