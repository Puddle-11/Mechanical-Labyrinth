using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Assets/Bullet Preset")]
public class PenetrationSettings : ScriptableObject
{
    public float B_penetrationDist;
    public int B_maxObjectsToPenetrate;
    public float B_damageRetentionPercent; 
}
