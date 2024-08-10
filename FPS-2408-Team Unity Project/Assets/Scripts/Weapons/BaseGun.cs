using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class BaseGun : Weapon
{

    [SerializeField] private int shootDamage;
    [SerializeField] private float shootDist; 
    [SerializeField] private Transform shootPos;
    [SerializeField] private GameObject bulletTrail;
    [SerializeField] private LayerMask ignoreMask;
    private bool PlayerGun = false;
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
  
        RaycastHit hit;
        GameObject trailRef = Instantiate(bulletTrail, shootPos.transform.position, Quaternion.identity);
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
            if (hit.collider != null)
            {
                BT.SetPositions(shootPos.position, hit.point);
            }
            else
            {

                BT.SetPositions(shootPos.transform.position, PlayerGun ? Camera.main.transform.position + Camera.main.transform.forward * shootDist : shootPos.forward * shootDist);
            }
        }





        //RaycastHit hit = GlobalMethods.globalMethodsRef.RaycastFromCam();
        // IHealth healthRef;
        // if (hit.collider != null && hit.collider.TryGetComponent<IHealth>(out healthRef))
        // {
        //     healthRef.UpdateHealth(-shootDamage);
        // }

        yield return new WaitForSeconds(coolDown);
        isAttacking = false;

    }
}
