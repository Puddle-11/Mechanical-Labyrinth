using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftCollision : MonoBehaviour
{
    private bool inTrigger;
    [SerializeField] private float pushBackForce;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.instance.playerRef)
        {
            inTrigger = true;
        }

    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject == GameManager.instance.playerRef)
        {
            inTrigger = false;
        }

    }
    void Update()
    {
        if (inTrigger)
        {
            Vector3 filterPos = new Vector3(transform.position.x, GameManager.instance.playerRef.transform.position.y, transform.position.z);
            GameManager.instance.playerControllerRef.UpdateForce((GameManager.instance.playerRef.transform.position - transform.position).normalized * Time.deltaTime * pushBackForce);
        }
    }
}
