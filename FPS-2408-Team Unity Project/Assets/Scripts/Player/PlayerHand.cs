using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class PlayerHand : MonoBehaviour
{
    [SerializeField] private GameObject handAnchor;
    [SerializeField] private float pickUpDist;
    private GameObject CurrentEquiped;
    [SerializeField] private LayerMask ignoreMask;
    [SerializeField] private Vector2 throwSpeed;
    [SerializeField] private Vector2 throwOffset;


    //========================================================
    //THESE METHODS NEED OPTIMIZING TOT
    public void SetUseItem(bool _val)
    {
        IUsable itemRef;
        if (CurrentEquiped != null && CurrentEquiped.TryGetComponent<IUsable>(out itemRef))
        {
            itemRef.SetUsingItem(_val);
        }
    }
    public bool GetUseItem()
    {
        IUsable itemRef;
        if (CurrentEquiped != null && CurrentEquiped.TryGetComponent<IUsable>(out itemRef))
        {
            return itemRef.GetUsingItem();
        }
        return false;
    }
    //========================================================
    //I really hate the "chain" of methods from the player controller
    //all the way to the currently equiped item, there are to many points of failure
    // - Rowan
    //========================================================



 


    public GameObject GetCurrentHand()
    {
        return CurrentEquiped;
    }


    public void ClickPickUp()
    {
        if (!AttemptPickup())
        {
            AttemptDrop();
        }
    }
    public void UseItem()
    {
        IUsable useRef;
        if (GetItem(out useRef))
        {
            useRef.UseItem();
        }
    }
    public bool GetItem(out IUsable _usableRef)
    {
        IUsable useRef;
        if (CurrentEquiped != null && CurrentEquiped.TryGetComponent<IUsable>(out useRef))
        {
            _usableRef = useRef;
            return true;
        }
        else
        {
            _usableRef = null;

            return false;
        }

    }
    private bool AttemptDrop()
    {
        if (CurrentEquiped != null)
        {

            //======================================
            //External Resets
            UIManager.instance.AmmoDisplay(0, 0);
            UIManager.instance.UpdateAmmoFill(1, 1);
            CameraController.instance.ResetOffset(true);
            //======================================


            IUsable IRef;
            if (GetItem(out IRef))
            {
             
                Pickup _pickup = Instantiate(IRef.GetPickup(), transform.position, IRef.GetPickup().transform.rotation).GetComponent<Pickup>();
                _pickup.DropItem(transform.position + transform.forward * throwOffset.x + new Vector3(0, throwOffset.y, 0), Camera.main.transform.forward * throwSpeed.x + Vector3.up * throwSpeed.y, 1);
            }


            //======================================
            //Internal Resets
            Destroy(CurrentEquiped);
            CurrentEquiped = null;
            //======================================
            return true;
        }
        return false;
    }
    private bool AttemptPickup()
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, pickUpDist, ~GameManager.instance.projectileIgnore))
        {
            Pickup objectPickupRef;
            if (hit.transform.TryGetComponent<Pickup>(out objectPickupRef) && objectPickupRef.GetItem() != null)
            {
                AttemptDrop(); //attempt a drop before picking up a new item
                objectPickupRef.PickupItem(out CurrentEquiped, handAnchor.transform.position, handAnchor.transform.rotation, handAnchor.transform);
                // CurrentEquiped = Instantiate(itemRef.Object, handAnchor.transform.position, handAnchor.transform.rotation, handAnchor.transform);
                IUsable iRef;
                if (GetItem(out iRef)) {
                    GameManager.instance.playerControllerRef.playerUseEvent = iRef.UseItem;
                }
                BaseGun BGref;
                if(CurrentEquiped.TryGetComponent<BaseGun>(out BGref))
                {
                    BGref.SetPlayerGun(true);
                }

                return true;
            }
        }
        return false;
    }
}
