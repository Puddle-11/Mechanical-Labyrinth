using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class BaseGun : Weapon
{
    [SerializeField] private GunType shotType;
    [SerializeField] private int shootDamage;
    [SerializeField] private float shootDist;
    [SerializeField] private Transform shootPos;
    [SerializeField] private GameObject bulletTrail;
    [SerializeField] private int burstSize;
    [SerializeField] private int clipSizeMax;
    [SerializeField] private int currAmmo;
    [SerializeField] private float reloadSpeed;
    private bool isReloading = false;
    private bool playerGun = false;
    private bool offTrigger;
    private int burstCounter;
     
    public enum GunType
    {
        Automatic,
        Burst,
        Manual,
    }
    private void Update()
    {
        if (ShootConditional()) Attack();
    }
    #region Getters Setters
    public void SetShootPos(Transform _pos)
    {
        shootPos = _pos;
    }
    public void SetPlayerGun(bool _val)
    {
        playerGun = _val;
    }
    #endregion
    private bool ShootConditional()
    {
        if (usingItem)
        {
            switch (shotType)
            {
                case GunType.Automatic:
                    return true;
                case GunType.Burst:
                    if (burstCounter < burstSize)
                    {
                        return true;
                    }
                    break;
                case GunType.Manual:
                    if (offTrigger)
                    {
                        return true;
                    }
                    break;

            }
            offTrigger = false;
        }
        else
        {
            burstCounter = 0;
            offTrigger = true;
        }
            return false;
    }

    private void OnEnable()
    {
        if (isAttacking) isAttacking = false; //safegaurding against edgecases with the AttackDelay Ienumerator
    }

    public override IEnumerator AttackDelay()
    {
        isAttacking = true;


        if (playerGun)      CameraController.instance.StartCamShake(coolDown, 0);
        if (shotType ==  GunType.Burst)     burstCounter++;
        
        RaycastHit hit;
        if(Physics.Raycast(playerGun ? Camera.main.transform.position : shootPos.position, playerGun ? Camera.main.transform.forward : shootPos.forward, out hit, shootDist, ~ignoreMask))
        {


            IHealth healthRef;
            if(hit.collider.TryGetComponent<IHealth>(out healthRef))    healthRef.UpdateHealth(-shootDamage);
        }
        Debug.DrawRay(playerGun ? Camera.main.transform.position : shootPos.position, (playerGun ? Camera.main.transform.forward : shootPos.forward) * shootDist);
        SummonBulletTracer(hit);
        yield return new WaitForSeconds(coolDown);
        isAttacking = false;
    }
    private void SummonBulletTracer(RaycastHit _path)
    {
        GameObject trailRef = Instantiate(bulletTrail, shootPos.transform.position, Quaternion.identity);
        BulletTracer BT;
        if (trailRef.TryGetComponent<BulletTracer>(out BT))
        {
            if (playerGun)
            {
                BT.SetPositions(shootPos.transform.position, _path.collider != null ? _path.point : Camera.main.transform.position + Camera.main.transform.forward * shootDist);
            }
            else
            {
                BT.SetDirection(shootPos.transform.position, shootPos.forward);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Debug.DrawRay(shootPos.position,shootPos.forward * 10,Color.red);
    }

    public override void Attack()
    {
        if (!isAttacking && currAmmo > 0 && !isReloading)
        {
            currAmmo--;
            StartCoroutine(AttackDelay());
        }
        else if (currAmmo <= 0 && !isAttacking) 
        {
            StartCoroutine(Reload());
        }
        
    }

    public IEnumerator Reload()
    {
        if (isReloading) yield break;
        isReloading = true;
        yield return new WaitForSeconds(reloadSpeed);
        currAmmo = clipSizeMax;
        isReloading = false;
    }
}
