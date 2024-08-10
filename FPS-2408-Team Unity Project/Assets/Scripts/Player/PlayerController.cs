using System.Collections;
using System.Collections.Generic;
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
    private bool isSprinting;
    [Header("Jump Variables")]
    [Space]

    [SerializeField] private float jumpHeight;
    [SerializeField] private int jumpMax;
    private int jumpCurr;
    [Header("Physics Variables")]
    [Space]
    [SerializeField] private float gravityStrength;
    private Vector3 playerVel;
    private PlayerHand playerHandRef;
    //for testing purposes only \/
    //[Header("Gun Variables")]
    //[Space]
    //[SerializeField] private bool isShooting;
    //[SerializeField] private float shootDist;
    //[SerializeField] private float shootSpeed;
    //[SerializeField] private int shootDamage;
    //[SerializeField] private LayerMask ignoreMask;
   
    
   
    public override void Awake()
    {
        if(!TryGetComponent<PlayerHand>(out playerHandRef))
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

    }

    // Update is called once per frame
    public override void Update()
    {

        base.Update();
        Movement();
        Sprint();
        if (Input.GetButton("Shoot"))
        {

            playerHandRef.UseItem();
        }
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
        
        //if (Input.GetButton("Shoot") && !isShooting)
        //{
        //    StartCoroutine(Shoot());
        //}
        
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

    //public IEnumerator Shoot()
    //{

    //    isShooting = true;
    //    RaycastHit hit;
    //    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
    //    {

    //        IHealth damageRef;
    //        if (hit.collider.TryGetComponent<IHealth>(out damageRef))
    //        {
    //            damageRef.UpdateHealth(-shootDamage);
    //        }
    //    }



    //    yield return new WaitForSeconds(shootSpeed);
    //    isShooting = false;
    //}

    public override void Death()
    {
        UIManager.instance.OpenLoseMenu();
    }
}
