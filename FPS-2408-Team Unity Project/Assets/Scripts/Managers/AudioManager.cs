using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioSource soundPlayer;
    [SerializeField] private float generalVol = 1;
    [SerializeField] private float SXFVol = 1;
    [SerializeField] private float environmentalVol = 1;
    [SerializeField] private float enemyVol = 1;
    [SerializeField] private float playerVol = 1;
    [SerializeField] private float uiSFXVol = 1;

    public enum soundType
    {
        SFX,
        environmental,
        enemy,
        player,
        uiSFX,
    }
    public float GetTypeVolume(soundType _type)
    {
        switch (_type)
        {
            case soundType.SFX:
                return SXFVol;
            case soundType.environmental:
                return environmentalVol;
            case soundType.enemy:
                return enemyVol;
            case soundType.player:
                return playerVol;
            case soundType.uiSFX:
                return uiSFXVol;
        }
        return 1;
    }
    public void SetTypeVolume(soundType _type, float _volume) {

        switch (_type)
        {
            case soundType.SFX:
                 SXFVol = _volume;
                break;
            case soundType.environmental:
                environmentalVol = _volume;
                break;
            case soundType.enemy:
                enemyVol = _volume;
                break;
            case soundType.player:
                playerVol = _volume;
                break;
            case soundType.uiSFX:
                uiSFXVol = _volume;
                break;
        }
    }
 
    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(instance);
    }
    public void PlaySound(AudioClip _sound, float _volume)
    {
        soundPlayer.PlayOneShot(_sound, _volume * generalVol);
    }
    public void PlaySound(AudioClip _sound, soundType _type)
    {
        PlaySound(_sound, GetTypeVolume(_type));
    }
    public void PlaySound(AudioClip _sound, soundType _type, float _volMod)
    {
        PlaySound(_sound, GetTypeVolume(_type) * _volMod);

    }
}
