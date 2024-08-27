using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeInputReceiver : MonoBehaviour
{
    private int index;
    private Rope ropeRef;
    [SerializeField] private float triggerDelay;
    private float timer;

    public void SetRopeRef(Rope _reference)
    {
        ropeRef = _reference;

    }
    public void SetIndex(int _index)
    {
        index = _index;
    }
    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = 0;
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        if (timer == 0)
        {
            ropeRef.RopeInput(other.transform.position, index);
            timer = triggerDelay;
        }


    }
}
