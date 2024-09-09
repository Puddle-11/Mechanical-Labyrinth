using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioManager;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private SettingsSO settings;
    public static SettingsController instance;
    public enum soundType
    {
        SFX,
        environmental,
        enemy,
        player,
        uiSFX,
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Two settings controllers found in scene\nDestroyed instance at " + gameObject.name);
            Destroy(this);
        }
    }
    public SettingsSO GetSettings()
    {
        return settings;
    }
    public float GetGeneralVolume()
    {
        return settings.S_generalVol;
    }
    public void SetSensitivity()
    {

    }
    public float GetTypeVolume(soundType _type)
    {
        switch (_type)
        {
            case soundType.SFX:
                return settings.S_SXFVol;
            case soundType.environmental:
                return settings.S_environmentalVol;
            case soundType.enemy:
                return settings.S_enemyVol;
            case soundType.player:
                return settings.S_playerVol;
            case soundType.uiSFX:
                return settings.S_SXFVol;
        }
        return 1;
    }
    public void SetTypeVolume(soundType _type, float _volume)
    {
        switch (_type)
        {
            case soundType.SFX:
                settings.S_SXFVol = _volume;
                break;
            case soundType.environmental:
                settings.S_environmentalVol = _volume;
                break;
            case soundType.enemy:
                settings.S_enemyVol = _volume;
                break;
            case soundType.player:
                settings.S_playerVol = _volume;
                break;
            case soundType.uiSFX:
                settings.S_uiSFXVol = _volume;
                break;
        }
    }
}
