using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Editor;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class BaseEnemy : SharedEnemyBehavior
{
    
    [Space]
    [Header("BASE ENEMY GENERAL")]
    [Header("_______________________________")]
    [Space]
    [SerializeField] protected Weapon weaponScr;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Vector3[] patrolPoints;
    [SerializeField] protected DetectionType senseType;
    [SerializeField] private Vector2Int minmaxScrap;


    [Space]
    [Header("PLAYER FEEDBACK")]
    [Space]

    [SerializeField] protected Animator anim;
    [SerializeField] protected float transitionSpeed = 0.5f;
    [SerializeField] public AudioClip[] Deathsounds;

    protected EnemyState currState;
    protected float timer;
    protected bool isRoaming;
    protected Vector3 startingPos;
    protected bool runningContactDamage;

    //public object UnityEngine { get; private set; }


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
    public void OnEnable(){if (target != null) SetUpEvents();}
    public void OnDisable() { RemoveEvents();}


    public override void Start()
    {
        GetTarget();
        SetUpEvents();
        startingPos = transform.position;
       if(GameManager.instance != null) GameManager.instance.updateGameGoal(1);
        base.Start();
    }
    //-------------
    public override void Update()
    {
        timer += Time.deltaTime;
        StateHandler();
        if (anim != null)
        {
            float agentSpeed = agent.velocity.normalized.magnitude;
            float lerpedSpeed = Mathf.Lerp(anim.GetFloat("Speed"), agentSpeed, Time.deltaTime * transitionSpeed);
            anim.SetFloat("Speed", lerpedSpeed);
        }
        base.Update();
    }

    //=======================================
    #endregion

    #region Getters and Setters
    //=======================================
    //GETTERS AND SETTERS
    public void SetPatrolPoints(Vector3[] _val) { patrolPoints = _val; }
    protected void SetNavmeshTarget(Vector3 _pos)
    {

        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(_pos);
        }
    }





    private void SetNavmeshTarget() { if (target != null) SetNavmeshTarget(target.transform.position); }
        public void GetTarget(){if (target == null && GameManager.instance != null) target = GameManager.instance.playerRef;}
    private float DistanceToDestination() { return agent != null ? agent.remainingDistance : 0; }
    //=======================================
    #endregion

    #region Helper Functions
    //=======================================
    //HELPER FUNCTIONS

    protected void RemoveEvents()
    {
        if ((senseType == DetectionType.Sound || senseType == DetectionType.Vision_Sound) && target != null)
        {
            SoundSenseSource sssRef = target.GetComponentInChildren<SoundSenseSource>();
            if (sssRef != null) sssRef.makeSound -= EnterInvestigate;
        }
    }
    //-------------
    public void SetUpEvents()
    {
        if ((senseType == DetectionType.Sound || senseType == DetectionType.Vision_Sound) && target != null)
        {
            SoundSenseSource sssRef = target.GetComponentInChildren<SoundSenseSource>();
            if (sssRef != null) sssRef.makeSound += EnterInvestigate;
        }
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
                if ((agent == null || !agent.isActiveAndEnabled) || agent.remainingDistance < 0.1f)
                {
                    StartCoroutine(Roam());
                }
                break;
            case EnemyState.Investigate:
                agent.stoppingDistance = 0;
                break;
            case EnemyState.Persue:
            case EnemyState.Attack:
              if(target != null)  SetNavmeshTarget(target.transform.position);
                agent.stoppingDistance = stoppingDistance;
                if (agent.remainingDistance <= stoppingDistance) FaceTarget();
                break;
        }
        if (currState == EnemyState.Attack)
        {
            if(weaponScr != null) weaponScr.Attack();
        }
    }
    //-------------
    protected void EnemyStatus(ref EnemyState _enemyStateRef)
    {
       
        bool inAttackRange = IsInRange(attackRange);
        bool inRange = IsInRange();
        if (timer >= GameManager.instance.Getmaxtime()) {
            if (inAttackRange) { _enemyStateRef = EnemyState.Attack; }
            else { _enemyStateRef = EnemyState.Persue; }
        }
        else if (senseType == DetectionType.InRange)
        {
            if (inAttackRange) {_enemyStateRef = EnemyState.Attack;}
            else if(IsInRange()) {_enemyStateRef = EnemyState.Persue;}
            else { _enemyStateRef = EnemyState.Patrol;}
        }
        else if (senseType == DetectionType.Continuous)
        {
            if (inAttackRange) _enemyStateRef = EnemyState.Attack;
            else _enemyStateRef = EnemyState.Persue;
        }
        if (senseType == DetectionType.Vision || senseType == DetectionType.Vision_Sound)
        {
            bool inAngleRange = InAngleRange();
            float angle = GetAngle();
            if (inRange)
            {
                if (angle < sightAngle / 2)
                {
                    if (inAttackRange && angle < attackAngle / 2) { _enemyStateRef = EnemyState.Attack; }
                    else { _enemyStateRef = EnemyState.Persue; }
                }
                else if (!inAngleRange && currState != EnemyState.Investigate) { _enemyStateRef = EnemyState.Patrol; }
                else if (currState == EnemyState.Investigate && DistanceToDestination() < 0.01f) currState = EnemyState.Patrol;
            }
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
            Vector3 randDist = global::UnityEngine.Random.insideUnitSphere * roamingDistance;
            randDist += startingPos;
            NavMeshHit hit;
            NavMesh.SamplePosition(randDist, out hit, roamingDistance, 1);
            _nextPos = hit.position;
        }
        else
        {
            _nextPos = patrolPoints[global::UnityEngine.Random.Range(0, patrolPoints.Length)];

        }
        agent.SetDestination(_nextPos);
        isRoaming = false;
    }
    //-------------
  
    #endregion

    #region IHealth Methods
    public override void UpdateHealth(int _amount, float _shieldPen = 1)
    {
        Debug.Log("Hit base enemy");
        base.UpdateHealth(_amount);
        if (currState == EnemyState.Patrol || currState == EnemyState.Investigate)
        {
            _amount = _amount * sneakDamageMultiplyer;
           if(target != null) EnterInvestigate(target.transform.position);
        }

    }
    //-------------
    public override void Death()
    {
        int scrapDrop = global::UnityEngine.Random.Range(minmaxScrap.x, minmaxScrap.y);
        if (ScrapInventory.instance != null) ScrapInventory.instance.AddScrap(scrapDrop);
        if (Deathsounds.Length > 0 && AudioManager.instance != null) AudioManager.instance.PlaySound(Deathsounds[global::UnityEngine.Random.Range(0, Deathsounds.Length)], SettingsController.soundType.enemy);
        GameManager.instance?.updateGameGoal(-1);
        GameManager.instance.UpdateKillCounter(1);
        if (weaponScr != null && weaponScr.GetPickup() != null) DropItem(weaponScr.GetPickup());
        RemoveEvents();
        base.Death();
    }
    #endregion

  
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
