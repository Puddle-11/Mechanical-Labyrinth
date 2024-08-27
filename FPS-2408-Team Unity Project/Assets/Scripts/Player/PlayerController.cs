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
    [SerializeField] private float dashMod; 
    //[SerializeField] private float timer;
    private bool isDashing;
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
    private Vector3 playerVel;
    private PlayerHand playerHandRef;
    private GameObject playerSpawnPos;

    public delegate void PlayerUsed();
    public PlayerUsed playerUseEvent;

    private float momentum;

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
        base.Awake();
    }
    // Start is called before the first frame update
    public override void Start()
    {

        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
        //SpawnPlayer();
        base.Start();
        UIManager.instance.UpdateHealthBar((float)currentHealth / maxHealth);
    }
    public override void Update()
    {
        base.Update();
        Movement();
        Sprint();

        //move = input * accelerationSpeed * Time.deltaTime;
        //move.x = Mathf.Clamp(move.x,0,maxSpeed);
        //move.y = Mathf.Clamp(move.y, 0, maxSpeed);
        //controllerRef.Move(move);
        if (UIManager.instance.GetStatePaused() || (BootLoadManager.instance != null && BootLoadManager.instance.IsLoading()))
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
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartCoroutine(Dash());
            }
            if (Input.GetMouseButtonDown(1))
            {
                playerHandRef?.toggleADS();
            }
        }

        wallslide();
       // Walljump();
        momentum = mass * acceleration;
    }
    public void UpdatePlayerSpeed(float _mod)
    {
        acceleration *= _mod;
    }
    public IEnumerator Dash()
    {
        if (isDashing) yield break;
        isDashing = true;
        acceleration = acceleration * dashMod;
        //timer = 2;
        //while (timer < 0 )
        //{
        //    yield return null;
        //    timer -= Time.deltaTime;
        //}
        yield return new WaitForSeconds(0.5f);
        isDashing = false;
        acceleration = acceleration / dashMod;
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
        UIManager.instance?.UpdateHealthBar((float)currentHealth / maxHealth);
    }
    // Update is called once per frame

  

    private void Movement()
    {
        //move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            playerVel.x = 0;
        playerVel.z = 0;

        if (controllerRef.isGrounded)
        {
            jumpCurr = 0;
            playerVel.y = 0;

        }
        else
        {
            playerVel.y -= gravityStrength * Time.deltaTime;
        }
        move = Input.GetAxis("Vertical") * transform.forward + Input.GetAxis("Horizontal") * transform.right;

        if (Mathf.Abs(move.x) > 0.01 || Mathf.Abs(move.y) > 0.01 || Mathf.Abs(move.z) > 0.01)
        {
            //LATER THIS LINE OF CODE NEEDS TO BE HOOKED UP TO A PLAYER ANIMATION
            footstepSoundRef?.TriggerSound(transform.position);
        }
        Vector2 tempVel = new Vector2(playerVel.x, playerVel.z) + new Vector2(move.x, move.z) * acceleration;
        if(tempVel.magnitude > maxSpeed)
        {
            tempVel = tempVel.normalized * maxSpeed;
        }
        playerVel = new Vector3(tempVel.x, playerVel.y, tempVel.y);
        controllerRef.Move(playerVel * Time.deltaTime);
        if (Input.GetButtonDown("Jump"))
        {
            Jump(new Vector3(playerVel.x, jumpHeight, playerVel.z));
        }
        controllerRef.Move(playerVel * Time.deltaTime);
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
        if (Physics.Raycast(GameManager.instance.playerRef.transform.position, GameManager.instance.playerRef.transform.right, out hit, 2f, jumplayer))
        {
            jumpCurr = 0;
            onWall = true;
            if (Input.GetButtonDown("Jump") && !controllerRef.isGrounded)
            {
                Walljumpdir = new Vector3(GameManager.instance.playerRef.transform.position.x, jumpHeight, 0);
            }
        }
        else if (Physics.Raycast(GameManager.instance.playerRef.transform.position, -GameManager.instance.playerRef.transform.right, out hit, 2f, jumplayer))
        {
            jumpCurr = 0;
            onWall = true;
            if (Input.GetButtonDown("Jump") && !controllerRef.isGrounded)
            {
                Walljumpdir = new Vector3(-GameManager.instance.playerRef.transform.position.x, jumpHeight, 0);
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

        if (onWall == true && playerVel.y < 0)
        {
            playerVel.y = 0;
            gravityStrength = gravityStrength /= wallgravity;
            gravityStrength = Mathf.Clamp(gravityStrength, 12, 26);
        }
    }

    public override void Death()
    {
        UIManager.instance.OpenLoseMenu();
    }
}
