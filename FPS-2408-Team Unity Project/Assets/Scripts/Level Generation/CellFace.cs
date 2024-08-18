using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CellFace 
{
    public Vector3Int[] f_points = new Vector3Int[4];
    public int[] f_triangles = new int[6];
    public Facetype f_facetype;
    public int f_textureID;
    public enum Facetype
    {
        Up,
        Down,
        Left,
        Right,
        Front,
        Back
    }
}
