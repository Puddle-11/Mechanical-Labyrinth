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
        [Range(0, 1)]
        public float volume;
        [Range(-1, 1)]
        public float pitch;
        public bool loop;

    }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(instance);

    }
    private void Start()
    {
        InitializeConstSounds();
    }
    public void InitializeConstSounds()
    {
        if (SettingsController.instance == null)
        {
            Debug.LogWarning("No settings found. failed to initialize background sounds");
            return;
        }
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
    public void UpdateConstSounds()
    {
        for (int i = 0; i < continuousSounds.Length; i++)
        {
            continuousSounds[i].source.volume = continuousSounds[i].volume * SettingsController.instance.GetGeneralVolume() * SettingsController.instance.GetTypeVolume(SettingsController.soundType.environmental);
        }
    }
    public void PlaySound(AudioClip _sound, float _volume)
    {
        PlaySound(soundPlayer, _sound, SettingsController.soundType.general, _volume);

    }
    public void PlaySound(AudioClip _sound, SettingsController.soundType _type, float _volume)
    {
        PlaySound(soundPlayer, _sound, _type, _volume);
    }
    public void PlaySound(AudioClip _sound, SettingsController.soundType _type)
    {
        PlaySound(soundPlayer, _sound, _type);
    }
    public void PlaySound(AudioSource _source, AudioClip _sound, SettingsController.soundType _type = SettingsController.soundType.general, float _volume = 1)
    {
        if (SettingsController.instance != null)
        {
            if (_type == SettingsController.soundType.general)
            {
                _source.PlayOneShot(_sound, _volume * SettingsController.instance.GetGeneralVolume() * SettingsController.instance.GetTypeVolume(_type));
            }
            else
            {
                _source.PlayOneShot(_sound, _volume * SettingsController.instance.GetTypeVolume(_type));
            }
            return;
        }
        Debug.Log("No settings found, failed to play sound");
    }

}
