using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : BaseEntity
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] protected Weapon weaponScr;
    private bool inRange;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        GameManager.instance.updateGameGoal(1);

    }
    public override void Update()
    {
        if (agent != null)
        {
            agent.SetDestination(GameManager.instance.playerRef.transform.position);
        }
        base.Update();
        if(weaponScr != null && inRange) //automatically fires gun when enemy is in range 
        {
            weaponScr.Attack();
        }
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
  
}
