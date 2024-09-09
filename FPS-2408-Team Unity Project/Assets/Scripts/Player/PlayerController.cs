using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Transactions;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SocialPlatforms;
//====================================
//REWORKED
//====================================
public class PlayerController : BaseEntity
{


    private CharacterController controllerRef;
    [Header("Walk Variables")]
    [Space]
    [SerializeField] private SoundSenseSource footstepSoundRef;
    [SerializeField] private float baseSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float sprintMod;
    [SerializeField] private float externalForceDampening;
    [SerializeField] private float speedBonus;
    [SerializeField] private float maxSpeedBonus;
    private bool isSprinting;


    [Header("Jump Variables")]
    [Space]
    [SerializeField] private LayerMask jumplayer;
    [SerializeField] private float jumpHeight;
    [SerializeField] private int jumpMax;


    [Header("Physics Variables")]
    [Space]
    [SerializeField] private float gravityStrength;
    [Header("Sounds")]
    [Space]
    [SerializeField] private float footstepDelay;
    public AudioClip[] footstepSounds;
    public AudioClip[] damageSounds;
    public delegate void PlayerUsed();
    public PlayerUsed playerUseEvent;
    [Range(0,1)][SerializeField] float footstepvol;



    private int jumpCurr;
    private bool onWall;

    public Vector3 externalForce;
    private Vector3 filteredMove;
    private Vector3 finalVel;
    private float originalgravity;
    private PlayerHand playerHandRef;
    private GameObject playerSpawnPos;
    private bool playingFootstepSound;
    private bool isDead;
    private Vector3 frameNormals;
    private float moveTimer;




    #region MonoBehavior Methods
    public override void Awake()
    {
        FetchComponents();
    }

    // Start is called before the first frame update
    public override void Start()
    {
        SetHealth(GameManager.instance.GetCurrentHealth());
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
        originalgravity = gravityStrength;
        UIManager.instance.UpdateHealthBar((float)currentHealth / maxHealth);
        GameObject currHandObj = playerHandRef.GetCurrentHand();
        if(currHandObj != null)
        {
            BaseGun bref;
            if(currHandObj.TryGetComponent(out bref))
            {
                UIManager.instance.UpdateExternalAmmoInv(true, (int)bref.GetAmmoType());
            }
        }
    }



    public override void Update()
    {
        frameNormals = Vector3.zero;
        #region Button Handling
        //-----------------------------------------
        //Button Handling
        if (playerHandRef != null)
        {
            if (GameManager.instance.GetStatePaused() || (BootLoadManager.instance != null && BootLoadManager.instance.IsLoading()))
            {
                playerHandRef.SetUseItem(false);
            }
            else
            {
                if (Input.GetButtonDown("Pick Up"))
                {
                    playerHandRef.ClickPickUp();
                }
                if (Input.GetButtonDown("Shoot"))
                {
                    playerHandRef.SetUseItem(true);
                }
                if (Input.GetButtonUp("Shoot"))
                {
                    playerHandRef.SetUseItem(false);
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    if (playerHandRef.GetCurrentHand().TryGetComponent(out BaseGun tempOut))
                    {
                        StartCoroutine(tempOut.Reload());
                    }
                }
                if (Input.GetMouseButtonDown(1))
                {
                    playerHandRef.toggleADS(!playerHandRef.GetIsAiming());
                }
            }
        }
        //-----------------------------------------
        #endregion


        base.Update();
    }
    public void LateUpdate()
    {
        Movement();
    }
    #endregion

    #region Getters and Setters
    public ItemType GetCurrentItemType() { return playerHandRef.GetCurrentItemType(); }
    public void UpdatePlayerSpeed(float _mod) { acceleration *= _mod; }
    public void SetForce(Vector3 _newForce) { externalForce = _newForce; }

    public Vector3 GetForce() { return externalForce; }
    public PlayerHand GetPlayerHand() { return playerHandRef; }
    public void SetJumpAmount(int _val) { jumpCurr = Mathf.Clamp(_val, 0, jumpMax + 1); }
    public void UpdateJumpAmount(int _val) { SetJumpAmount(jumpCurr + _val); }
    #endregion

    #region Helper Methods
    //Get Components is called in awake
    public void FetchComponents()
    {
        if (!TryGetComponent(out playerHandRef))
        {
            Debug.LogWarning("No player hand component found on " + gameObject.name);
        }

        if (!TryGetComponent(out controllerRef))
        {
            Debug.LogWarning("No character controller found on " + gameObject.name);
        }

        Renderer[] tempArr = GetComponentsInChildren<Renderer>();
        rendRef = new RenderContainer[tempArr.Length];
        for (int i = 0; i < rendRef.Length; i++)
        {
            rendRef[i].currRenderer = tempArr[i];
        }
    }


    private bool TryFindPlayerSpawnPos(out GameObject _ref)
    {
        GameObject temp = GameObject.FindGameObjectWithTag("Player Spawn Pos");
        if (temp != null)
        {

            _ref = temp;

            return true;
        }

        _ref = null;

        return false;
    }

    #endregion

    #region IHealth Methods



    public override void ResetHealth()
    {
        isDead = false;
        base.ResetHealth();
    }
    public override void SetHealth(int _amount)
    {
        if (_amount < currentHealth)
        {
            if (CameraController.instance != null) CameraController.instance.StartCamShake();

            if (UIManager.instance != null)
            {
                StartCoroutine(UIManager.instance.flashDamage());
            }
        }

        GameManager.instance.SetCurrentHealth(_amount);
        if(UIManager.instance != null) UIManager.instance.UpdateHealthBar((float)_amount / maxHealth);
        base.SetHealth(_amount);
    }
    #endregion

    #region Override Methods
    public override void Death()
    {
        if (isDead) return;
        isDead = true;

        GameManager.instance.ResetAllStats();
        GameManager.instance.UpdateDeathCounter(1);
        UIManager.instance.OpenLoseMenu();
    }
    #endregion

    public bool SpawnPlayer()
    {
        if (TryFindPlayerSpawnPos(out playerSpawnPos))
        {
            controllerRef.enabled = false;

            transform.position = playerSpawnPos.transform.position;

            controllerRef.enabled = true;

            return true;
        }
        return false;
    }

    public void SetPlayerSpawnPos(Vector3 _pos)
    {
        if (TryFindPlayerSpawnPos(out playerSpawnPos))
        {
            playerSpawnPos.transform.position = _pos;
        }
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {

        frameNormals += hit.normal;
    }
    private void Movement()
    {
        Vector3 move = Input.GetAxis("Vertical") * transform.forward + Input.GetAxis("Horizontal") * transform.right;
        //Apply gravity
        filteredMove = move * baseSpeed + move * moveTimer;
        externalForce = Vector3.MoveTowards(externalForce, new Vector3(0,externalForce.y, 0), externalForceDampening * Time.deltaTime);
        finalVel =  filteredMove + externalForce;

        if (controllerRef.isGrounded)
        {

            jumpCurr = jumpMax;
            externalForce.y = Mathf.Clamp(externalForce.y, 0, Mathf.Infinity);
        }
        else
        {
            externalForce.y -= gravityStrength * Time.deltaTime;
        }
        if (move.magnitude > 0.3)
        {

            moveTimer = Mathf.Clamp(moveTimer + Time.deltaTime * speedBonus, 0, maxSpeedBonus);

            if (controllerRef.isGrounded)
            {
                StartCoroutine(PlayStepSound());
            }
        }
        else
        {
            moveTimer = 0;
        }
        

        controllerRef.Move(finalVel * Time.deltaTime);
        if (Input.GetButtonDown("Jump"))
        {
            Jump(new Vector3(0, jumpHeight, 0));
        }
        controllerRef.Move(externalForce * Time.deltaTime);
    }

    public void Jump(Vector3 _dir)
    {
        if (jumpCurr > 0)
        {
            jumpCurr--;
            externalForce += _dir;
        }
    }


    public IEnumerator PlayStepSound()
    {
        if (playingFootstepSound) yield break;
        playingFootstepSound = true;
        if (footstepSounds != null)
        {
            if (footstepSounds.Length > 0 && AudioManager.instance != null) AudioManager.instance.PlaySound(footstepSounds[Random.Range(0, footstepSounds.Length)], AudioManager.soundType.player, footstepvol);
            if (footstepSoundRef != null) footstepSoundRef.TriggerSound(transform.position);
        }
        if (isSprinting)
        {
            yield return new WaitForSeconds(footstepDelay / sprintMod);
        }
        else
        {
            yield return new WaitForSeconds(footstepDelay);
        }
        playingFootstepSound = false;
    }

  




    #region Obsolete (Temporary for rework)
    //void Sprint()
    //{
    //    if (Input.GetButton("Sprint") && !isSprinting)
    //    {
    //        acceleration *= sprintMod;
    //        isSprinting = true;
    //    }
    //    else if (!Input.GetButton("Sprint") && isSprinting)
    //    {
    //        acceleration /= sprintMod;
    //        isSprinting = false;
    //    }
    //}

    //void Walljump()
    //{
    //    RaycastHit hit;
    //    Walljumpdir = new Vector3(0, jumpHeight, 0);
    //    if (Physics.Raycast(GameManager.instance.playerRef.transform.position, GameManager.instance.playerRef.transform.right, out hit, 0.6f, jumplayer))
    //    {
    //        jumpCurr = 0;
    //        onWall = true;
    //        if (Input.GetButtonDown("Jump") && !controllerRef.isGrounded)
    //        {
    //            Walljumpdir = new Vector3(-playerVel.x, jumpHeight, -playerVel.z);
    //        }
    //    }
    //    else if (Physics.Raycast(GameManager.instance.playerRef.transform.position, -GameManager.instance.playerRef.transform.right, out hit, 0.6f, jumplayer))
    //    {
    //        jumpCurr = 0;
    //        onWall = true;
    //        if (Input.GetButtonDown("Jump") && !controllerRef.isGrounded)
    //        {
    //            Walljumpdir = new Vector3(-playerVel.x, jumpHeight, -playerVel.z);
    //        }
    //        else if (Input.GetButtonDown("Jump") && controllerRef.isGrounded) { 


    //        }
    //    }
    //    else
    //    {
    //        onWall = false;
    //    }
    //    if (Input.GetButtonDown("Jump"))
    //    {
    //        Jump(Walljumpdir);
    //    }
    //}

    //void wallslide()
    //{

    //    if (onWall == true)
    //    {
    //        playerVel.y = 0;
    //        gravityStrength = gravityStrength /= wallgravity;
    //        gravityStrength = Mathf.Clamp(gravityStrength, 12, 26);
    //    }
    //    else {
    //        returnoriginalgravity();
    //    }
    //}
    //void returnoriginalgravity() {
    //    if (gravityStrength != originalgravity) {
    //        gravityStrength = originalgravity;
    //    }
    //}

    #endregion
}
