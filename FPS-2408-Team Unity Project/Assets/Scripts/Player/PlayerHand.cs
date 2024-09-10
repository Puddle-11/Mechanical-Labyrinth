using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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
        if(GameManager.instance != null)
        {
          ItemType t =   GameManager.instance.GetCurrentItemType();
            if(t != null)
            {
                PickupItem(t, null);
            }
        }
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
    public void SetCurrentEquipped(GameObject _obj)
    {

        if (CurrentEquiped == _obj) return;
        if (CurrentEquiped != null)
        {
            Destroy(CurrentEquiped);
        }
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
            IInteractable interactionRef;
            if(hit.transform.TryGetComponent<IInteractable>(out interactionRef))
            {


                interactionRef.TriggerInteraction();
                return true;
            }

            if (hit.transform.GetComponent<Pickup>() != null) return true;
        }
        return false;
    }

    public void DropItem()
    {
        //======================================
        //External Resets
        UIManager.instance.AmmoDisplay(0, 0);
        UIManager.instance.UpdateAmmoFill(1);
        CameraController.instance.ResetOffset(true);
        UIManager.instance.UpdateCrosshairSpread(0);
        //======================================


        //======================================
        //Internal Resets
        // GeneralInventory.instance.Hotbar[GeneralInventory.instance.selectedSlot] = null;
        GeneralInventory.instance.SetSlot(null);
        Destroy(CurrentEquiped);
        CurrentEquiped = null;
        //======================================

    }

    public bool AttemptDrop()
    {
        GeneralInventory.instance.SetSlot(null);
        if (CurrentEquiped != null)
        {
            //======================================
            //Generate Dropped Item
            IUsable IRef;
            if (CurrentEquiped.TryGetComponent(out IRef))
            {
                GameObject dropObj = Instantiate(IRef.GetPickup(), transform.position, IRef.GetPickup().transform.rotation);
                Rigidbody rb;
                Pickup pRef = dropObj.GetComponent<Pickup>();
                dropObj.transform.position = transform.position + transform.forward * throwOffset.x + new Vector3(0, throwOffset.y, 0);
                dropObj.SetActive(true);

                //Set rotation
                dropObj.transform.localEulerAngles = new Vector3(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180));
                //Apply velocity if RB doesnt == null
                if (dropObj.TryGetComponent(out rb))
                {
                    rb.velocity = Camera.main.transform.forward * throwSpeed.x + Vector3.up * throwSpeed.y;
                    rb.angularVelocity = new Vector3(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180)).normalized * throwRotationSpeed;
                }

                if (pRef != null) pRef.currPStats = IRef.GetPStats();
            }
            //======================================
            DropItem();
            return true;
        }
        DropItem();

        return false;
    }
    public void PickupItem(ItemType t, Pickup p, bool dontDrop = false)
    {
        if (!dontDrop)
        {
            //if we are holding an item and we cant drop it, then break away
            if (CurrentEquiped != null && !AttemptDrop()) return;
        }
        

        //Check if item passed is actually an item
        if (t == null)
        {
            Debug.LogWarning("Failed to pickup\n ItemType variable on " + p.gameObject.name + " Unassigned");
            GameManager.instance.playerControllerRef.GetPlayerHand().SetCurrentEquipped(null);
            return;
        }

        GeneralInventory.instance.SetSlot(t);

        //Instantiate item in hand
        GameObject _ref = Instantiate(t.Object, handAnchor.transform.position, handAnchor.transform.rotation, handAnchor.transform);

        //If item is a gun or a weapon set weapon to player weapon
        BaseGun bgRef;
        if (_ref.TryGetComponent(out bgRef)) bgRef.SetPlayerGun(true);

        IUsable useRef;
        if (_ref.TryGetComponent(out useRef))
        {
            useRef.SetPickup(t.Pickup);
            if (p != null)
                useRef.SetPStats(p.currPStats);
            else
                useRef.SetPStats(t.defaultStats);
        }

        SetCurrentEquipped(_ref);
        GameManager.instance.SetCurrentItem(t);

        if (p != null) Destroy(p.gameObject);

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
