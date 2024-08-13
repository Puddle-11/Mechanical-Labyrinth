using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : BaseEntity
{
    [SerializeField] GameObject door;
    [SerializeField] private bool inRange = false;

    public override void Start()
    {

        base.Start();

    }

    public override void Update()
    {
        if (inRange == true)
        {
            door.SetActive(false);
        }
        else
        {
            door.SetActive(true);
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
