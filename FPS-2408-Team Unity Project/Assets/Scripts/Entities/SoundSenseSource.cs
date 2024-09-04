using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//====================================
//REWORKED
//====================================
public class SoundSenseSource : MonoBehaviour
{
    public delegate void SendSound(Vector3 _pos);
    public SendSound makeSound;
    public void TriggerSound(Vector3 _pos)
    {
        makeSound?.Invoke(_pos);
    }
}
