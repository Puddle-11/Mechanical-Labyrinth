using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class BaseGun : Weapon
{
    [SerializeField] private GunType shotType;
    [SerializeField] private int shootDamage;
    [SerializeField] private float shootDist;
    [SerializeField] private Transform shootPos;
    [SerializeField] private GameObject bulletTrail;
    [SerializeField] private LayerMask ignoreMask;
    [SerializeField] private int burstSize;
    private bool PlayerGun = false;
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
        if (usingItem)
        {
            switch (shotType)
            {
                case GunType.Automatic:
                    Attack();
                    break;
                case GunType.Burst:
                    if (burstCounter < burstSize)
                    {
                        Attack();
                    }
                    break;
                case GunType.Manual:
                    if (offTrigger)
                    {
                        Attack();
                    }
                    break;
                default:
                    break;
            }
            offTrigger = false;
        }
        else
        {
            burstCounter = 0;
            offTrigger = true;
        }
    }
    public void SetShootPos(Transform _pos)
    {
        shootPos = _pos;
    }
    public void SetPlayerGun(bool _val)
    {
        PlayerGun = _val;
    }
    public override IEnumerator AttackDelay()
    {
        isAttacking = true;
        if (shotType ==  GunType.Burst)
        {
            burstCounter++;
        }
        RaycastHit hit;
        GameObject trailRef = Instantiate(bulletTrail, shootPos.transform.position, Quaternion.identity);
        if (PlayerGun)
        {
            CameraController.instance.StartCamShake(coolDown, 0);
        }
        if(Physics.Raycast(PlayerGun ? Camera.main.transform.position : shootPos.position, PlayerGun ? Camera.main.transform.forward : shootPos.forward, out hit, shootDist, ~ignoreMask))
        {
            IHealth healthRef;
            if(hit.collider.TryGetComponent<IHealth>(out healthRef))
            {
                healthRef.UpdateHealth(-shootDamage);
            }
        }
            BulletTracer BT;
        if (trailRef.TryGetComponent<BulletTracer>(out BT))
        {
            if (PlayerGun)
            {
                if (hit.collider != null)
                {
                    BT.SetPositions(shootPos.transform.position, hit.point);
                }
                else
                {
                    BT.SetPositions(shootPos.transform.position, Camera.main.transform.position + Camera.main.transform.forward * shootDist);
                }
            }
            else
            {
                BT.SetDirection(shootPos.transform.position, shootPos.forward);

            }



        
        }
        yield return new WaitForSeconds(coolDown);
        isAttacking = false;
    }
    private void OnDrawGizmos()
    {
        Debug.DrawRay(shootPos.position,shootPos.forward * 10,Color.red);
    }
}
