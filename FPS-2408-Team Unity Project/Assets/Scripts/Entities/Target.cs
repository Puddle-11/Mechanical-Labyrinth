using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class Target : BaseEntity
{
    private Collider collRef;
    [SerializeField] private float RespawnTime;
    

    public override void Awake()
    {
        base.Awake();
        if (!TryGetComponent<Collider>(out collRef))
        {
            Debug.LogWarning("Failed to fetch Collider from " + gameObject.name);
        }
    }


    public override void Start()
    {
       
    }

    public override void Death()
    {
        StartCoroutine(RespawnDelay());
    }
    public IEnumerator RespawnDelay()
    {
        if (collRef == null || rendRef == null)
        {
            Debug.LogWarning("Target Respawn delay failed \nCollider or Renderer were unassigned\ndestroyed object");
            base.Death();
            yield break;
        }
        ResetHealth();

        collRef.enabled = false;
        rendRef.enabled = false;
        yield return new WaitForSeconds(RespawnTime);
        collRef.enabled = true;
        rendRef.enabled = true;


    }

}
