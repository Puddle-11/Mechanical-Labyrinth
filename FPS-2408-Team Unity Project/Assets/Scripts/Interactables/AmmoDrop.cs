using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ammoDrop : BasePickup
{

    [SerializeField] private AmmoInventory.bulletType type;
    [SerializeField] private Vector2Int amountRange;
    private int ammoAmount;
    #region MonoBehavior Methods
    public void Start()
    {
        ammoAmount = Random.Range(amountRange.x, amountRange.y);
    }
    #endregion

    #region IInteractable Methods
    public override string GetStats()
    {
        return "Ammo Type: " + AmmoInventory.instance.GetTypeName(type) + "\n\nAmount: " + ammoAmount.ToString();
    }
    public override void TriggerInteraction()
    {
        AmmoInventory.instance.UpdateAmmoInventory(type, ammoAmount);
        Destroy(gameObject);
    }
    #endregion

}
