using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPad : MonoBehaviour
{
    //[SerializeField] int HealAmount;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.instance.playerRef)
        {
            HealPlayer();
            
        }
    }
    void HealPlayer() {
        GameManager.instance.playerControllerRef.ResetHealth();

    }
    // Update is called once per frame
    void Update()
    {
        
    }
   
}
