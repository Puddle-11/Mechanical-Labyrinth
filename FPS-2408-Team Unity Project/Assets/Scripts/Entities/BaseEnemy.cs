using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class BaseEnemy : BaseEntity
{

    [SerializeField] private DetectionType DetectPlayerType;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator anim;
    [SerializeField] private float transitionSpeed;
    [SerializeField] protected Weapon weaponScr;
    private bool inRange;
    [SerializeField] private GameObject target;
    [SerializeField] private float sightRadius;
    [SerializeField] private float detectionRange;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform headPos;
    [SerializeField] private int sneakDamageMultiplyer = 2;
    [SerializeField] private float roamTimer;
    [SerializeField] private float roamingDistance;
    private float stoppingDistOriginal;
    private bool isRoaming;
    private Vector3 startingPos;
    [SerializeField] private float shootAngle;
    [SerializeField] private Vector3[] patrolPoints;
    public int patrolPointCount;
    public enum DetectionType
    {
        InRange,
        Vision,
        Sound,
        Vision_Sound,
        Continuous,
    }
    public void SetPatrolPoints(Vector3[] _val)
    {
        patrolPoints = _val;
    }
    private IEnumerator Roam()
    {
        isRoaming = true;
        yield return new WaitForSeconds(roamTimer);
            agent.stoppingDistance = 0;
        Vector3 _nextPos = transform.position;
        if (ChunkGrid.instance == null)
        {
            Vector3 randDist = Random.insideUnitSphere * roamingDistance;
            randDist += startingPos;
            NavMeshHit hit;
            NavMesh.SamplePosition(randDist, out hit, roamingDistance, 1);
            _nextPos = hit.position;
        }
        else
        {
            if (patrolPoints.Length != 0)
            {
                _nextPos = patrolPoints[Random.Range(0, patrolPoints.Length)];
            }
        }
            agent.SetDestination(_nextPos);
        isRoaming = false;
        
    }
    // Start is called before the first frame update
    
    public override void Start()
    {
        startingPos = transform.position;
        base.Start();
        GameManager.instance.updateGameGoal(1);
        stoppingDistOriginal = agent.stoppingDistance;
    }
    public override void Update()
    {
        if (anim != null)
        {
            float agentSpeed = agent.velocity.normalized.magnitude;
            float lerpedSpeed = Mathf.Lerp(anim.GetFloat("Speed"), agentSpeed, Time.deltaTime * transitionSpeed);
        anim.SetFloat("Speed", lerpedSpeed);
        }
     
            if (inRange && GetEnemyAlertStatus())
            {
                SetNavmeshTarget();
                if (weaponScr != null)
                {
                    if (weaponScr.CanAttack())
                    {

                    if (anim != null) anim.SetTrigger("Attack");
                        weaponScr.Attack();
                    }
                }
                if (agent.remainingDistance <= agent.stoppingDistance) FacePlayer();
                agent.stoppingDistance = stoppingDistOriginal;
            }
            else
            {
                agent.stoppingDistance = 0;

            }

        
        if (!GetEnemyAlertStatus())
        {
            if (!isRoaming && agent.remainingDistance < 0.1f)
            {
                StartCoroutine(Roam());
            }
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
                if (GetInrange())
                {
                    return true;
                }
                break;
            case DetectionType.Vision:
                if ((inRange && GetLineOfSight()))
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
    public override void UpdateHealth(int _amount)
    {
        if (!GetEnemyAlertStatus())
        {
            _amount = _amount * sneakDamageMultiplyer;
            SetNavmeshTarget();
        }
        base.UpdateHealth(_amount);
      
    }
    private bool GetLineOfSight()
    {
        Vector3 targetDir = (GetTarget().transform.position - transform.position).normalized;
        targetDir.y = -targetDir.y;
        float angle = Vector3.Angle(targetDir, transform.forward);
        

        if (angle < sightRadius / 2)
        {
            return GetInrange();
        }
        return false;
    }
    private bool GetInrange()
    {
        Vector3 targetDir = (GetTarget().transform.position - transform.position).normalized;
        targetDir.y = -targetDir.y;
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, targetDir, out hit, detectionRange, ~weaponScr.ignoreMask))
        {
            if (hit.collider.gameObject == GetTarget() && inRange)
            {

                return true;

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
        Quaternion rot = Quaternion.LookRotation(GetTarget().transform.position - transform.position);
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
        DropItem(weaponScr.GetPickup());
        base.Death();
    }
    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward * 10, Color.red);
    }

}
