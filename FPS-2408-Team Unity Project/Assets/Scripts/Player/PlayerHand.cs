using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class PlayerHand : MonoBehaviour
{
    [SerializeField] private GameObject handAnchor;
    [SerializeField] private GameObject adsAnchor;
    [SerializeField] private float adsSpeed;
    [SerializeField] private float pickUpDist;
    private GameObject CurrentEquiped;
    [SerializeField] private LayerMask ignoreMask;
    [SerializeField] private Vector2 throwSpeed;
    [SerializeField] private Vector2 throwOffset;


    private bool isAiming = false;
    private void Start()
    {
    }
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
    public IUsable GetIUsable()
    {
        IUsable itemRef;
        if (CurrentEquiped != null && CurrentEquiped.TryGetComponent<IUsable>(out itemRef))
        {
            return itemRef;
        }
        return null;
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
    public ItemType GetCurrentItemType()
    {
        if(CurrentEquiped != null)
        {
            IUsable temp = GetIUsable();
            if(temp != null)
            {
                return temp.GetItemType();
            }
        }
        return null;

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
            UIManager.instance.UpdateAmmoFill(1);
            CameraController.instance.ResetOffset(true);
            UIManager.instance.UpdateCrosshairSpread(0);
            //======================================

            IUsable IRef;
            if (GetItem(out IRef))
            {
                Pickup _pickup = Instantiate(IRef.GetPickup(), transform.position, IRef.GetPickup().transform.rotation).GetComponent<Pickup>();
                _pickup.DropItem(transform.position + transform.forward * throwOffset.x + new Vector3(0, throwOffset.y, 0), Camera.main.transform.forward * throwSpeed.x + Vector3.up * throwSpeed.y, 1);
                BaseGun tRef = CurrentEquiped.GetComponent<BaseGun>();
                if (tRef != null)
                {
                    _pickup.storedClip = tRef.GetCurrAmmo();
                }
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
            IInteractable interactionRef;
            if(hit.transform.TryGetComponent<IInteractable>(out interactionRef))
            {
                interactionRef.TriggerInteraction();
                return true;
            }
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

    public void toggleADS()
    {
        if (CurrentEquiped != null)
        {
            isAiming = !isAiming;

            if (isAiming)
            {
                //CurrentEquiped.transform.position = adsAnchor.transform.position;
                CurrentEquiped.transform.parent = adsAnchor.transform;
                CurrentEquiped.transform.localPosition = Vector3.zero;

            }
            else
            {
                //CurrentEquiped.transform.position = handAnchor.transform.position;
                CurrentEquiped.transform.parent = handAnchor.transform;
                CurrentEquiped.transform.localPosition = Vector3.zero;
            }
        }
    }
}
