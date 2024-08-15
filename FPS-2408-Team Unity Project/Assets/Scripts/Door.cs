using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : BaseEntity
{
    [SerializeField] GameObject door;
    [SerializeField] private bool inRange = false;
    [SerializeField] private Collider coll;
    public override void Start()
    {

        base.Start();

    }

    public override void Update()
    {
        if (inRange == true)
        {
            door.SetActive(false);
            coll.enabled = false;

        }
        else
        {
            door.SetActive(true);
            coll.enabled = true;
        }
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
    // Start is called before the first frame update
}
