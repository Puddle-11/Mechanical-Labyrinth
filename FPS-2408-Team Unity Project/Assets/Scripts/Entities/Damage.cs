using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
public class Damage : MonoBehaviour
{

    [SerializeField] private damageType type;
    [SerializeField] private int damageAmount;
    [SerializeField] private float damageSpeed;
    private IHealth currRef;
    private float timer;
    public enum damageType
    {
        single,
        continuous,
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        if(other.gameObject == GameManager.instance.playerRef)
        {
            if (type == damageType.single)
                GameManager.instance.playerControllerRef.UpdateHealth(-damageAmount);
            else
                currRef = GameManager.instance.playerControllerRef;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger) return;
        if (other.gameObject == GameManager.instance.playerRef)
        {
            if (type == damageType.continuous)
            {
                currRef = null;
            }
        }
    }

    private void Update()
    {
        if (type == damageType.continuous)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            if (timer <= 0)
            {
                if (currRef != null)
                {
                    currRef.UpdateHealth(-damageAmount);
                }
                timer = damageSpeed;
            }
        }
    }

    }
