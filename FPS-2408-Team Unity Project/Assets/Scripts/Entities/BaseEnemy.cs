using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : BaseEntity
{

    [SerializeField] private DetectionType DetectPlayerType;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] protected Weapon weaponScr;
    private bool inRange;
    [SerializeField] private GameObject target;
    [SerializeField] private float sightRadiusRad;
    public float angleRad;
    public enum DetectionType
    {
        Continuous,
        InRange,
        Vision,
        Sound,
        Vision_Sound,
    }
    // Start is called before the first frame update
    
    public override void Start()
    {
        base.Start();
   
        GameManager.instance.updateGameGoal(1);

    }
    public override void Update()
    {
        GetLineOfSight();
        switch (DetectPlayerType)
        {
            case DetectionType.Continuous:
                SetTarget();
                break;
            case DetectionType.InRange:
                if (inRange)
                {
                    SetTarget();
                }
                break;
            case DetectionType.Vision:
                if(inRange && GetLineOfSight())
                {
                    SetTarget();
                    weaponScr.Attack();
                }
                break;
            case DetectionType.Sound:
                break;
            case DetectionType.Vision_Sound:
                break;
            default:
                break;
        }


        base.Update();
        if (weaponScr != null && inRange) //automatically fires gun when enemy is in range 
        {
            //weaponScr.Attack();
        }
    }
    private bool GetLineOfSight()
    {
        if (target != null)
        {
            Vector3 targetDir = (target.transform.position - transform.position).normalized;
            Vector3 selfDir = transform.forward;

            float dotProduct = targetDir.x * selfDir.x + targetDir.z * selfDir.z;   
            float mag = targetDir.magnitude * selfDir.magnitude;

           
            angleRad = Mathf.Acos(dotProduct / mag);
            if(angleRad < sightRadiusRad)
            {
                return true;
            }
        }
        return false;
    }
    private void SetTarget()
    {
        if (agent != null)
        {

            //agent.SetDestination(target.transform.position);
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
