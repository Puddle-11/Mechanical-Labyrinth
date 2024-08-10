using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
public class Damage : MonoBehaviour
{

    [SerializeField] private damageType type;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private int damageAmount;
    [SerializeField] private float defaulltSpeed;
    [SerializeField] private float deafultDestroyTime;
    public enum damageType
    {
        bullet,
        stationary,
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    public void FireBullet(int _damageAmount, float _speed, Vector3 _dir)
    {
        damageAmount = _damageAmount;
        if (type == damageType.bullet)
        {

            rb.velocity = _dir * _speed;
            Destroy(gameObject, deafultDestroyTime);
        }
        else
        {
            Debug.LogWarning("Attempted to fire a non-bullet type damage script at " + gameObject.name);
        }

    }

    public void FireBullet(int _damageAmount, float _speed) { FireBullet(_damageAmount, _speed, transform.forward); }
    public void FireBullet(int _damageAmount, Vector3 _dir) { FireBullet(_damageAmount, defaulltSpeed, _dir); }
    public void FireBullet(float _speed, Vector3 _dir) { FireBullet(damageAmount, _speed, _dir); }
    public void FireBullet(int _damageAmount){FireBullet(_damageAmount, defaulltSpeed);}
    public void FireBullet(float _speed) { FireBullet(damageAmount, _speed,transform.forward); }
    public void FireBullet(Vector3 _dir) { FireBullet(damageAmount,defaulltSpeed ,_dir); }
    public void FireBullet(){FireBullet(damageAmount, defaulltSpeed, transform.forward);}


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
