using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IUsable
{
    public void UseItem()
    {
        Attack();
    }
    [SerializeField] protected float coolDown;
    protected bool isAttacking;
    public bool GetIsAttacking()
    {
        return isAttacking;
    }
    public void Attack()
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
