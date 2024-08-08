using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    [SerializeField] private GameObject handAnchor;
    private GameObject CurrentEquiped;
    [SerializeField] private LayerMask ignoreMask;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !AttemptDrop())
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
        }
        return false;
    }
    private bool AttemptPickup()
    {
        
        Debug.Log("entered pickup");
        RaycastHit hit = GlobalMethods.RaycastFromCam(ignoreMask);
        if(hit.collider != null)
        {
            Pickup objectPickupRef;
            if (hit.transform.TryGetComponent<Pickup>(out objectPickupRef) && objectPickupRef.GetItem() != null)
            {
                CurrentEquiped = Instantiate(objectPickupRef.PickupItem().Object, handAnchor.transform.position, handAnchor.transform.rotation, handAnchor.transform);
                
                return true;
            }
        }
        return false;
    }
}
