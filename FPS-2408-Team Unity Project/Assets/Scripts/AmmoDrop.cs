using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ammoDrop : MonoBehaviour, IInteractable
{
    [SerializeField] AmmoInventory.bulletType type;
    [SerializeField] int ammoAmount;

    public void TriggerInteraction()
    {


        AmmoInventory.instance.UpdateAmmoInventory(type, ammoAmount);
        Destroy(gameObject);

    }

}
