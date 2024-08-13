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
        InRange,
        Vision,
        Sound,
        Vision_Sound,
        Continuous,
    }
    // Start is called before the first frame update
    
    public override void Start()
    {
        base.Start();
   
        GameManager.instance.updateGameGoal(1);

    }
    public override void Update()
    {
        if (GetEnemyAlertStatus())
        {
            weaponScr.Attack();
        }
        base.Update();
    }
    public bool GetEnemyAlertStatus()
    {
        switch (DetectPlayerType)
        {
            case DetectionType.Continuous:
                return true;
            case DetectionType.InRange:
                if (inRange)
                {
                    return true;
                }
                break;
            case DetectionType.Vision:
                if (inRange && GetLineOfSight())
                {
                    return true;
                }
                break;
            case DetectionType.Sound:
                break;
            case DetectionType.Vision_Sound:
                break;

        }
        return false;
    }
    private bool GetLineOfSight()
    {
        Vector3 targetDir = (GetTarget().transform.position - transform.position).normalized;
        Vector3 selfDir = transform.forward;

        float dotProduct = targetDir.x * selfDir.x + targetDir.z * selfDir.z;
        float mag = targetDir.magnitude * selfDir.magnitude;


        angleRad = Mathf.Acos(dotProduct / mag);
        if (angleRad < sightRadiusRad)
        {
            return true;
        }

        return false;
    }
    private void SetNavmeshTarget()
    {
        if (agent != null)
        {
            //agent.SetDestination(target.transform.position);
        }
    }
    public GameObject GetTarget()
    {
        if(target == null) return GameManager.instance.playerRef;
        
        return target;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GetTarget())
        {
            inRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == GetTarget())
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
