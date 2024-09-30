using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BasePickup : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private float pickupVolume = 0.5f;
    public virtual string GetStats()
    {
        return "";
    }

    public virtual void TriggerInteraction()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlaySound(pickupSound, SettingsController.soundType.SFX, pickupVolume);
    }

 
}
