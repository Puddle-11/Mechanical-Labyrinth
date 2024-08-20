using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomGenerator : IGenerator
{

    [SerializeField] private int maxHeight; //This value will cap out at bounds height 
    [SerializeField] private Sprite roomMap;
    [SerializeField] private int baseBoardHeight;
    [SerializeField] private int topPlateHeight;
    [SerializeField] private int floorBlockID;
    [SerializeField] private int wallBlockID;
    [SerializeField] private int ceilingBlockID;
    [SerializeField] private int baseBoardBlockID;
    [SerializeField] private int topPlaceBlockID;
    private ChunkGrid.GridBounds bounds;

    public override void SetGeneratorBounds(ChunkGrid.GridBounds _bounds)
    {
        bounds = _bounds;
    }

    public override int PlaceTile(Vector3Int _pos)
    {
        Color temp = roomMap.texture.GetPixel(_pos.x, _pos.z);
        float greyCol = temp.grayscale * maxHeight;
        if (roomMap.texture.GetPixel(_pos.x + 1, _pos.z).grayscale != 0f || roomMap.texture.GetPixel(_pos.x - 1, _pos.z).grayscale != 0f || roomMap.texture.GetPixel(_pos.x, _pos.z + 1).grayscale != 0f || roomMap.texture.GetPixel(_pos.x, _pos.z - 1).grayscale != 0f)
        {


            if (_pos.y == 0)
            {
                return floorBlockID;
            }
            if (_pos.y == maxHeight)
            {
                return ceilingBlockID;
            }
            if (_pos.y > maxHeight)
            {
                return 0;
            }
            if (_pos.y >= greyCol)
            {
                if (_pos.y <= baseBoardHeight)
                {
                    return baseBoardBlockID;
                }
                else if (_pos.y >= maxHeight - topPlateHeight)
                {
                    return topPlaceBlockID;
                }
                else
                {
                    if(_pos.y -1 < greyCol)
                    {
                        return ceilingBlockID;
                    }
                    return wallBlockID;
                }
            }
        }

        return 0;
    }

}
