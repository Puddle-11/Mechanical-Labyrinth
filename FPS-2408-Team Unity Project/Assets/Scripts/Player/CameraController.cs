using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public static CameraController instance;
    [Space]
    [Header("Camera variables")]
    [Space]
    [SerializeField] private int sens;
    [SerializeField] private float miny, maxy;
    [SerializeField] private bool invert;
    [SerializeField] private Transform cameraAnchor;
    private float originalFOV;
    [SerializeField] private float zoomInSpeed;
    private float targetFOV;
    [Space]
    [Header("Camera Offset variables")]
    [Space]
    [SerializeField] private float offsetResetSpeed;
    [SerializeField] private float offsetResetDampening;
    private float rotX;
    [Header("Camera Shake variables")]
    [Space]
    [SerializeField] AnimationCurve camShakeIntensity;
    [SerializeField]  private float camShakeScalar;
    [SerializeField] private float camShakeDurration;
    public bool resettingOffset;
    private bool camShaking;
    public Camera mainCamera;
    public Vector3 targetAngles;
    public float maxOffsetMoveSpeed;
    public GameObject offsetObj;
    public void Awake()
    {

        if(instance == null) instance = this;
        else
        {
            Debug.LogWarning("Found two Camera Controllers in scene\nDestroyed at " + gameObject.name);
            Destroy(this);
        }
    }
    public void Start()
    {
        mainCamera =  GetMainCamera();
        originalFOV = mainCamera.fieldOfView;
        targetFOV = originalFOV;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public Camera GetMainCamera()
    {
        if (mainCamera != null) return mainCamera;
        GameObject[] camList = GameObject.FindGameObjectsWithTag("MainCamera");
        if (camList.Length != 0)
        {

            if (camList.Length > 1)
            {

                Debug.Log("Multiple Camera with 'MainCamera' tag found in scene\nthere can only be one main camera at a time");
            }
            Camera camRef;
            if (camList[0].TryGetComponent<Camera>(out camRef))
            {
                return camRef;
            }
            else
            {
                Debug.LogWarning("Gameobject: " + camList[0].name + " Marked with 'MainCamera' but doesnt contain a camera component");
                return null;
            }
        }
        else
        {
            Debug.Log("no Camera with 'MainCamera' tag found in scene");
            return null;
        }
    }
    public void SetFOV(float _FOV) { targetFOV = _FOV;}
    public float GetDefaultFOV() { return originalFOV;}
    public void ResetFOV() { targetFOV = originalFOV;}




    public void StartCamShake() { StartCoroutine(StartCamShakeEnum(camShakeDurration, camShakeScalar, camShakeIntensity));}
    public void StartCamShake(float _durr, float _scalar){ StartCoroutine(StartCamShakeEnum(_durr, _scalar, camShakeIntensity));}
    private IEnumerator StartCamShakeEnum(float _durr,  float _scalar, AnimationCurve _intensity)
    {
        if (camShaking) yield break;
        camShaking = true;
        if (_durr == 0) _durr = camShakeDurration;
        if (_scalar == 0) _scalar = camShakeScalar;
        Vector3 originPos = transform.localPosition;
        float timeElapsed = 0.0f;

        while (timeElapsed < _durr)
        {
        
                float evaluatedIntensity = _intensity.Evaluate(timeElapsed / camShakeDurration);
                float x = Random.Range(-1f, 1f) * _scalar * evaluatedIntensity;
                float y = Random.Range(-1f, 1f) * _scalar * evaluatedIntensity;
                transform.localPosition = new Vector3(x, y, originPos.z);
                timeElapsed += Time.unscaledDeltaTime;
                yield return null;
        }
        transform.localPosition = originPos;
        camShaking = false;
    }

    void Update()
    {
        mainCamera.fieldOfView = Mathf.MoveTowards(mainCamera.fieldOfView, targetFOV, zoomInSpeed * Time.deltaTime);
        if (UIManager.instance == null || (UIManager.instance != null && !GameManager.instance.GetStatePaused()))
        {
            UpdateCamPos();
        }
    }
    private void FixedUpdate()
    {


        targetAngles = Vector2.MoveTowards(targetAngles, Vector2.zero, offsetResetSpeed + Mathf.Pow(Vector2.Distance(targetAngles, Vector2.zero), 2) * offsetResetDampening);
        offsetObj.transform.localEulerAngles = targetAngles;

    }



    public void UpdateCamPos()
    {
        float yaw = Input.GetAxis("Mouse X") * (SettingsController.instance != null ? SettingsController.instance.GetSettings().S_cameraSensitivity : sens);
        float pitch = Input.GetAxis("Mouse Y") * (SettingsController.instance != null ? SettingsController.instance.GetSettings().S_cameraSensitivity:sens);
        rotX = invert ? rotX + pitch: rotX - pitch;
        rotX = Mathf.Clamp(rotX, miny, maxy);

        cameraAnchor.localRotation = Quaternion.Euler(rotX, 0, 0);
        GameManager.instance.playerRef.transform.Rotate(Vector3.up * yaw);
    }

    public void UpdateOffset(Vector3 _offset)
    {
        SetOffset(targetAngles + _offset);
    }
    public void SetOffset(Vector3 _offset)
    {
        targetAngles = _offset;
    }
    private void ResetOffset()
    {
        targetAngles = Vector3.zero;
    }
}
