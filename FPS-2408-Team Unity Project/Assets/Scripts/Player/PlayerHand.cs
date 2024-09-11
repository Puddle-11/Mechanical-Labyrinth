using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.UI;
using static UnityEditor.Progress;

public class PlayerHand : MonoBehaviour
{
    [SerializeField] private GameObject handAnchor;
    [SerializeField] private GameObject centerAnchor;
    [SerializeField] private Transform sideAnchor;
    [SerializeField] private float adsSpeed;
    [SerializeField] private float handAnchorMoveMargin;
    [SerializeField] private float pickUpDist;
    private GameObject CurrentEquiped;
    [SerializeField] private LayerMask ignoreMask;


    [SerializeField] private float throwRotationSpeed;
    [SerializeField] private Vector2 throwSpeed;
    [SerializeField] private Vector2 throwOffset;

    private bool isAiming = false;
    private bool movingHandAnchor;
    #region MonoBehavior Methods
    private void Start()
    {

    }
    #endregion

    //========================================================
    //I really hate the "chain" of methods from the player controller
    //all the way to the currently equiped item, there are to many points of failure
    // - Rowan
    //========================================================


    #region Getters and Setters
    public float GetPickupDist(){ return pickUpDist;}
    public bool GetIsAiming() { return isAiming; }
    public GameObject GetCurrentEquipped()
    {
        return CurrentEquiped;
    }
    public void SetCurrentEquipped(GameObject _obj)
    {
        Debug.Log("Hit Current Equipped");
        if (CurrentEquiped == _obj) return;
       
        if(_obj == null)
        {
            UIManager.instance.AmmoDisplay(0, 0);
            UIManager.instance.UpdateAmmoFill(1);
        }
        else if (_obj.TryGetComponent(out BaseGun BGRef))
        {
            UIManager.instance.AmmoDisplay(BGRef.GetCurrAmmo(), BGRef.GetMaxClipSize());
            UIManager.instance.UpdateAmmoFill((float)BGRef.GetCurrAmmo() / BGRef.GetMaxClipSize());
        }
        else if (_obj.TryGetComponent(out IUsable usableRef))
        {
            UIManager.instance.AmmoDisplay(usableRef.GetPStats().uses, usableRef.GetPStats().maxUses);
            UIManager.instance.UpdateAmmoFill(usableRef.GetPStats().uses / (float)usableRef.GetPStats().maxUses);
        }
        CameraController.instance.ResetOffset(true);
        UIManager.instance.UpdateCrosshairSpread(0);
        ToggleADS(false);
        CurrentEquiped = _obj;
    }
    public void SetUseItem(bool _val)
    {
        IUsable itemRef;
        if (CurrentEquiped != null && CurrentEquiped.TryGetComponent<IUsable>(out itemRef))
        {
            itemRef.SetUsingItem(_val);
        }
    }
    public IUsable GetIUsable()
    {
        if (CurrentEquiped != null)
        {
            return CurrentEquiped.GetComponent<IUsable>();
        }
        return null;
    }
    public GameObject GetCurrentHand(){ return CurrentEquiped; }

    public ItemType GetCurrentItemType()
    {
        if(CurrentEquiped != null)
        {
            IUsable temp = GetIUsable();
            if (temp != null)
                return temp.GetItemType();
        }
        return null;
    }
    #endregion


    public void ClickPickUp()
    {
        if (!AttemptPickup())
            AttemptDrop();
    }
    private bool AttemptPickup()
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, pickUpDist, ~GameManager.instance.projectileIgnore))
        {
            if(hit.transform.TryGetComponent(out IInteractable interactionRef))
            {
                interactionRef.TriggerInteraction();
                return true;
            }

            if (hit.transform.GetComponent<Pickup>() != null) return true;
        }
        return false;
    }


    public bool AttemptDrop()
    {
        bool res = CurrentEquiped != null ? true : false;
        GeneralInventory.instance.DropItem();
        return res;
    }
    public GameObject GetHandAnchor() {return handAnchor;}
    public void PickupItem(ItemType t, Pickup p)
    {
        if(CurrentEquiped != null && !AttemptDrop())
        {
            return;
        }
        GeneralInventory.instance.AddItemToInventory(t, p);
        return;
    }



    public void ToggleADS(bool Aiming)
    {
        if (GetIUsable() != null && !GetIUsable().CanAim()) return;
        if (CurrentEquiped != null && movingHandAnchor == false)
        {
            isAiming = Aiming;
            UIManager.instance.ToggleADS(isAiming);
            float zoomAmnt = 0;
            if (GetCurrentHand().TryGetComponent(out BaseGun tempOut))
            {
                zoomAmnt = tempOut.GetZoomAmount();
            }
            if (isAiming)
            {
                CameraController.instance.SetFOV(CameraController.instance.GetDefaultFOV() - zoomAmnt);
                StartCoroutine(MoveHandAnchor(sideAnchor.transform.localPosition, centerAnchor.transform.localPosition, adsSpeed));
            }
            else
            {
                CameraController.instance.ResetFOV();
                StartCoroutine(MoveHandAnchor(centerAnchor.transform.localPosition, sideAnchor.transform.localPosition, adsSpeed));
            }
        }
    }


    public IEnumerator MoveHandAnchor(Vector3 _localStartPos, Vector3 _localEndPos, float _speed)
    {
        if (movingHandAnchor) yield break;
        movingHandAnchor = true;
        float timer = 0;
        while (timer < _speed)
        {
            if(Vector3.Distance(handAnchor.transform.localPosition, _localEndPos) < handAnchorMoveMargin)
            {
                handAnchor.transform.localPosition = _localEndPos;
                break;
            }

            handAnchor.transform.localPosition = Vector3.Lerp(handAnchor.transform.localPosition, _localEndPos, timer / _speed);

            timer += Time.deltaTime;
            yield return null;
        }
        movingHandAnchor = false;

    }
}
