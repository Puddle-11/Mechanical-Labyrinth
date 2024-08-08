using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    [SerializeField] private GameObject handAnchor;
    private GameObject CurrentEquiped;
    [SerializeField] private LayerMask ignoreMask;
    private Pickup CurrentPickup;
    [SerializeField] private Vector2 throwSpeed;
    [SerializeField] private Vector2 throwOffset;
    private void Update()
    {
        if (Input.GetButtonDown("Pick Up") && !AttemptDrop())
        {

            AttemptPickup();
        }
       
    }
    private bool AttemptDrop()
    {
        if(CurrentEquiped != null)
        {

            Destroy(CurrentEquiped);
            CurrentEquiped = null;
            CurrentPickup.DropItem(transform.position + transform.forward * throwOffset.x + new Vector3(0,throwOffset.y,0), Camera.main.transform.forward *throwSpeed.x+ Vector3.up * throwSpeed.y);
            return true;
        }
        return false;
    }
    private bool AttemptPickup()
    {
        
        RaycastHit hit = GlobalMethods.RaycastFromCam(~ignoreMask);
        if(hit.collider != null)
        {
            Pickup objectPickupRef;
            if (hit.transform.TryGetComponent<Pickup>(out objectPickupRef) && objectPickupRef.GetItem() != null)
            {
                CurrentEquiped = Instantiate(objectPickupRef.PickupItem().Object, handAnchor.transform.position, handAnchor.transform.rotation, handAnchor.transform);
                CurrentPickup = objectPickupRef;

                return true;
            }
        }
        return false;
    }
}
