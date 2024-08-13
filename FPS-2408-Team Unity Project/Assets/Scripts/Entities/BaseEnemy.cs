using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class BaseEnemy : BaseEntity
{

    [SerializeField] private DetectionType DetectPlayerType;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] protected Weapon weaponScr;
    private bool inRange;
    [SerializeField] private GameObject target;
    [SerializeField] private float sightRadiusRad;
    [SerializeField] private float detectionRange;
    [SerializeField] private float rotationSpeed;
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
            SetNavmeshTarget();
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
        float angle = Vector3.Angle(targetDir, transform.forward);
        

        if (angle < sightRadiusRad)
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, targetDir, out hit, detectionRange, ~weaponScr.ignoreMask))
            {
                if (hit.collider.gameObject == GetTarget())
                {
                    return true;
                }
            }
        }

        return false;
    }
    private void SetNavmeshTarget()
    {
        if (agent != null)
        {
            agent.SetDestination(GetTarget().transform.position);
        }
    }
    public void FacePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(GetTarget().transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * rotationSpeed);


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
