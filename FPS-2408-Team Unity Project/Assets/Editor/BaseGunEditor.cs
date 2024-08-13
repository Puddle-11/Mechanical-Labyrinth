using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(BaseGun))]
public class BaseGunEditor : Editor
{
#if UNITY_EDITOR
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BaseGun _target = (BaseGun)target;
        if (_target == null) return;
        Undo.RecordObject(_target, "Change EntityManager");

        DrawFields(_target);
    }
    static void DrawFields(BaseGun _managerCast)
    {
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("GENERAL", EditorStyles.boldLabel);
        EditorGUILayout.Space();



    }

#endif
}
