using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ammoDrop : MonoBehaviour
{
    [SerializeField] AmmoInventory.bulletType type;
    [SerializeField] int ammoAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.instance.playerRef)
        {
            AmmoInventory.instance.updateAmmoInventory(type, ammoAmount);
            Destroy(gameObject);
        }
    }
}
