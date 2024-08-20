using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomGenerator : IGenerator
{
   public GenerationType generatorType;
  [HideInInspector] public int maxHeight; //This value will cap out at bounds height 
 //Specific to FromTexture
  [HideInInspector] public Sprite roomMap;
//Specific to FromVariables
  [HideInInspector] public int RoomSize;
  [HideInInspector] public int DoorHeight;
  [HideInInspector] public int DoorWidth;
 //Global
  [HideInInspector] public int baseBoardHeight;
  [HideInInspector] public int topPlateHeight;
  [HideInInspector] public int floorBlockID;
  [HideInInspector] public int wallBlockID;
  [HideInInspector] public int ceilingBlockID;
  [HideInInspector] public int baseBoardBlockID;
  [HideInInspector] public int topPlaceBlockID;
    private ChunkGrid.GridBounds bounds;
    public enum GenerationType
    {
        FromTexture,
        FromVariables,
        FromWaveFunction,
    }
    public override void SetGeneratorBounds(ChunkGrid.GridBounds _bounds)
    {
        bounds = _bounds;
    }

    public override int PlaceTile(Vector3Int _pos)
    {
        switch (generatorType)
        {
            case GenerationType.FromTexture:
                return FromTexture(_pos);
            case GenerationType.FromVariables:
                return FromVariables(_pos);
        }
        return 0;
    }
    private int FromVariables(Vector3Int _pos)
    {
        //ceiling
        if (_pos.y == bounds.max.y)
        {
            return 2;
        }

        //Flor
        if (_pos.y == 0)
        {
            return 2;
        }
        //Outer walls

        if (_pos.x == 0 || _pos.x == bounds.max.x || _pos.z == 0 || _pos.z == bounds.max.z)
        {
            return _pos.y == 1 ? 3 : 1;
        }

        //Doors
        if (_pos.y < 4)
        {
            if ((float)_pos.x % 10 >= 4 && (float)_pos.x % 10 <= 6)
            {
                return 0;
            }
            if ((float)_pos.z % 10 >= 4 && (float)_pos.z % 10 <= 6)
            {
                return 0;
            }
        }


        //walls
        if (_pos.y < 10)
        {
            if (_pos.x % 10 == 0 || _pos.z % 10 == 0)
            {

                return _pos.y == 1 ? 3 : 1;

            }
        }

        return 0;
    }
    private int FromTexture(Vector3Int _pos)
    {
        Color temp = roomMap.texture.GetPixel(_pos.x, _pos.z);
        float greyCol = temp.grayscale * maxHeight;
        if (roomMap.texture.GetPixel(_pos.x + 1, _pos.z).grayscale != 0f || roomMap.texture.GetPixel(_pos.x - 1, _pos.z).grayscale != 0f || roomMap.texture.GetPixel(_pos.x, _pos.z + 1).grayscale != 0f || roomMap.texture.GetPixel(_pos.x, _pos.z - 1).grayscale != 0f)
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
