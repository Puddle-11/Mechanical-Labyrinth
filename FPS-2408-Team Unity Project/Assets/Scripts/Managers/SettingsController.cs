using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static AudioManager;

public class SettingsController : MonoBehaviour
{
    public static SettingsController instance;
    [SerializeField] private SettingsSO settings;


    public enum soundType
    {
        general,
        environmental,
        SFX,
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
    public void SetSensitivity(float _val)
    {
        settings.S_cameraSensitivity = _val;
        TriggerSettingsUpdate();
    }
    public void TriggerSettingsUpdate()
    {
        if(AudioManager.instance != null)
        {
            AudioManager.instance.UpdateConstSounds();
        }
    }
    public float GetTypeVolume(soundType _type)
    {

        return GetTypeVolume((int)_type);
    }
    public float GetTypeVolume(int _type)
    {
        switch (_type)
        {
            case 0:
                return settings.S_generalVol;
            case 1:
                return settings.S_environmentalVol;
            case 2:
                return settings.S_SXFVol;
            case 3:
                return settings.S_enemyVol;
            case 4:
                return settings.S_playerVol;
            case 5:
                return settings.S_uiSFXVol;
        }
        return 1;
    }
    public void SetTypeVolume(int _index, float _volume)
    {

        switch (_index)
        {
            case 0:
                settings.S_generalVol = _volume;
                break;
            case 1:
                settings.S_SXFVol = _volume;
                break;
            case 2:
                settings.S_environmentalVol = _volume;
                break;
            case 3:
                settings.S_enemyVol = _volume;
                break;
            case 4:
                settings.S_playerVol = _volume;
                break;
            case 5:
                settings.S_uiSFXVol = _volume;
                break;

        }
        TriggerSettingsUpdate();
    }
    public void SetTypeVolume(soundType _type, float _volume)
    {
        SetTypeVolume((int)_type, _volume);
    }
}
