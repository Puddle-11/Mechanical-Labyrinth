using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ammoDrop : MonoBehaviour, IInteractable
{

    [SerializeField] private AmmoInventory.bulletType type;
    [SerializeField] private Vector2Int amountRange;
    private int ammoAmount;

    public void Start()
    {
        ammoAmount = Random.Range(amountRange.x, amountRange.y);
    }

    public string GetStats()
    {
        return "Type: " + AmmoInventory.instance.GetTypeName(type) + "\n" + "Amount: " + ammoAmount.ToString();
    }

    public void TriggerInteraction()
    {
        AmmoInventory.instance.UpdateAmmoInventory(type, ammoAmount);
        Destroy(gameObject);
    }

}
