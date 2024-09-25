using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] int Explosiondamage;
    [SerializeField] float radius;
    [SerializeField] GameObject grenade;
    Collider[] colliderref;
    [SerializeField] private float timer;
    [SerializeField] private float maxtimer;

    private void Start()
    {
        timer = maxtimer;
    }
    private void Update()
    {
        timer -= Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        Explosion();
    }
    private void Explosion()
    {
        colliderref = Physics.OverlapSphere(transform.position, radius);
        IHealth healthref;
        for (int i = 0; i < colliderref.Length; i++)
        {
            for (int j = 0; j < colliderref.Length; j++)
            {
                if (colliderref[i].gameObject == gameObject)
                {
                    return;
                }
                else
                {
                    if (colliderref[i].GetComponent<BaseEnemy>().GetCurrentShield() > 0 || colliderref[i].GetComponent<BaseEnemy>().GetCurrentHealth() > 0)
                    {
                        Debug.Log("hit enemy");
                        if (colliderref[i].GetComponent<BaseEnemy>().GetCurrentShield() <= 0)
                        {
                            colliderref[i].GetComponent<BaseEnemy>().SetCurrentShield(-Explosiondamage);
                            Destroy(grenade);
                        }
                        if (colliderref[i].GetComponent<BaseEnemy>().GetCurrentHealth() <= 0)
                        {
                            colliderref[i].GetComponent<BaseEnemy>().SetHealth(-Explosiondamage);
                            Destroy(grenade);
                        }
                    }
                }
            }
        }
    }
}
