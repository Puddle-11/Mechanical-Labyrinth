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
    [SerializeField] private float reloadSpeed;
    [SerializeField] private float barrelDelay;
    [Space]
    [Header("Accuracy Variables")]
    [Space]
    [SerializeField] private float FSAccuracy;
    [SerializeField] private AnimationCurve FSAOverTime;
    [SerializeField] private float recoilCooldownFactor;
    [SerializeField] private float maxRecoil;
    private float FSAtimer;
    private int currAmmo;
    private bool isReloading = false;
    private bool playerGun = false;
    private bool offTrigger;
     
    public enum GunType
    {
        Automatic,
        Burst,
        Manual,
    }
    public void Start()
    {
        SetAmmo(clipSizeMax);
    }
    private void Update()
    {
        
        if (ShootConditional()) Attack();
        else
        {
        }
        if (!isAttacking)
        {
            if (playerGun) CameraController.instance.ResetOffsetPos();
            FSAtimer = Mathf.Clamp(FSAtimer - Time.deltaTime * recoilCooldownFactor, 0, Mathf.Infinity);

        }

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
        bool res = false;
        if (!isReloading && usingItem)
        {
            
                switch (shotType)
                {
                    case GunType.Automatic:
                        res = true;
                        break;
                    case GunType.Burst:
                    case GunType.Manual:
                        if (offTrigger)
                        {
                            res = true;
                        }
                        break;

                }
                offTrigger = false;
            
        }
        else
        {
            
            offTrigger = true;
        }
        return res;
    }

    private void OnEnable()
    {
        if (isAttacking) isAttacking = false; //safegaurding against edgecases with the AttackDelay Ienumerator
    }

    public override IEnumerator AttackDelay()
    {
        isAttacking = true;

        int size = 1;
        float FSATimerMax = barrelDelay * size * clipSizeMax + coolDown * (clipSizeMax / size);
        if (shotType == GunType.Burst)
        {
            size = burstSize;
         FSATimerMax = barrelDelay * size * clipSizeMax;
        }
      
        WaitForSeconds wfs = new WaitForSeconds(barrelDelay);
        for (int i = 0; i < size; i++)
        {
            if (currAmmo == 0) break;
        float normalizedTimer = FSAtimer/ FSATimerMax;
            UpdateAmmo(-1);
            FSAtimer += barrelDelay;
            if (playerGun) CameraController.instance.StartCamShake(barrelDelay <= 0 ? coolDown : barrelDelay, 0);


            CameraController.instance.SetOffsetPos(new Vector2(0, -maxRecoil * normalizedTimer));


            Vector3 shootDir = playerGun ? Camera.main.transform.forward : shootPos.forward;
            shootDir += new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * FSAccuracy * FSAOverTime.Evaluate(normalizedTimer);
            yield return null;

            RaycastHit hit;
            if (Physics.Raycast(playerGun ? Camera.main.transform.position : shootPos.position, shootDir, out hit, shootDist, ~ignoreMask))
            {
                IHealth healthRef;
                if (hit.collider.TryGetComponent<IHealth>(out healthRef)) healthRef.UpdateHealth(-shootDamage);
            }
            SummonBulletTracer(hit, shootDir);
            yield return wfs;
        }
        FSAtimer += coolDown;
        yield return new WaitForSeconds(coolDown);
        isAttacking = false;
    }

    public void UpdateAmmo(int _val)
    {
        SetAmmo(currAmmo + _val);
    }
    public void SetAmmo(int _val)
    {
        if(_val > clipSizeMax) _val = clipSizeMax;
        if(_val < 0) _val = 0;
        
        currAmmo = _val;
        if (playerGun == true) UIManager.instance.ammoDisplay(currAmmo, clipSizeMax);
    }
    private void SummonBulletTracer(RaycastHit _path, Vector3 _dir)
    {
        GameObject trailRef = Instantiate(bulletTrail, shootPos.transform.position, Quaternion.identity);
        BulletTracer BT;
        if (trailRef.TryGetComponent<BulletTracer>(out BT))
        {
            if (playerGun)
            {
                BT.SetPositions(shootPos.transform.position, _path.collider != null ? _path.point : Camera.main.transform.position + _dir * shootDist);
            }
            else
            {
                BT.SetDirection(shootPos.transform.position, _dir);
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
        SetAmmo(clipSizeMax);
        isReloading = false;
    }
}
