using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CanvasScalar : MonoBehaviour
{
    public Vector2Int defaultResolution;
    public canvasObj[] canvasObjects;
    [System.Serializable]
    public struct canvasObj
    {
        public RectTransform transform;
        [HideInInspector] public Vector3 originalScale;
    }
    private void Start()
    {
        GetOriginalScales();
    }

    public void Update()
    {
        Scale();
    }

    public void GetOriginalScales()
    {
        if (canvasObjects == null || canvasObjects.Length <= 0) return;

        for (int i = 0; i < canvasObjects.Length; i++)
        {
            canvasObjects[i].originalScale = transform.localScale;
        }
    }
    public void Scale()
    {
        if (canvasObjects == null || canvasObjects.Length <= 0) return;
        Vector2 scalar = new Vector2((float) Screen.width / defaultResolution.x, (float) Screen.height/ defaultResolution.y);
        for (int i = 0; i < canvasObjects.Length; i++)
        {
            canvasObjects[i].transform.localScale = canvasObjects[i].originalScale * scalar;
        }

    }
}

