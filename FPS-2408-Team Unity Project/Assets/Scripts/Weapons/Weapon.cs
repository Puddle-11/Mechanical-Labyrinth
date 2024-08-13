using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IUsable
{
    protected bool usingItem;
    [SerializeField] protected float coolDown;
    protected bool isAttacking;
    public LayerMask ignoreMask;

    public bool GetUsingItem()
    {
        return usingItem;
    }
    public void SetUsingItem(bool _val)
    {
        usingItem = _val;
    }
    public void UseItem()
    {
        Attack();
    }
  
    public bool GetIsAttacking()
    {
        return isAttacking;
    }
    public virtual void Attack()
    {

        if (!isAttacking)
        {
            StartCoroutine(AttackDelay()); 
        }
    }
    private void OnDisable()
    {
        isAttacking = false;
    }
    public virtual IEnumerator AttackDelay()
    {
        isAttacking = true;
        //begin attack
        yield return new WaitForSeconds(coolDown);
        isAttacking = false;
    }
}
