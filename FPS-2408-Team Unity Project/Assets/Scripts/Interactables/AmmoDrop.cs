using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ammoDrop : MonoBehaviour, IInteractable
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
    public string GetStats()
    {
        return "Type: " + AmmoInventory.instance.GetTypeName(type) + "\n" + "Amount: " + ammoAmount.ToString();
    }
    public void TriggerInteraction()
    {
        AmmoInventory.instance.UpdateAmmoInventory(type, ammoAmount);
        Destroy(gameObject);
    }
    #endregion

}
