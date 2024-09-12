using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioSource soundPlayer;
    [SerializeField] private ConstSound[] continuousSounds;

    [System.Serializable]
    public struct ConstSound
    {
        public AudioClip clip;
        [HideInInspector] public AudioSource source;
        [Range(0,1)]
        public float volume;
        [Range(-1, 1)]
        public float pitch;
        public bool loop;

    }

    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(instance);

    }
    private void Start()
    {
        for (int i = 0; i < continuousSounds.Length; i++)
        {
            if (continuousSounds[i].source == null)
            {
                continuousSounds[i].source = gameObject.AddComponent<AudioSource>();
                continuousSounds[i].source.clip = continuousSounds[i].clip;
                continuousSounds[i].source.pitch = continuousSounds[i].pitch;
                continuousSounds[i].source.volume = continuousSounds[i].volume * SettingsController.instance.GetGeneralVolume() * SettingsController.instance.GetTypeVolume(SettingsController.soundType.environmental);
                continuousSounds[i].source.loop = continuousSounds[i].loop;
            continuousSounds[i].source.Play();
            }
        }
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
