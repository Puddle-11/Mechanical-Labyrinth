using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LockOnDisplay : MonoBehaviour
{
    public GameObject LockOnGUI;
    [SerializeField] private float relativeScale;

    [SerializeField] private Vector2 minSize;
    [SerializeField] private float boundSize;
    [SerializeField] private GameObject test;
    [SerializeField] private TMP_Text infoDisplay;
    public float scalar;
    struct ssBounds
    {
       public Vector3 min;
        public Vector3 max;
    }
    public void Update()
    {

        CheckLOS();
    }
    public void CheckLOS()
    {
        RaycastHit checkLineOfSight;
        if(Physics.Raycast(CameraController.instance.mainCamera.transform.position, CameraController.instance.mainCamera.transform.forward,out checkLineOfSight ,Mathf.Infinity, ~GameManager.instance.projectileIgnore))
        {
            Pickup pickupRef;
            if (checkLineOfSight.collider.TryGetComponent<Pickup>(out pickupRef) || checkLineOfSight.collider.GetComponent<IHealth>() != null)
            {


                LockOnGUI.SetActive(true);
                float distanceScale = relativeScale / Vector3.Distance(checkLineOfSight.collider.transform.position, CameraController.instance.mainCamera.transform.position);
                Renderer rendRef = checkLineOfSight.collider.GetComponent<Renderer>();

                ssBounds objectScreenBounds = worldToScreenBounds(rendRef != null ? rendRef.bounds : checkLineOfSight.collider.bounds);

                RectTransform rectRef = LockOnGUI.GetComponent<RectTransform>();
                Vector2 WH = new Vector2(objectScreenBounds.max.x - objectScreenBounds.min.x, objectScreenBounds.max.y - objectScreenBounds.min.y);
                WH = WH / distanceScale * (boundSize / Screen.width);
                WH = new Vector2(Mathf.Clamp(WH.x, minSize.x, Mathf.Infinity), Mathf.Clamp(WH.y, minSize.y, Mathf.Infinity));
                rectRef.sizeDelta = WH;

                if (pickupRef != null)
                {

                    UpdateInfo(pickupRef.GetStats(), new Vector2(objectScreenBounds.max.x, objectScreenBounds.max.y), distanceScale);
                    infoDisplay.transform.gameObject.SetActive(true);

                }
                else
                {
                    UpdateInfo("", Vector3.zero, distanceScale);
                    infoDisplay.transform.gameObject.SetActive(false);
                }
                LockOnGUI.transform.position = (objectScreenBounds.min + objectScreenBounds.max) / 2;
                LockOnGUI.transform.localScale = Vector3.one * distanceScale;

                return;
            }
        }
        infoDisplay.transform.gameObject.SetActive(false);

        LockOnGUI.SetActive(false);

    }
    public void UpdateInfo(string _text, Vector2 _pos, float _distScale)
    {
        infoDisplay.text = _text;
        infoDisplay.transform.position = _pos;
        infoDisplay.transform.localScale = Vector3.one * _distScale;
    }
    private ssBounds worldToScreenBounds(Bounds _boundingBox)
    {
        ssBounds temp = new ssBounds();
        temp.max = _boundingBox.max;
        temp.min = _boundingBox.min;
        return worldToScreenBounds(temp);
    }
    private ssBounds worldToScreenBounds(ssBounds _worldBounds)
    {

        Vector3[] screenPosCorners = new Vector3[8];
        screenPosCorners[0] = CameraController.instance.mainCamera.WorldToScreenPoint(_worldBounds.max) * scalar;

        screenPosCorners[1] = CameraController.instance.mainCamera.WorldToScreenPoint(new Vector3(_worldBounds.min.x, _worldBounds.max.y, _worldBounds.max.z)) * scalar;
        screenPosCorners[2] = CameraController.instance.mainCamera.WorldToScreenPoint(new Vector3(_worldBounds.min.x, _worldBounds.max.y, _worldBounds.min.z)) * scalar;
        screenPosCorners[3] = CameraController.instance.mainCamera.WorldToScreenPoint(new Vector3(_worldBounds.max.x, _worldBounds.max.y, _worldBounds.min.z)) * scalar;

        screenPosCorners[4] = CameraController.instance.mainCamera.WorldToScreenPoint(new Vector3(_worldBounds.min.x, _worldBounds.min.y, _worldBounds.max.z)) * scalar;
        screenPosCorners[5] = CameraController.instance.mainCamera.WorldToScreenPoint(new Vector3(_worldBounds.max.x, _worldBounds.min.y, _worldBounds.max.z)) * scalar;
        screenPosCorners[6] = CameraController.instance.mainCamera.WorldToScreenPoint(new Vector3(_worldBounds.max.x, _worldBounds.min.y, _worldBounds.min.z)) * scalar;
        screenPosCorners[7] = CameraController.instance.mainCamera.WorldToScreenPoint(_worldBounds.min) * scalar;


        ssBounds result = new ssBounds();
        result.min = screenPosCorners[0];

        result.max = screenPosCorners[0];

        for (int i = 0; i < 8; i++)
        {
            //new min for x found
            if (screenPosCorners[i].x < result.min.x)
            {
                result.min.x = screenPosCorners[i].x;
            }
            //new max for x found
            if (screenPosCorners[i].x > result.max.x)
            {
                result.max.x = screenPosCorners[i].x;
            }

            if (screenPosCorners[i].y < result.min.y)
            {
                result.min.y = screenPosCorners[i].y;
            }
            //new max for x found
            if (screenPosCorners[i].y > result.max.y)
            {
                result.max.y = screenPosCorners[i].y;
            }
        }
        return result;
    }
   
}
