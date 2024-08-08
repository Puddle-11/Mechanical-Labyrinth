using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
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

    [Header("Gun Variables")]
    [Space]
    [SerializeField] private int shootDamage;
    [SerializeField] private float shootRate;
    [SerializeField] private float shootDist;
    private bool isShooting;
    [SerializeField] private LayerMask ignoreMask;
    private void Awake()
    {


        if(!TryGetComponent<CharacterController>(out controllerRef))
        {
            Debug.LogWarning("No character controller found, please assign a character controller to " + gameObject.name);
        }
    }
    // Start is called before the first frame update
    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {

   
        Movement();
        Sprint();
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
        if (Input.GetButton("Shoot") && !isShooting)
        {
            StartCoroutine(Shoot());
        }
    }
    void Sprint()
    {
        if(Input.GetButton("Sprint") && !isSprinting)
        {

            speed *= sprintMod;
            isSprinting = true;
        }
        else if(!Input.GetButton("Sprint") && isSprinting)
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }
    public IEnumerator Shoot()
    {
        isShooting = true;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
        {

            IHealth damageRef;
            if (hit.collider.TryGetComponent<IHealth>(out damageRef))
            {
                damageRef.UpdateHealth(-shootDamage);
            }
            DrawSceneBulletTracer(true);


        }
        else
        {

        DrawSceneBulletTracer(false);
        }


        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    private void DrawSceneBulletTracer(bool _hit)
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, _hit ? Color.green : Color.red);

    }
}
