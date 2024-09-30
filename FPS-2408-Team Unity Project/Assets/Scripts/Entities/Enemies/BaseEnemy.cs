using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


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
    [SerializeField] protected GameObject shield;
    [SerializeField] protected float attackdelay;
    private float attackDelayTimer;

    [Space]
    [Header("PLAYER FEEDBACK")]
    [Space]
    [SerializeField] private FaceTarget headScript;
    [SerializeField] protected Animator anim;
    [SerializeField] protected float transitionSpeed = 0.5f;
    [SerializeField] public AudioClip[] Deathsounds;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private ParticleSystem damageParticles;
    protected EnemyState currState;
    protected bool isRoaming;
    protected Vector3 startingPos;
    protected bool runningContactDamage;
    protected bool damagedThisFrame;
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
        if(currState == EnemyState.Attack)
        {
            attackDelayTimer += Time.deltaTime;
        }
        else
        {
            attackDelayTimer = 0;

        }
        if (currState == EnemyState.Investigate|| currState == EnemyState.Persue || currState == EnemyState.Attack)
        {
            if(headScript != null) headScript.enabled = true;

        }
        else
        {
            if (headScript != null) headScript.enabled = false;
            if(headScript != null) headScript.gameObject.transform.localRotation = Quaternion.identity;
        }
        StateHandler();
        if (anim != null)
        {
            float agentSpeed = agent.velocity.normalized.magnitude;
            float lerpedSpeed = Mathf.Lerp(anim.GetFloat("Speed"), agentSpeed, Time.deltaTime * transitionSpeed);
            anim.SetFloat("Speed", lerpedSpeed);
        }

        if (currentShield <= 0)
        {
            if(shield != null) shield.SetActive(false);
        }
        base.Update();
        damagedThisFrame = false;
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
                if ( agent.remainingDistance <= stoppingDistance) FaceTarget();
                break;
        }
        if (currState == EnemyState.Attack)
        {
            if (weaponScr != null && attackDelayTimer >= attackdelay)
            {
                weaponScr.Attack();
            }
        }
    }
    //-------------
    protected virtual void EnemyStatus(ref EnemyState _enemyStateRef)
    {
        bool inAttackRange = IsInRange(attackRange);
        bool inRange = IsInRange();

        /* if (timer >= GameManager.instance.Getmaxtime()) {
             float angle = GetAngle();

             if (inAttackRange && angle < attackAngle / 2) { _enemyStateRef = EnemyState.Attack; }
             else { _enemyStateRef = EnemyState.Persue; }
         }
         else*/
        if(GameManager.instance.GetPerLevelTime() <= GameManager.instance.startingDelay)
        {
            _enemyStateRef = EnemyState.Patrol;
        }
        else if (senseType == DetectionType.InRange)
        {
            
            float angle = GetAngle();
            if (inAttackRange && angle < attackAngle / 2) {
                _enemyStateRef = EnemyState.Attack;
            
            }
            else if(IsInRange()) 
            {

                _enemyStateRef = EnemyState.Persue;
            
            }
            else if(currState == EnemyState.Investigate)
            {

                if (DistanceToDestination() < 0.01f) {
                    _enemyStateRef = EnemyState.Patrol;
                }
            
            }
            else
            {

                _enemyStateRef = EnemyState.Patrol;

            }
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
            else
            {
                _enemyStateRef = EnemyState.Patrol;
            }
        }
    }
    //-------------
    protected void EnterInvestigate(Vector3 _pos)
    {
        if ((currState == EnemyState.Patrol || currState == EnemyState.Investigate))
        {
            currState = EnemyState.Investigate;
            StopCoroutine("Roam");
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
            Vector3 randDist = Random.insideUnitSphere * roamingDistance;
            randDist += startingPos;
            NavMeshHit hit;
            NavMesh.SamplePosition(randDist, out hit, roamingDistance, 1);
            _nextPos = hit.position;
        }
        else
        {
            _nextPos = patrolPoints[Random.Range(0, patrolPoints.Length)];

        }
        if(agent != null) agent.SetDestination(_nextPos);
        isRoaming = false;
    }
    //-------------
  
    #endregion

    #region IHealth Methods
    public override void UpdateHealth(int _amount, float _shieldPen = 1)
    {
        
        if (damagedThisFrame == false)
        {
            if (hitSound != null && AudioManager.instance != null)
            {
                AudioManager.instance.PlaySound(hitSound, SettingsController.soundType.enemy);
            }
        }
        base.UpdateHealth(_amount);
        if (_amount < currentHealth) damagedThisFrame = true;
        if (damageParticles != null) damageParticles.Play();
        if (currState == EnemyState.Patrol || currState == EnemyState.Investigate)
        {
            _amount = _amount * sneakDamageMultiplyer;
           if(target != null) EnterInvestigate(target.transform.position);
        }

    }
    //-------------
    public override void Death()
    {
        if (dead) return;

        base.Death();
        if (Deathsounds.Length > 0 && AudioManager.instance != null) AudioManager.instance.PlaySound(Deathsounds[Random.Range(0, Deathsounds.Length)], SettingsController.soundType.enemy);
        GameManager.instance?.updateGameGoal(-1);
        GameManager.instance.UpdateKillCounter(1);
        if (weaponScr != null && weaponScr.GetPickup() != null) DropItem(weaponScr.GetPickup());
        DropInventory();
        RemoveEvents();
    }
    #endregion

  
}
