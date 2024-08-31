using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    [SerializeField] Vector3 SpringVelocity;
    [SerializeField] Vector3 SpringDirection;
    [SerializeField] float Springforce;
    bool onspring;
 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            onspring = true;
            other.GetComponent<PlayerController>();
            
             GameManager.instance.playerControllerRef.GetPlayervel();
             GameManager.instance.playerControllerRef.SetPlayervel(SpringDirection.normalized * Springforce);
             GameManager.instance.playerControllerRef.UpdateJumpAmount(1);
        }
           else 
           {
            onspring = false;
           }
    }
    
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
