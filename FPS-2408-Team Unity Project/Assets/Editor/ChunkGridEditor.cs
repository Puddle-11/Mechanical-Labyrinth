using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(ChunkGrid))]
public class ChunkGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ChunkGrid _target = (ChunkGrid)target;
        if (_target == null) return;

        Undo.RecordObject(_target, "Change ChunkGrid Editor");
        if(GUILayout.Button("Bake Mesh"))
        {
            _target.GenFromEditor();
        }
        if (GUILayout.Button("Clear Mesh"))
        {
            _target.ClearChunkGrid();
        }
    }
    
}
#endif
