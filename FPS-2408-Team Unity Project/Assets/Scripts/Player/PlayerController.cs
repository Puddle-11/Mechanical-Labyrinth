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
    [SerializeField] private float speed;
    [SerializeField] private float sprintMod;
    [SerializeField] private float mass;
    private bool isSprinting;
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
        base.Start();
        UIManager.instance.UpdateHealthBar((float)currentHealth / maxHealth);
        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");
        spawnPlayer();
    }

    public void spawnPlayer()
    {
        
        controllerRef.enabled = false;
        transform.position = playerSpawnPos.transform.position;
        controllerRef.enabled = true;
    }
    public override void SetHealth(int _amount)
    {
        base.SetHealth(_amount);
        if (_amount > currentHealth)
        {
            if (CameraController.instance != null) CameraController.instance.StartCamShake();

            if (UIManager.instance != null)
            {
                StartCoroutine(UIManager.instance.flashDamage());
            }
        }
                UIManager.instance?.UpdateHealthBar((float)currentHealth / maxHealth);
    }
    // Update is called once per frame
    public override void Update()
    {

        base.Update();
        Movement();
        Sprint();
        if (UIManager.instance.GetStatePaused())
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
            playerHandRef?.SetUseItem(true);
        }
        if (Input.GetButtonUp("Shoot"))
        {
            playerHandRef?.SetUseItem(false);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            BaseGun tempOut;
            if ((playerHandRef?.GetCurrentHand()).TryGetComponent<BaseGun>(out tempOut))
            {
                StartCoroutine(tempOut.Reload());
            }
        }
        }


        Walljump();
        momentum = mass * speed;
    }

    private void Movement()
    {
        //move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (controllerRef.isGrounded)
        {
            jumpCurr = 0;
            playerVel = Vector3.zero;
        }
        else
        {

            playerVel.y -= gravityStrength * Time.deltaTime;
        }
        move = Input.GetAxis("Vertical") * transform.forward + Input.GetAxis("Horizontal") * transform.right;
        controllerRef.Move(move * speed * Time.deltaTime);
        if (Input.GetButtonDown("Jump") )
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

            speed *= sprintMod;
            isSprinting = true;
        }
        else if (!Input.GetButton("Sprint") && isSprinting)
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }
    void Walljump()
    {
        RaycastHit hit;
        if (Physics.Raycast(GameManager.instance.playerRef.transform.position, GameManager.instance.playerRef.transform.right, out hit, 2f, jumplayer))
        {
            jumpCurr = 0;
            onWall = true;
        }
        else if (Physics.Raycast(GameManager.instance.playerRef.transform.position, -GameManager.instance.playerRef.transform.right, out hit, 2f, jumplayer))
        {
            jumpCurr = 0;
            onWall = true;
        }
        else
        {
            onWall = false;
        }
            Walljumpdir = new Vector3(0, jumpHeight, 0);
        if (Input.GetButtonDown("Jump"))
        {
            Jump(Walljumpdir);
        }

    }
    void wallslide()
    {
        while (onWall == true && playerVel.y < 0)
        {
            gravityStrength = gravityStrength / wallgravity;
        }
       
    }
    


    public override void Death()
    {
        UIManager.instance.OpenLoseMenu();
    }
}
