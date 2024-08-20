using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

    #region Editor
    [CustomEditor(typeof(RoomGenerator))]
    [CanEditMultipleObjects]
    public class RoomGenerationCustomEditor: Editor
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
           

            switch ( _targetCast.generatorType ) 
            {
                case RoomGenerator.GenerationType.FromVariables:
                    DrawFromVariables(_targetCast);

                    break;
                case RoomGenerator.GenerationType.FromTexture:
                    DrawFromTexture(_targetCast);
                    break;
                case RoomGenerator.GenerationType.FromWaveFunction:
                    break;
            }
            DrawGeneral(_targetCast);
        }
        static void DrawGeneral(RoomGenerator _targetCast)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("GENERAL", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            _targetCast.baseBoardHeight = EditorGUILayout.IntField("Base Board Height", _targetCast.baseBoardHeight);
            _targetCast.topPlateHeight = EditorGUILayout.IntField("Top Plate Height", _targetCast.topPlateHeight);
            _targetCast.floorBlockID = EditorGUILayout.IntField("Floor Block ID", _targetCast.floorBlockID);
            _targetCast.ceilingBlockID = EditorGUILayout.IntField("Ceiling Block ID", _targetCast.ceilingBlockID);
            _targetCast.baseBoardBlockID = EditorGUILayout.IntField("Base Board ID", _targetCast.baseBoardBlockID);
            _targetCast.topPlaceBlockID = EditorGUILayout.IntField("Top Plate ID", _targetCast.topPlaceBlockID);

        }
        static void DrawFromVariables(RoomGenerator _targetCast)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("FROM VARIABLES", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            _targetCast.RoomSize = EditorGUILayout.IntField("Room Size", _targetCast.RoomSize);
            _targetCast.DoorHeight = EditorGUILayout.IntField("Door Height", _targetCast.DoorHeight);
            _targetCast.DoorWidth = EditorGUILayout.IntField("Door Width", _targetCast.DoorWidth);

        }
        static void DrawFromTexture(RoomGenerator _targetCast)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("FROM TEXTURE", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            _targetCast.roomMap = EditorGUILayout.ObjectField("Room Map", _targetCast.roomMap, typeof(Sprite), true) as Sprite;
         

        }
    }
    #endregion
#endif