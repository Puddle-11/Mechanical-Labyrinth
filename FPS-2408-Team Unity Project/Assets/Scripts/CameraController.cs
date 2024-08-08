using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static Vector3 AimPoint;


    [Header("Gun Variables")]
    [Space]
    [SerializeField] private float aimDist;
    [SerializeField] private LayerMask ignoreMask;

    [Space]
    [Header("Camera variables")]
    [Space]
    [SerializeField] private int sens;
    [SerializeField] private float miny, maxy;
    [SerializeField] private bool invert;
    private float rotX;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState  = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.GetStatePaused())
        {
            float yaw = Input.GetAxis("Mouse X") * sens;
            float pitch = Input.GetAxis("Mouse Y") * sens;
            rotX = invert ? rotX + pitch : rotX - pitch;
            rotX = Mathf.Clamp(rotX, miny, maxy);

            transform.localRotation = Quaternion.Euler(rotX, 0, 0);
            transform.parent.Rotate(Vector3.up * yaw);
            UpdateAimPoint();
        }
    }
    
    private void UpdateAimPoint()
    {
        RaycastHit hit = GlobalMethods.RaycastFromCam(~ignoreMask, aimDist);


        if (hit.collider != null)
        {
            AimPoint = hit.point;
        }
        else
        {
            AimPoint = transform.position + Camera.main.transform.forward * aimDist;
        }
    }
    
}
