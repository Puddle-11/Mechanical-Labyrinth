using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupCollision : MonoBehaviour
{
    [SerializeField] private BasePickup p;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.instance.playerRef)
        {
            
                p.TriggerInteraction();
            
        }
    }
}
