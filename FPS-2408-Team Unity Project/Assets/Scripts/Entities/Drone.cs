using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : BaseEnemy
{
    [SerializeField] private Vector3 magnitude;
    [SerializeField] private float speed;
    [SerializeField] private int explosionDamage;
    private float originalHeight;
    private float timer;
    public override void Start()
    {

        originalHeight = transform.position.y;
        base.Start();

    }

    public override void Update()
    {

        if (timer > Mathf.PI * 2) timer = 0;
        else timer += Time.deltaTime * speed;
        
        transform.position = new Vector3(transform.position.x, originalHeight + (Mathf.Sin(timer) * magnitude.y), transform.position.z);
        base.Update();

    }
    private void DamagePlayer()
    {

        GameManager.instance.playerControllerRef.UpdateHealth(-explosionDamage);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.instance.playerRef)
        {
            DamagePlayer();
            SetHealth(0);
        }
    }

}
