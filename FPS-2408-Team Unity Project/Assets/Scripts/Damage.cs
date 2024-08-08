using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
public class Damage : MonoBehaviour
{

    [SerializeField] private damageType type;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private int damageAmount;
    [SerializeField] private float speed;
    [SerializeField] private float destroyTime;
    public enum damageType
    {
        bullet,
        stationary,
    }
    // Start is called before the first frame update
    void Start()
    {
        if (type == damageType.bullet)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        IHealth healthRef;
        if(other.TryGetComponent<IHealth>(out healthRef))
        {
            healthRef.UpdateHealth(-damageAmount);
        }
        Destroy(gameObject);
    }
}
