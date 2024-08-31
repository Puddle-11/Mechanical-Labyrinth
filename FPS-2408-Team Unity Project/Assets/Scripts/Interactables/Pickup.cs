using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Pickup : MonoBehaviour
{
    [SerializeField] private ItemType Item;
    private Rigidbody rb;
    [SerializeField] private string stats;
     public int storedClip;
    public string GetStats()
    {
        IUsable temp;
        if(Item.Object.TryGetComponent<IUsable>(out temp))
        {
            return Item.itemName + "\n"+temp.GetItemStats();
        }
        return stats;
    }
    private void Awake()
    {
        if(!TryGetComponent<Rigidbody>(out rb))
        {
            Debug.LogWarning("Failed to fetch rigidbody on " +gameObject.name);
        }
    }
    public ItemType GetItem()
    {
        if (Item == null) return null;
        return Item;
    }
    public void PickupItem(out GameObject _ref, Vector3 _pos, Quaternion _rotation, Transform _parent)
    {
        if (Item == null)
        {
            Debug.LogWarning("Failed to pickup\n ItemType variable on " + gameObject.name + " Unassigned");
            _ref = null;
            return;
        }
        _ref = Instantiate(Item.Object, _pos, _rotation, _parent);


        if(_ref.GetComponent<IUsable>() != null)
        {
            _ref.GetComponent<IUsable>().SetPickup(Item.Pickup);
        }
        if (_ref.GetComponent<BaseGun>() != null)
        {
            _ref.GetComponent<BaseGun>().SetPlayerGun(true); 
            _ref.GetComponent<BaseGun>().SetAmmo(storedClip);
        }

        Destroy(gameObject);
    }

    public bool DropItem(Vector3 _pos, Vector3 _velocity, float rotationalSpeed)
    {
        gameObject.transform.position = _pos;
        gameObject.SetActive(true);
        if (rb != null)
        {
            rb.velocity = _velocity;
            rb.angularVelocity = new Vector3(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180)).normalized * rotationalSpeed;
        }
        gameObject.transform.localEulerAngles = new Vector3(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180));

        return true;
    }
    public bool DropItem(Vector3 _pos)
    {
        return DropItem(_pos, Vector3.zero, 1);
    }
}
