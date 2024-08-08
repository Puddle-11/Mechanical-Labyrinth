using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] float coolDown;
    private bool isAttacking;
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
    private IEnumerator AttackDelay()
    {
        isAttacking = true;
        //begin attack
        Debug.Log("attacked");
        yield return new WaitForSeconds(coolDown);
        isAttacking = false;
    }
}
