using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SinFloat : MonoBehaviour
{
    [SerializeField] private Vector3 magnitude;
    [SerializeField] private float speed;
    private Vector3 originalPos;
    private float timer;
    private void Awake()
    {
        originalPos = transform.position; 
        
    }
    public void Update()
    {
        if(timer > Mathf.PI * 2)
        {
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime * speed;
        }
        transform.position = originalPos + new Vector3(Mathf.Sin(timer) * magnitude.x, Mathf.Sin(timer) * magnitude.y, Mathf.Sin(timer) * magnitude.z);
    }
}
