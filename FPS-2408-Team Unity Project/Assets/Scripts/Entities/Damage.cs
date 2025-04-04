using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
//====================================
//REWORKED
//====================================
public class Damage : MonoBehaviour
{

    [SerializeField] private damageType type;
    [SerializeField] private int damageAmount;
    [SerializeField] private float damageSpeed;
    [SerializeField] Vector3 Knockback;
    [SerializeField] private float knockbackmod;
    private bool dealingDamage;

    #region Custom Structs and Enums
    public enum damageType
    {
        single,
        continuous,
    }
    #endregion

    #region MonoBehavior Methods
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        if (type == damageType.single)
        {
            other.GetComponent<IHealth>()?.UpdateHealth(-damageAmount);
            knockbackPlayer();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger) return;
        StartCoroutine(DamageDelay(other.GetComponent<IHealth>()));
    }

    #endregion
    public IEnumerator DamageDelay(IHealth _ref)
    {
        if (dealingDamage || _ref == null) yield break;
        dealingDamage = true;
        _ref.UpdateHealth(-damageAmount);
        yield return new WaitForSeconds(damageSpeed);
        dealingDamage = false;

    }
    void knockbackPlayer() {
        Knockback = GameManager.instance.playerRef.transform.position - transform.position;
        GameManager.instance.playerControllerRef.SetForce(Knockback.normalized * knockbackmod);
    }
}