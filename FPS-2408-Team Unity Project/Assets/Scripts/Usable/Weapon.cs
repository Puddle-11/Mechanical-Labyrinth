using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : UsableItemBase
{
    [Space]
    [Header("WEAPONS GENERAL")]
    [Space]
    [SerializeField] protected float coolDown;
    [SerializeField] protected bool isAttacking;
    public LayerMask ignoreMask;
    [SerializeField] protected bool canAim;

    [Space]
    [Header("Sound Variables")]
    [Space]

    [SerializeField] protected float attackVolume;
    [SerializeField] public AudioClip[] shootsounds;
    [SerializeField] protected AudioClip reloadStart;
    [SerializeField] protected AudioClip reloadEnd;
    [Range(0,1)]
    [SerializeField] protected float reloadVolume;

    protected bool playerWeapon = false;

    #region Override Methods
    public override bool CanAim(){return canAim;}

    public override string GetItemStats() { return "Speed: " + coolDown; }

    public override void SetUsingItem(bool _val) { usingItem = _val; }
    public override void UseItem() { Attack(); }

    #endregion

    #region Getters Setters

    public virtual bool CanAttack() { return !isAttacking; }
    public bool GetIsAttacking() { return isAttacking; }
    #endregion
    public void SetPlayerWeapon(bool _val) {playerWeapon = _val;}

    
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
        AudioManager.instance.PlaySound(shootsounds[Random.Range(0, shootsounds.Length)], SettingsController.soundType.enemy);
    }
}
