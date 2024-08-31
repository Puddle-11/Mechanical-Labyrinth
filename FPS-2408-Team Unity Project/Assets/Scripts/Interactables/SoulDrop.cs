using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulDrop : MonoBehaviour, IInteractable
{
    [SerializeField] int healAmount;
    public string GetStats()
    {
        return "Heal: " + healAmount;
    }

    public void TriggerInteraction()
    {
        GameManager.instance.playerControllerRef.UpdateHealth(healAmount);
        Destroy(gameObject);
    }
}
