using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioSource soundPlayer;
    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(instance);
    }
    public void PlaySound(AudioClip _sound, float _volume)
    {
        if (SettingsController.instance != null) soundPlayer.PlayOneShot(_sound, _volume * SettingsController.instance.GetGeneralVolume());
        else Debug.LogWarning("No Settings found, failed to play sound");
    }
    public void PlaySound(AudioClip _sound, SettingsController.soundType _type)
    {
        if (SettingsController.instance != null)
        {
            PlaySound(_sound, SettingsController.instance.GetTypeVolume(_type));
        }
        else
        {
            Debug.LogWarning("No Settings found, failed to play sound");
        }
    }
    public void PlaySound(AudioClip _sound, SettingsController.soundType _type, float _volMod)
    {
        if (SettingsController.instance != null)
        {
            PlaySound(_sound, SettingsController.instance.GetTypeVolume(_type) * _volMod);
        }
        else
        {
            Debug.LogWarning("No Settings found, failed to play sound");
        }
    }
}
