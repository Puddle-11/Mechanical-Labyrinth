using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class BaseEnemy : BaseEntity
{
    //NEW VARIABLES
    //===========================
    [Space]
    [Header("BASE ENEMY GENERAL")]
    [Header("_______________________________")]
    [Space]
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Weapon weaponScr;
    [SerializeField] protected Transform headPos;
    [SerializeField] protected Vector3[] patrolPoints;
    [SerializeField] protected int contactDamage = 15;
    [SerializeField] protected float contactDamageFrequency = 0.5f;

    [Space]
    [SerializeField] protected float stoppingDistance = 7;
    [SerializeField] protected float rotationSpeed = 180;
    [SerializeField] protected float roamTimer = 2;
    [SerializeField] protected float roamingDistance = 15;
    [SerializeField] protected int sneakDamageMultiplyer = 2;
    [Header("DETECTION")]
    [Space]
    [SerializeField] protected float attackRange = 7;
    [SerializeField] protected float sightRange = 13.5f;
    [SerializeField] protected float hearingRange = 20;
    [Space]
    [SerializeField] protected float attackAngle = 50;
    [SerializeField] protected float sightAngle = 90;
    [Space]
    [SerializeField] protected GameObject target;
    [SerializeField] protected DetectionType senseType;
    [SerializeField] protected LayerMask sightMask;

    [Space]
    [Header("PLAYER FEEDBACK")]
    [Space]

    [SerializeField] protected Image alertMarker;
    [SerializeField] protected Animator anim;
    [SerializeField] protected float transitionSpeed = 0.5f;
    [SerializeField] public AudioClip[] Deathsounds;
    protected EnemyState currState;
    protected bool isRoaming;
    protected Vector3 startingPos;
    protected bool runningContactDamage;
    #region Custom Structs and Enums
    //=======================================
    //CUSTOM STRUCTS AND ENUMS
    public enum DetectionType
    {
        InRange,
        Vision,
        Sound,
        Vision_Sound,
        Continuous,
    }
    //-------------
    public enum EnemyState
    {
        Patrol,
        Investigate,
        Persue,
        Attack
    }
    //=======================================
    #endregion

    #region Default MonoBehavior Methods
    //=======================================
    //DEFAULT MONOBEHAVIOR METHODS


    //OnEnable is called before the start function
    public void OnEnable()
    {
        if (target != null) SetUpEvents();
    }
    //-------------
    public void OnDisable()
    {
        RemoveEvents();
    }
    protected void RemoveEvents()
    {
        if (senseType == DetectionType.Sound || senseType == DetectionType.Vision_Sound)
        {
            if (target != null)
            {
                SoundSenseSource sssRef = target.GetComponentInChildren<SoundSenseSource>();
                if (sssRef != null)
                {
                    sssRef.makeSound -= EnterInvestigate;
                }
            }
        }
    }
    //-------------
    public void SetUpEvents()
    {
        if (senseType == DetectionType.Sound || senseType == DetectionType.Vision_Sound)
        {

            SoundSenseSource sssRef = target.GetComponentInChildren<SoundSenseSource>();
            if (sssRef != null)
            {

                sssRef.makeSound += EnterInvestigate;
            }
        }
    }

    public override void Start()
    {
        GetTarget();
        SetUpEvents();
        startingPos = transform.position;
        base.Start();
        GameManager.instance.updateGameGoal(1);
    }
    //-------------
    public override void Update()
    {
        StateHandler();
        if (anim != null)
        {
            float agentSpeed = agent.velocity.normalized.magnitude;
            float lerpedSpeed = Mathf.Lerp(anim.GetFloat("Speed"), agentSpeed, Time.deltaTime * transitionSpeed);
            anim.SetFloat("Speed", lerpedSpeed);
        }
     
        base.Update();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == GameManager.instance.playerRef)
        {
            StartCoroutine(contactDamageDelay(other.GetComponent<IHealth>()));
        }
    }
    //=======================================
    #endregion

    #region Getters and Setters
    //=======================================
    //GETTERS AND SETTERS
    public void SetPatrolPoints(Vector3[] _val)
    {
        patrolPoints = _val;
    }
    //-------------
    protected void SetNavmeshTarget(Vector3 _pos)
    {
       
       agent.SetDestination(_pos);
    }
    private void SetNavmeshTarget()
    {
        SetNavmeshTarget(target.transform.position);
    }
    //-------------
    public void GetTarget()
    {
        
        if (target == null) target = GameManager.instance.playerRef;
    }
    //=======================================
    #endregion

    #region Helper Functions
    //=======================================
    //HELPER FUNCTIONS

    protected float DistanceToTarget()
    {
        return Vector3.Distance(target.transform.position, transform.position);
    }
    //-------------
    public Vector3 DirectionToTarget()
    {
        Vector3 targetDir = (target.transform.position - transform.position).normalized;
        return targetDir;
    }
    //-------------
    protected bool IsInRange(float _dist)
    {
        Vector3 angle;
        return IsInRange(out angle, _dist);
    }
    protected bool IsInRange()
    {
        return IsInRange(hearingRange);
    }
    protected bool IsInRange(out Vector3 _dirToTarget)
    {

       return IsInRange(out _dirToTarget, hearingRange);
    }
    protected bool IsInRange(out Vector3 _dirToTarget, float _dist)
    {
        _dirToTarget = DirectionToTarget();

        if (DistanceToTarget() < hearingRange)
        {
            RaycastHit hit;
            Vector3 pos = transform.position;
            if (headPos != null) pos = headPos.transform.position;
            if (Physics.Raycast(pos, _dirToTarget, out hit, _dist, ~sightMask))
            {
                if (hit.collider.gameObject == target)
                {
                    return true;
                }
            }
        }
        return false;

    }
    //-------------
    public bool InAngleRange(float _range)
    {
        Vector3 dPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        Vector3 targetDir = (dPos - transform.position).normalized;

        //Checks if target is in range, Gets the direction to the target and checks the angle from transform.forward
        //if less than sight radius, target is in sight
        if (IsInRange(sightRange) && Vector3.Angle(targetDir, transform.forward) < _range / 2)
        {
            return true;
        }
        return false;
    }
    public bool InAngleRange()
    {
        return InAngleRange(sightAngle);
    }
    //-------------
    private float DistanceToDestination()
    {
        if (agent != null)
        {
            return agent.remainingDistance;
        }
        else return 0;
    }
    //=======================================
    #endregion

    #region State Machine
    protected virtual void StateHandler()
    {
        if (agent == null) return;
        EnemyStatus(ref currState);
        switch (currState)
        {
            case EnemyState.Patrol:
                 agent.stoppingDistance = 0;
                if (agent.remainingDistance < 0.1f)
                {
                    StartCoroutine(Roam());
                }
                break;
            case EnemyState.Investigate:
                agent.stoppingDistance = 0;
                break;
            case EnemyState.Persue:
            case EnemyState.Attack:
                SetNavmeshTarget(target.transform.position);
                agent.stoppingDistance = stoppingDistance;
                if (agent.remainingDistance <= stoppingDistance) FacePlayer();
                break;
        }
        if (currState == EnemyState.Attack)
        {
            weaponScr?.Attack();
        }
    }
    //-------------
    protected void EnemyStatus(ref EnemyState _enemyStateRef)
    {
        if (senseType == DetectionType.InRange)
        {
            if (IsInRange(attackRange))
            {
                _enemyStateRef = EnemyState.Attack;
            }
            else if(IsInRange())
            {
                _enemyStateRef = EnemyState.Persue;

            }
            else
            {
                _enemyStateRef = EnemyState.Patrol;

            }
        }
        else if (senseType == DetectionType.Continuous)
        {
            if (IsInRange(attackRange)) _enemyStateRef = EnemyState.Attack;
            else _enemyStateRef = EnemyState.Persue;
        }
        if (senseType == DetectionType.Vision || senseType == DetectionType.Vision_Sound)
        {
            if (InAngleRange())
            {
                if (IsInRange(attackRange) && InAngleRange(attackAngle))
                {
                    _enemyStateRef = EnemyState.Attack;
                }
                else
                {

                    _enemyStateRef = EnemyState.Persue;
                }
            }
            else if (!InAngleRange() && currState != EnemyState.Investigate) {
                _enemyStateRef = EnemyState.Patrol;
            }
            else if (currState == EnemyState.Investigate && DistanceToDestination() < 0.01f) currState = EnemyState.Patrol;
            

        }
    }
    //-------------
    protected void EnterInvestigate(Vector3 _pos)
    {
        if ((currState == EnemyState.Patrol || currState == EnemyState.Investigate) && IsInRange())
        {
            currState = EnemyState.Investigate;
            SetNavmeshTarget(_pos);
        }
    }
    //-------------
    protected IEnumerator Roam()
    {
        if (isRoaming) yield break;
        isRoaming = true;
        yield return new WaitForSeconds(roamTimer);
        Vector3 _nextPos = transform.position;
        if (patrolPoints.Length <= 0)
        {
            Vector3 randDist = UnityEngine.Random.insideUnitSphere * roamingDistance;
            randDist += startingPos;
            NavMeshHit hit;
            NavMesh.SamplePosition(randDist, out hit, roamingDistance, 1);
            _nextPos = hit.position;
        }
        else
        {
            _nextPos = patrolPoints[UnityEngine.Random.Range(0, patrolPoints.Length)];

        }
        agent.SetDestination(_nextPos);
        isRoaming = false;
    }
    //-------------
    public void FacePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z) - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * rotationSpeed);

    }
    #endregion

    #region IHealth Methods
    public override void UpdateHealth(int _amount)
    {
        if (currState == EnemyState.Patrol || currState == EnemyState.Investigate)
        {
            _amount = _amount * sneakDamageMultiplyer;
            EnterInvestigate(target.transform.position);
        }
        base.UpdateHealth(_amount);

    }
    //-------------
    public override void Death()
    {
       if(Deathsounds.Length > 0) AudioManager.instance.PlaySound(Deathsounds[UnityEngine.Random.Range(0, Deathsounds.Length)], AudioManager.soundType.enemy);
        GameManager.instance?.updateGameGoal(-1);
        GameManager.instance.UpdateKillCounter(1);
        if (weaponScr != null && weaponScr.GetPickup() != null) DropItem(weaponScr.GetPickup());
        RemoveEvents();
        base.Death();
    }
    #endregion

    
    protected IEnumerator contactDamageDelay(IHealth _target)
    {
        if (runningContactDamage) yield break;
        runningContactDamage = true;
        _target.UpdateHealth(-contactDamage);
        yield return new WaitForSeconds(contactDamageFrequency);
        runningContactDamage = false;

    }
    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hearingRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
