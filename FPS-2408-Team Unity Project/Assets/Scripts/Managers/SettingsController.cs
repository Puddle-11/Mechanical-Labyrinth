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
        soundType type = (soundType) _type;

        switch (type)
        {
            case soundType.general:
                return settings.S_generalVol;
            case soundType.environmental:

                return settings.S_environmentalVol;
            case soundType.SFX:

                return settings.S_SXFVol;
            case soundType.enemy:
                return settings.S_enemyVol;
            case soundType.player:
                return settings.S_playerVol;
            case soundType.uiSFX:
                return settings.S_uiSFXVol;
        }
        return 1;
    }
    public void SetTypeVolume(int _index, float _volume)
    {
        soundType type = (soundType)_index;
        switch (type)
        {
            case soundType.general:
                settings.S_generalVol = _volume;
                break;
            case soundType.environmental:
                settings.S_environmentalVol = _volume;
                break;
            case soundType.SFX:
                settings.S_SXFVol = _volume;
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
        TriggerSettingsUpdate();
    }
    public void SetTypeVolume(soundType _type, float _volume)
    {
        SetTypeVolume((int)_type, _volume);
    }
}
