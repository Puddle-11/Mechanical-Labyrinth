using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnDisplay : MonoBehaviour
{
    public GameObject LockOnGUI;
    [SerializeField] private Vector3 relativeScale;
    public void Update()
    {
        CheckLOS();
    }
    public void CheckLOS()
    {
        RaycastHit checkLineOfSight;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,out checkLineOfSight ,Mathf.Infinity, ~GameManager.instance.projectileIgnore))
        {
            if(checkLineOfSight.collider.GetComponent<IHealth>() != null || checkLineOfSight.collider.GetComponent<Pickup>() != null)
            {
                LockOnGUI.SetActive(true);
                Vector3 screenPos = Camera.main.WorldToScreenPoint(checkLineOfSight.collider.transform.position);
                LockOnGUI.transform.position = screenPos;
                LockOnGUI.transform.localScale = relativeScale / Vector3.Distance(checkLineOfSight.collider.transform.position, Camera.main.transform.position);
                return;
            }
        }
        LockOnGUI.SetActive(false);

    }
}
