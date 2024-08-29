using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
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
            if (other.GetComponent<PlayerController>() != null)
            {
                GameManager.instance.playerControllerRef.SetPlayervel(SpringDirection.normalized * Springforce);
                Vector3.Reflect(SpringDirection.normalized * Springforce, SpringVelocity);
            }
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
