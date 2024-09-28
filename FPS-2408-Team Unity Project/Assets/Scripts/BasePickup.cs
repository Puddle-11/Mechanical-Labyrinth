using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BasePickup : MonoBehaviour, IInteractable
{ 
    public virtual string GetStats()
    {
        return "";
    }

    public virtual void TriggerInteraction()
    {
     
    }

 
}
