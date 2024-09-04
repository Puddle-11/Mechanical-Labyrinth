using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformController : MonoBehaviour
{
    [SerializeField] private ControlType type;
    [SerializeField] private Transform parent;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 rotation;

    #region Custom Structs and Enums
    public enum ControlType
    {
        positon,
        rotation,
        position_rotation,
    }
    #endregion

    #region MonoBehavior Methods
    public void Awake()
    {
        if (parent == null)
        {
            parent = transform.parent;
        }
    }
    public void LateUpdate()
    {
        UpdatePosition();
    }
    #endregion
    private void UpdatePosition()
    {
        if (transform.parent == null) return;
        if (type == ControlType.position_rotation || type == ControlType.positon)
        {
            transform.position = transform.parent.position + offset;
        }
        if (type == ControlType.position_rotation || type == ControlType.rotation)
        {
            transform.eulerAngles = rotation;
        }
    }


}