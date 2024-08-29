using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class PlayerController : BaseEntity
{
    private Vector3 move;


    private CharacterController controllerRef;
    [Header("Walk Variables")]
    [Space]
    [SerializeField] private SoundSenseSource footstepSoundRef;
    [SerializeField] private float acceleration;
    [SerializeField] private float sprintMod;
    private bool isSprinting;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float friction;
    //[SerializeField] private float timer;
    [SerializeField] private float mass;

    [Header("Jump Variables")]
    [Space]
    [SerializeField] private LayerMask jumplayer;
    [SerializeField] private float jumpHeight;
    [SerializeField] private int jumpMax;
    private int jumpCurr;
    [SerializeField] private Vector3 Walljumpdir;
    [SerializeField] private int WalljumpSpeed;
    [SerializeField] private int wallgravity;
    private bool onWall;

    [Header("Physics Variables")]
    [Space]
    [SerializeField] private float gravityStrength;
    float originalgravity;
    private Vector3 playerVel;
    private PlayerHand playerHandRef;
    private GameObject playerSpawnPos;
    [Header("Sounds")]
    [Space]
    [SerializeField] private float footstepDelay;
    public AudioClip[] footstepSounds;
    public AudioClip[] damageSounds;
    public delegate void PlayerUsed();
    public PlayerUsed playerUseEvent;
    private bool playingFootstepSound;
   [Range(0,1)][SerializeField] float footstepvol;
    private float momentum;


    private bool isDead;
    public override void Awake()
    {
        if (!TryGetComponent<PlayerHand>(out playerHandRef))
        {
            Debug.LogWarning("No player hand component found on " + gameObject.name);
        }
        if (!TryGetComponent<CharacterController>(out controllerRef))
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
    // Start is called before the first frame update
    public override void Start()
    {
        SetHealth(GameManager.instance.GetCurrentHealth());
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
        //SpawnPlayer();
        originalgravity = gravityStrength;
        base.Start();
        UIManager.instance.UpdateHealthBar((float)currentHealth / maxHealth);
    }
    public void SetPlayervel(Vector3 Playervel)
    {
        playerVel = Playervel;
    }
   public Vector3 GetPlayervel()
    {
        return playerVel;
    }
    public override void Update()
    {
        base.Update();
        Movement();
        Sprint();
        if (GameManager.instance.GetStatePaused() || (BootLoadManager.instance != null && BootLoadManager.instance.IsLoading()))
        {
            playerHandRef?.SetUseItem(false);
        }
        else
        {
            if (Input.GetButtonDown("Pick Up"))
            {

                playerHandRef?.ClickPickUp();

            }
            if (Input.GetButtonDown("Shoot"))
            {
                //playerUseEvent?.Invoke();
                playerHandRef?.SetUseItem(true);
            }
            if (Input.GetButtonUp("Shoot"))
            {
                playerHandRef?.SetUseItem(false);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                BaseGun tempOut;
                if (playerHandRef != null && playerHandRef.GetCurrentHand().TryGetComponent<BaseGun>(out tempOut))
                {
                    StartCoroutine(tempOut.Reload());
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                playerHandRef?.toggleADS();
            }
        }
        
        wallslide();
        Walljump();
        momentum = mass * acceleration;
    }
    public ItemType GetCurrentItemType()
    {
 
            return playerHandRef.GetCurrentItemType();
    }

    public void UpdatePlayerSpeed(float _mod)
    {
        acceleration *= _mod;
    }

    private bool TryFindPlayerSpawnPos(out GameObject _ref)
    {

            GameObject temp = GameObject.FindGameObjectWithTag("Player Spawn Pos");
        if(temp != null)
        {

            _ref = temp;

            return true;
        }

        _ref = null;

        return false;
    }
    public override void ResetHealth()
    {
        isDead = false;
        base.ResetHealth();
    }
    public bool spawnPlayer()
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

        base.SetHealth(_amount);
        GameManager.instance.SetCurrentHealth(currentHealth);
        UIManager.instance?.UpdateHealthBar((float)currentHealth / maxHealth);
    }
    // Update is called once per frame



    private void Movement()
    {
        move = Input.GetAxis("Vertical") * transform.forward + Input.GetAxis("Horizontal") * transform.right;
        playerVel.x = 0;
        playerVel.z = 0;

        if (controllerRef.isGrounded)
        {
            jumpCurr = 0;
            if (playerVel.y < 0)
            {
                playerVel.y = 0;
            }
        }
        else playerVel.y -= gravityStrength * Time.deltaTime;


        if (move.magnitude > 0.3 && controllerRef.isGrounded)
        {
            StartCoroutine(playStepSound());
        }

        Vector2 tempVel = new Vector2(playerVel.x, playerVel.z) + new Vector2(move.x, move.z) * acceleration;
        if (tempVel.magnitude > maxSpeed) tempVel = tempVel.normalized * maxSpeed;
        
        playerVel = new Vector3(tempVel.x, playerVel.y, tempVel.y);
        controllerRef.Move(playerVel * Time.deltaTime);
        if (Input.GetButtonDown("Jump"))
        {
            Jump(new Vector3(playerVel.x, jumpHeight, playerVel.z));
        }
        controllerRef.Move(playerVel * Time.deltaTime);

    }
    public IEnumerator playStepSound()
    {
        if (playingFootstepSound) yield break;
        playingFootstepSound = true;
        if(footstepSounds.Length > 0) AudioManager.instance.PlaySound(footstepSounds[Random.Range(0, footstepSounds.Length)], AudioManager.soundType.player, footstepvol);
        if(footstepSoundRef != null) footstepSoundRef.TriggerSound(transform.position);
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
    public void Jump(Vector3 _dir)
    {
        if (jumpCurr < jumpMax)
        {
            jumpCurr++;
            playerVel = _dir;
        }
    }
    void Sprint()
    {
        if (Input.GetButton("Sprint") && !isSprinting)
        {
            acceleration *= sprintMod;
            isSprinting = true;
        }
        else if (!Input.GetButton("Sprint") && isSprinting)
        {
            acceleration /= sprintMod;
            isSprinting = false;
        }
    }
    void Walljump()
    {
        RaycastHit hit;
        Walljumpdir = new Vector3(0, jumpHeight, 0);
        if (Physics.Raycast(GameManager.instance.playerRef.transform.position, GameManager.instance.playerRef.transform.right, out hit, 0.6f, jumplayer))
        {
            jumpCurr = 0;
            onWall = true;
            if (Input.GetButtonDown("Jump") && !controllerRef.isGrounded)
            {
                Walljumpdir = new Vector3(-playerVel.x, jumpHeight, -playerVel.z);
            }
        }
        else if (Physics.Raycast(GameManager.instance.playerRef.transform.position, -GameManager.instance.playerRef.transform.right, out hit, 0.6f, jumplayer))
        {
            jumpCurr = 0;
            onWall = true;
            if (Input.GetButtonDown("Jump") && !controllerRef.isGrounded)
            {
                Walljumpdir = new Vector3(-playerVel.x, jumpHeight, -playerVel.z);
            }
            else if (Input.GetButtonDown("Jump") && controllerRef.isGrounded) { 
            
            
            }
        }
        else
        {
            onWall = false;
        }
        if (Input.GetButtonDown("Jump"))
        {
            Jump(Walljumpdir);
        }
    }

    void wallslide()
    {

        if (onWall == true)
        {
            playerVel.y = 0;
            gravityStrength = gravityStrength /= wallgravity;
            gravityStrength = Mathf.Clamp(gravityStrength, 12, 26);
        }
        else {
            returnoriginalgravity();
        }
    }
    void returnoriginalgravity() {
        if (gravityStrength != originalgravity) {
            gravityStrength = originalgravity;
        }
    }

    public override void Death()
    {
        if (isDead) return;
        isDead = true;
        GameManager.instance.ResetAllStats();
        GameManager.instance.UpdateDeathCounter(1);
        UIManager.instance.OpenLoseMenu();
    }
}
