using System.Collections;
using System.Collections.Generic;
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
    public Vector2 offset;
    private bool camShaking;
    public Camera mainCamera;
    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Found two Camera Controllers in scene\nDestroyed at " + gameObject.name);
            Destroy(this);
        }
    }
    public void Start()
    {
        GetMainCamera();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void GetMainCamera()
    {
        if (mainCamera != null) return;
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
                mainCamera = camRef;
            }
            else
            {
                Debug.LogWarning("Gameobject: " + camList[0].name + " Marked with 'MainCamera' but doesnt contain a camera component");
            }
        }
        else
        {
            Debug.Log("no Camera with 'MainCamera' tag found in scene");

        }
    }
    public void StartCamShake()
    {
        StartCoroutine(StartCamShakeEnum(camShakeDurration, camShakeScalar, camShakeIntensity));
    }

    public void StartCamShake(float _durr, float _scalar)
    {
        StartCoroutine(StartCamShakeEnum(_durr, _scalar, camShakeIntensity));
    }

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
                timeElapsed += Time.deltaTime;
                yield return null;
        }
        transform.localPosition = originPos;
        camShaking = false;
    }
    // Start is called before the first frame update
  

    // Update is called once per frame
    void Update()
    {
        if (UIManager.instance == null || (UIManager.instance != null && !UIManager.instance.GetStatePaused()))
        {
            UpdateCamPos();
        }
    }
    private void FixedUpdate()
    {
       if(resettingOffset) ResetOffsetPos();
    }

    public void UpdateCamPos()
    {

        float yaw = Input.GetAxis("Mouse X") * sens;
        float pitch = Input.GetAxis("Mouse Y") * sens;
        rotX = invert ? rotX + pitch: rotX - pitch;
        rotX = Mathf.Clamp(rotX, miny, maxy);

        cameraAnchor.localRotation = Quaternion.Euler(rotX + offset.y, 0, 0);
        GameManager.instance.playerRef.transform.Rotate(Vector3.up * yaw);
    }
    public void UpdateOffsetPos(Vector2 _offset)
    {
        SetOffsetPos(offset + _offset);
    }
    public void SetOffsetPos(Vector2 _offset)
    {
        offset = _offset;
    }
    public void ResetOffset(bool _val)
    {
        resettingOffset = _val;
    }
    private void ResetOffsetPos()
    {
        offset = Vector2.MoveTowards(offset, Vector2.zero, offsetResetSpeed + Mathf.Pow(Vector2.Distance(offset, Vector2.zero), 2) * offsetResetDampening);
        if (Mathf.Abs(offset.y) < 0.01) offset.y = 0;
        if (Mathf.Abs(offset.x) < 0.01) offset.x = 0;
    }
}
