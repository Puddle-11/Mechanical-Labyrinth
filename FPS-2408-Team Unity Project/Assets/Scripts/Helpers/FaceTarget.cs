using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FaceTarget : MonoBehaviour
{
    //target defaults to main camera if not assigned 
    [SerializeField] private GameObject target;
    [SerializeField] private type lookType = type.faceCamera;


    #region Custom Structs and Enums
    private enum type
    {
        faceCamera,
        faceTarget,
    }
    #endregion

    #region MonoBehavior Methods
    private void Start()
    {
        if(lookType == type.faceCamera)
        {
            
            if(CameraController.instance != null) target = CameraController.instance.GetMainCamera().gameObject;
        }
        else if (lookType == type.faceTarget)
        {
            if (target == null) target = GameManager.instance.playerRef;
        }
    }
    // Update is called once per frame
    void LateUpdate()
    {
        UpdateRotation();
    }
    #endregion


    // Update rotation is called in late update
    void UpdateRotation()
    {
        if (target == null) return;
 

        transform.rotation = Quaternion.LookRotation((target.transform.position - transform.position).normalized);
    }
}
