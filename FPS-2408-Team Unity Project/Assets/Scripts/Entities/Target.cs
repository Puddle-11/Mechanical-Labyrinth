using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//====================================
//REWORKED
//====================================
public class Target : BaseEntity
{
    [SerializeField] private float RespawnTime;
    private Collider collRef;

    #region MonoBehavior Methods
    public override void Awake()
    {
        if (!TryGetComponent(out collRef))
        {
            Debug.LogWarning("Failed to fetch Collider from " + gameObject.name);
        }
        base.Awake();
    }
    #endregion

    #region Override Methods
    public override void Death()
    {
        StartCoroutine(RespawnDelay());
    }
    #endregion

    public IEnumerator RespawnDelay()
    {
        if (collRef == null || rendRef == null)
        {
            Debug.LogWarning("Target Respawn delay failed \nCollider or Renderer were unassigned\nDestroying object...");
            base.Death();
            yield break;
        }
        healthBar.gameObject.SetActive(false);

        collRef.enabled = false;
        for (int i = 0; i < rendRef.Length; i++)
        {
            rendRef[i].currRenderer.enabled = false;
        }
        yield return new WaitForSeconds(RespawnTime);
        healthBar.gameObject.SetActive(true);
        ResetHealth();
        collRef.enabled = true;
        for (int i = 0; i < rendRef.Length; i++)
        {
            rendRef[i].currRenderer.enabled = true;
        }

    }

}
