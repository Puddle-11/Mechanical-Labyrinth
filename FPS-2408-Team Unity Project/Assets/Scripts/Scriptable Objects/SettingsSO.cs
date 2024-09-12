using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Custom Assets/Settings Preset")]
public class SettingsSO : ScriptableObject
{

    public float S_cameraSensitivity;
    [Header("Volume")]
    public float S_generalVol = 1;
    public float S_SXFVol = 1;
    public float S_environmentalVol = 1;
    public float S_enemyVol = 1;
    public float S_playerVol = 1;
    public float S_uiSFXVol = 1;
}
