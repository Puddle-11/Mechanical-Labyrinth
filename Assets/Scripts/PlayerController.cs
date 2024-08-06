using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 move;

    private CharacterController controllerRef;

    [SerializeField] private float speed;

    private void Awake()
    {
        if(TryGetComponent<CharacterController>(out controllerRef))
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
        //move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        move = Input.GetAxis("Vertical")* transform.forward + Input.GetAxis("Horizontal") * transform.right;

        controllerRef.Move(move* speed * Time.deltaTime);
    }
}
