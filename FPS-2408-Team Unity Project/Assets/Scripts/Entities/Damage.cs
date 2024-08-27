using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
public class Damage : MonoBehaviour
{

    [SerializeField] private damageType type;
    [SerializeField] private int damageAmount;
    [SerializeField] private float defaulltSpeed;
    private IHealth currRef;
    private float timer;
    public enum damageType
    {
        stationary,
        continuous,
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        IHealth healthRef;
        if(other.TryGetComponent<IHealth>(out healthRef))
        {
            currRef = healthRef;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger) return;
        IHealth healthRef;
        if (other.TryGetComponent<IHealth>(out healthRef))
        {
            if (currRef == healthRef)
            {

                currRef = null;
            }
        }
    }
    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        if(timer <= 0)
        {
            if (currRef != null)
            {
                currRef.UpdateHealth(-damageAmount);
            }
            timer = defaulltSpeed;
        }
    }

}
