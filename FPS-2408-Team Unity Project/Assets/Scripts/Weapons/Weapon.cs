using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IUsable
{
    [Space]
    [Header("WEAPONS GENERAL")]
    [Space]
    protected bool usingItem;
    [SerializeField] protected float coolDown;
    [SerializeField] protected bool isAttacking;
    public LayerMask ignoreMask;
    [SerializeField] protected GameObject pickUp;
    [SerializeField] protected ItemType gunDrop;


    [Space]
    [Header("Sound Variables")]
    [Space]

    [SerializeField] protected float attackVolume;
    [SerializeField] public AudioClip[] shootsounds;

    #region Getters Setters
    public virtual ItemType GetItemType() { return gunDrop; }
    public virtual bool CanAttack() { return !isAttacking; }
    public virtual string GetItemName() { return gunDrop.name; }
    public virtual string GetItemStats() { return "Speed: " + coolDown; }
    public void SetPickup(GameObject _pickup) { pickUp = _pickup; }
    public bool GetUsingItem() { return usingItem; }
    public void SetUsingItem(bool _val) { usingItem = _val; }
    public bool GetIsAttacking() { return isAttacking; }
    #endregion
    public void UseItem() { Attack(); }
    public GameObject GetPickup()
    {
        if (pickUp != null)
        {
            return pickUp;
        }
        else if (gunDrop?.Pickup != null)
        {
            return gunDrop.Pickup;
        }
        else
        {
            Debug.LogWarning("No pickup available on " + gameObject.name);
            return null;
        }
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
    protected virtual void PlayAttacksound() {
        AudioManager.instance.PlaySound(shootsounds[Random.Range(0, shootsounds.Length)],AudioManager.soundType.enemy);
    }
}
