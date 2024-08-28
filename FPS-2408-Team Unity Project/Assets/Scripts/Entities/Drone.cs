using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    [SerializeField] private Vector3 magnitude;
    private Vector3 newPos;
    [SerializeField] private float speed;
    [SerializeField] private GameObject drone;
    [SerializeField] private float explodeTime;
    [SerializeField] private int explosionDamage;
    private bool exploded = false;
    private float timer;
    private float originalHeight;

    [SerializeField] private bool inRange = false;
    [SerializeField] private Collider coll;

    private void Start()
    {
        originalHeight = drone.transform.position.y;

    }

    public void Update()
    {

        if (timer > Mathf.PI * 2)
        {
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime * speed;
        }
        newPos = new Vector3(transform.position.x, originalHeight + (Mathf.Sin(timer) * magnitude.y), transform.position.z);
        transform.position = newPos;

        if (inRange == true)
        {
            drone.SetActive(false);
            coll.enabled = false;

            if (exploded == true) {
                if (inRange == true)
                {
                    IHealth healthRef = null;
                    GameManager.instance.playerRef.TryGetComponent<IHealth>(out healthRef);
                    healthRef.UpdateHealth(-explosionDamage);
                }
                
                Destroy(drone);
            }
            

        }
        else
        {
            drone.SetActive(true);
            coll.enabled = true;
        }
    }
    private IEnumerator DroneExplodeDelay()
    {
        yield return new WaitForSeconds(explodeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.instance.playerRef)
        {
            inRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == GameManager.instance.playerRef)
        {
            inRange = false;
        }
    }
}
