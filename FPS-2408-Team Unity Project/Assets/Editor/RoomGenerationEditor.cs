using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

#region Editor
[CustomEditor(typeof(RoomGenerator))]
[CanEditMultipleObjects]
public class RoomGenerationCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RoomGenerator _target = (RoomGenerator)target;
        if (_target == null) return;
        Undo.RecordObject(_target, "Change RoomGeneration");

        DrawFields(_target);
    }
    static void DrawFields(RoomGenerator _targetCast)
    {


        switch (_targetCast.generatorType)
        {


            case RoomGenerator.GenerationType.FromAlgorithm:
                DrawFromAlgoithm(_targetCast);
                break;
        }
        DrawGeneral(_targetCast);
    }
    static void DrawGeneral(RoomGenerator _targetCast)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("GENERAL", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        _targetCast.maxHeight = EditorGUILayout.IntField("Max Height", _targetCast.maxHeight);
        _targetCast.roomMap = EditorGUILayout.ObjectField("Room Map", _targetCast.roomMap, typeof(Sprite), true) as Sprite;
        _targetCast.baseBoardHeight = EditorGUILayout.IntField("Base Board Height", _targetCast.baseBoardHeight);
        _targetCast.topPlateHeight = EditorGUILayout.IntField("Top Plate Height", _targetCast.topPlateHeight);
        _targetCast.floorBlockID = EditorGUILayout.IntField("Floor Block ID", _targetCast.floorBlockID);
        _targetCast.ceilingBlockID = EditorGUILayout.IntField("Ceiling Block ID", _targetCast.ceilingBlockID);
        _targetCast.baseBoardBlockID = EditorGUILayout.IntField("Base Board ID", _targetCast.baseBoardBlockID);
        _targetCast.topPlaceBlockID = EditorGUILayout.IntField("Top Plate ID", _targetCast.topPlaceBlockID);

    }


    static void DrawFromAlgoithm(RoomGenerator _targetCast)
    {
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("FROM ALGORITHM", EditorStyles.boldLabel);

        EditorGUILayout.Space();
        _targetCast.ropePrefab = EditorGUILayout.ObjectField("Rope Prefab", _targetCast.ropePrefab, typeof(GameObject), true) as GameObject;
        _targetCast.numberOfRopes = EditorGUILayout.IntField("Number Of Ropes", _targetCast.numberOfRopes);
        _targetCast.maxRopeAnchorDistance = EditorGUILayout.IntField("Rope Anchor Distance", _targetCast.maxRopeAnchorDistance);
        _targetCast.numberOfRopeAnchors = EditorGUILayout.IntField("Number Of Rope Anchors", _targetCast.numberOfRopeAnchors);

        _targetCast.ceilingHeight = EditorGUILayout.IntField("Ceiling Height", _targetCast.ceilingHeight);
        _targetCast.minRoomDist = EditorGUILayout.FloatField("Min Room Distance", _targetCast.minRoomDist);
        _targetCast.maxNumOfPrimaryRooms = EditorGUILayout.IntField("Number of Rooms", _targetCast.maxNumOfPrimaryRooms);
        _targetCast.maxNumOfSecondaryRooms = EditorGUILayout.IntField("Number of Secondary Rooms", _targetCast.maxNumOfSecondaryRooms);

        _targetCast.minRoomSize = EditorGUILayout.Vector2IntField("Min Room Size", _targetCast.minRoomSize);
        _targetCast.maxRoomSize = EditorGUILayout.Vector2IntField("Max Room Size", _targetCast.maxRoomSize);
        _targetCast.DoorWidth = EditorGUILayout.IntField("Door Width", _targetCast.DoorWidth);
        _targetCast.DoorHeight = EditorGUILayout.IntField("Door Height", _targetCast.DoorHeight);
    }
}
    #endregion
#endif