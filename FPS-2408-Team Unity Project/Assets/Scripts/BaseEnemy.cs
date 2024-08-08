using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : BaseEntity
{
    [SerializeField] private Transform shootPos;
    bool isShooting;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float fireRate;
    [SerializeField] private NavMeshAgent agent;
    private bool inRange;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        GameManager.instance.updateGameGoal(1);

    }
    public override void Update()
    {
        agent.SetDestination(GameManager.instance.playerRef.transform.position);
        base.Update();
        Attack();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == GameManager.instance.playerRef)
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
    public override void Death()
    {
        GameManager.instance.updateGameGoal(-1);

        base.Death(); //base death contains destroy(gameObject);
    }
    public void Attack()
    {
        if (!isShooting && inRange)
        {
            StartCoroutine(AttackDelay());
        }
    }
    private IEnumerator AttackDelay()
    {
        isShooting = true;
        Instantiate(bullet,shootPos.position, transform.rotation);

        yield return new WaitForSeconds(fireRate);
        isShooting = false;
    }
}
