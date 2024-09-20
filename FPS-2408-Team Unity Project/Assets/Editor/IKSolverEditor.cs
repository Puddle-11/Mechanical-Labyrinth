using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(IKSolver))]

public class IKSolverEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
         IKSolver _target = (IKSolver)target;
        if (_target == null) return;

        Undo.RecordObject(_target, "Change IK Editor");
        if(GUILayout.Button("Create Arm"))
        {
            _target.CreateArm();
        }
        if (GUILayout.Button("Solve"))
        {
            _target.Solve();
        }
    }
}
