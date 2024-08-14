using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEditor.Rendering;
using UnityEditor.UIElements;
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
    }
    public override void SetHealth(int _amount)
    {
        base.SetHealth(_amount);
        UIManager.instance.UpdateHealthBar((float)currentHealth / maxHealth);
        StartCoroutine(UIManager.instance.flashDamage());
        CameraController.instance.StartCamShake();
    }
    // Update is called once per frame
    public override void Update()
    {

        base.Update();
        Movement();
        Sprint();
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

        if (Input.GetButtonDown("Jump") && jumpCurr < jumpMax)
        {

            jumpCurr++;
            playerVel.y = jumpHeight;
        }
        controllerRef.Move(playerVel * Time.deltaTime);


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
            WalljumpSpeed = 0;
            Debug.Log("Right");
            gravityStrength = gravityStrength * wallgravity;
            if (Input.GetButtonDown("Jump"))
                Walljumpdir = transform.forward * playerVel.y + Vector3.left;

        }
        else if (Physics.Raycast(GameManager.instance.playerRef.transform.position, GameManager.instance.playerRef.transform.InverseTransformDirection(-transform.right), out hit, 2f, jumplayer))
        {
            jumpCurr = 0;
            onWall = true;
            WalljumpSpeed = 0;
            Debug.Log("Left");
            gravityStrength = gravityStrength * wallgravity;
            if (Input.GetButtonDown("Jump"))
                Walljumpdir = transform.right + transform.up;
        }
        else
        {
            onWall = false;
        }

    }
    void wallslide()
    {
        if (onWall == true && playerVel.y < 0)
        {

        }
    }
    


    public override void Death()
    {
        UIManager.instance.OpenLoseMenu();
    }
}
