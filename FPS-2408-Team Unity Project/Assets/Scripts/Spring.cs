using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class Spring : MonoBehaviour
{
    [SerializeField] Vector3 SpringDirection;
    [SerializeField] float Springforce;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") {
            other.GetComponent<PlayerController>();
            if (other.GetComponent<PlayerController>() != null) {
              GameManager.instance.playerControllerRef.SetPlayervel(SpringDirection.normalized * Springforce);  
              
            }
            
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
