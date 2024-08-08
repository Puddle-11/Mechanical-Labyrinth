using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private ItemType Item;
    private Rigidbody rb;
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
    public ItemType PickupItem()
    {
        if (Item == null)
        {
            Debug.LogWarning("Failed to pickup\n ItemType variable on " + gameObject.name + " Unassigned");
            return null;
        }
        gameObject.SetActive(false);
        return Item;
    }
    public bool DropItem(Vector3 _pos, Vector3 _velocity)
    {
        gameObject.transform.position = _pos;
        gameObject.SetActive(true);
        if (rb != null)
        {
            rb.velocity = _velocity;
        }
        return true;
    }
    public bool DropItem(Vector3 _pos)
    {
        return DropItem(_pos, Vector3.zero);
    }
}
