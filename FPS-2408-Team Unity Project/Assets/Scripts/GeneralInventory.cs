using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

public class GeneralInventory : MonoBehaviour
{
    //make list of ItemSlot names Hotbar.
    public int selectedSlot;

    //List of ItemSlots

    //array of hotbar UI elements


    public static GeneralInventory instance;

    [SerializeField] private int numOfslots;
    [SerializeField] private float throwRotationSpeed;
    [SerializeField] private Vector2 throwSpeed;
    [SerializeField] private Vector2 throwOffset;
    [SerializeField] private ItemType respawnItem;
    public HotbarSlot[] Hotbar;

    [System.Serializable]
    public struct HotbarSlot
    {
        public ItemType t;
        public Image UIImage;
        public GameObject obj;
    }

    public ItemType GetRespawnItemType() {return respawnItem;}
    public int GetInventorySize() { return numOfslots;}

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        Hotbar = new HotbarSlot[numOfslots];
    }

    public void ImportInventory()
    {
        if (GameManager.instance != null)
        {
            ItemType[] temp = GameManager.instance.GetGeneralInventory();
            if (temp != null && temp.Length > 0)
            {
                for (int i = 0; i < temp.Length; i++)
                {
                    if (i >= Hotbar.Length) break;
                    AddItemToInventory(temp[i], i);
                }
            }
        }
        GameManager.instance.playerControllerRef.GetPlayerHand().SetCurrentEquipped(Hotbar[selectedSlot].obj);

    }
    public void Update()
    {
        SelectItem();
        GetNextFreeIndex(out _);
    }
    public void AddItemToInventory(ItemType t, Pickup p = null)
    {
        AddItemToInventory(t, selectedSlot, p);
    }
    public void AddItemToInventory(ItemType t, int _index, Pickup p = null)
    {

        if (Hotbar[_index].t != null)
        {
            return;
        }

        if (t == null)
        {
            Debug.LogWarning("Failed to pickup\n ItemType variable Unassigned");
            GameManager.instance.playerControllerRef.GetPlayerHand().SetCurrentEquipped(null);
            return;
        }

        SetSlot(_index, t);
        GameObject handAnchor = GameManager.instance.playerControllerRef.GetPlayerHand().GetHandAnchor();
        //Instantiate item in hand
        GameObject _ref = Instantiate(t.Object, handAnchor.transform.position, handAnchor.transform.rotation, handAnchor.transform);
        Hotbar[_index].obj = _ref;
        if (_index != selectedSlot) _ref.SetActive(false);
        UpdateSelectedObj();
        //If item is a gun or a weapon set weapon to player weapon
                BaseGun bgRef;
        if (_ref.TryGetComponent(out bgRef)) bgRef.SetPlayerGun(true);

        //if item is usable set its stats
        IUsable useRef;
        if (_ref.TryGetComponent(out useRef))
        {
            useRef.SetPickup(t.Pickup);
            if (p != null)
                useRef.SetPStats(p.currPStats);
            else
                useRef.SetPStats(t.defaultStats);
        }

        if (p != null) Destroy(p.gameObject);

        UIManager.instance.SetSlotIcon(t.Icon, _index);
        GameManager.instance.playerControllerRef.GetPlayerHand().SetCurrentEquipped(Hotbar[selectedSlot].obj);
        GameManager.instance.SetGeneralInventory(ConvertHotbarToItem(Hotbar));
    }

    public ItemType[] ConvertHotbarToItem(HotbarSlot[] _Hotbar)
    {
        ItemType[] temp = new ItemType[_Hotbar.Length];
        for (int i = 0; i < temp.Length; ++i)
        {
            temp[i] = _Hotbar[i].t;
        }

       return temp;
    }

    public void DropItem()
    {
        DropItem(selectedSlot);
    }
    public void DropItem(int _index)
    {
        SpawnDrop(Hotbar[_index].obj);
        if (_index == selectedSlot)
        {
            UIManager.instance.AmmoDisplay(0, 0);
            UIManager.instance.UpdateAmmoFill(1);
            UIManager.instance.UpdateCrosshairSpread(0);
            GameManager.instance.playerControllerRef.GetPlayerHand().SetCurrentEquipped(null);

        }

        ResetSlot(_index);
    }
    public void ResetSlot(int _index)
    {
        Destroy(Hotbar[_index].obj);
        Hotbar[_index].obj = null;
        SetSlot(_index, null);
        UIManager.instance.SetSlotIcon(_index);
    }
    public void SpawnDrop(GameObject _obj)
    {
        if (_obj == null) return;
        IUsable IRef;
        if (_obj.TryGetComponent(out IRef))
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
                rb.linearVelocity = Camera.main.transform.forward * throwSpeed.x + Vector3.up * throwSpeed.y;
                rb.angularVelocity = new Vector3(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180)).normalized * throwRotationSpeed;
            }

            if (pRef != null) pRef.currPStats = IRef.GetPStats();
        }
    }
    void SelectItem()
    {
        if (GameManager.instance != null && GameManager.instance.GetStatePaused())
        {
            return;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            //if at top of list, loop to bottom
            selectedSlot = (selectedSlot + 1) % Hotbar.Length;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            //if at bottom of list, loop to top
            selectedSlot = selectedSlot <= 0 ? Hotbar.Length - 1 : selectedSlot - 1;
        }
        else
        {
            //if no change, dont run any methods
            return;
        }
        GameManager.instance.playerControllerRef.GetPlayerHand().SetCurrentEquipped(Hotbar[selectedSlot].obj);
        GameManager.instance.playerControllerRef.GetPlayerHand().ToggleADS(false);
        UpdateSelectedObj();
    }
    public bool Contains(ItemType t, out int _index)
    {
        for (int i = 0; i < numOfslots; i++)
        {
            if (Hotbar[i].t == t)
            {

                _index = i;
                return true;
            }
        }
        _index = 0;
        return false;

    }
    public bool Contains(ItemType t)
    {
        return Contains(t, out _);
    }
    void UpdateSelectedObj()
    {
        for (int i = 0; i < Hotbar.Length; i++)
        {
            if (Hotbar[i].obj != null)
            {

                if (i != selectedSlot)
                {
                    Hotbar[i].obj.SetActive(false);
                }
            }
        }
        if (Hotbar[selectedSlot].obj != null) Hotbar[selectedSlot].obj.SetActive(true);
        UIManager.instance.UpdateSelectionHover(selectedSlot);

    }
    public int GetSelectedIndex()
    {
        return selectedSlot;
    }
    public ItemType GetSlot(int _index) { return Hotbar[_index].t;}
    public void SetSlot(int _index, ItemType t) {
        
        Hotbar[_index].t = t;
    
    }
    public void SetSlot(ItemType t) {
        SetSlot(selectedSlot, t);
        GameManager.instance.playerControllerRef.GetPlayerHand().SetCurrentEquipped(Hotbar[selectedSlot].obj);
    }
    public bool GetNextFreeIndex(out int result)
    {
        for(int i = 0;i < Hotbar.Length;i++) 
        {
            if (Hotbar[i].t == null)
            {
                result = i;
                return true;
            }
        }

        result = 0;
        return false;
    }
}
