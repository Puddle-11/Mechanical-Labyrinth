using System;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class BaseGun : Weapon
{
    [Space]
    [Header("General Gun Variables")]
    [Space]
    [SerializeField] private GunType shotType;
    [SerializeField] private int shootDamage;
    [SerializeField] private float shootDist;
    [SerializeField] private Transform shootPos;
    [SerializeField] private GameObject bulletTrail;
    [SerializeField] private GameObject bulletHoleDecal;
    [SerializeField] private Material[] NP_bulletHoleMat;
    [SerializeField] private Material[] P_bulletHoleMat;
    [SerializeField] private Material[] E_bulletHoleMat;

    [SerializeField] private int burstSize;
    [SerializeField] private int clipSizeMax;
    [SerializeField] private float reloadSpeed;
    [SerializeField] private float barrelDelay;
    [SerializeField] private Animator muzzleFlash;
    [SerializeField] private float muzzleFlashSize;
    [SerializeField] private ParticleSystem sparkParticles;

    [SerializeField] private float penetratingDistance;
    [SerializeField] private float penetratingDamageFalloff;
    [Space]
    [Header("Accuracy Variables")]
    [Space]
    [SerializeField] private float FSAccuracy;
    [SerializeField] private AnimationCurve FSAOverTime;
    [SerializeField] private float recoilCooldownFactor;
    [SerializeField] private float maxRecoil;

    private float FSATimerMax;
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
    private void Awake()
    {
        int size = 1;
        if (shotType == GunType.Burst) {size = burstSize;}
        FSATimerMax = barrelDelay * size * clipSizeMax + coolDown * (clipSizeMax / size);
    }
    public void Start()
    {

        SetAmmo(clipSizeMax);
    }

    public override string GetItemStats()
    {
        return "Speed: " + coolDown + "\nDamage: " + shootDamage;
    }
    private void Update()
    {
        
        if (ShootConditional()) Attack();

        if (!isAttacking)
        {
            if (playerGun) CameraController.instance.ResetOffset(true);
            FSAtimer = Mathf.Clamp(FSAtimer - Time.deltaTime * recoilCooldownFactor, 0, Mathf.Infinity);
        }
        else
        {
            if (playerGun) CameraController.instance.ResetOffset(false);

        }
        UIManager.instance.UpdateCrosshairSpread(FSAccuracy * FSAOverTime.Evaluate(FSAtimer/ FSATimerMax));


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
        if (shotType == GunType.Burst) { size = burstSize; }
        WaitForSeconds wfs = new WaitForSeconds(barrelDelay);
        for (int i = 0; i < size; i++)
        {
            if (currAmmo == 0) break;
            float normalizedTimer = FSAtimer/ FSATimerMax;
            UpdateAmmo(-1);
            FSAtimer += barrelDelay;
            if (playerGun)
            {
                CameraController.instance.StartCamShake(barrelDelay <= 0 ? coolDown : barrelDelay, 0);
                CameraController.instance.SetOffsetPos(new Vector2(0, -maxRecoil * normalizedTimer));

            }
            bool penetrated = false;
            Vector3 shootDir = playerGun ? CameraController.instance.transform.forward : shootPos.forward;
            shootDir += new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)) * FSAccuracy * FSAOverTime.Evaluate(normalizedTimer);
            StartMuzzleFlash();
            RaycastHit hit;
                IHealth healthRef = null;
            if (Physics.Raycast(playerGun ? CameraController.instance.mainCamera.transform.position : shootPos.position, shootDir, out hit, shootDist, ~ignoreMask))
            {
                if (hit.collider.TryGetComponent<IHealth>(out healthRef))
                {
                    healthRef.UpdateHealth(-shootDamage);
                }



                RaycastHit penetratingHit;

                if (penetratingDistance > 0 && Physics.Raycast(hit.point + shootDir.normalized * penetratingDistance, -shootDir, out penetratingHit, penetratingDistance, ~GameManager.instance.penetratingIgnore))
                {
                    penetrated = true;
                    if (healthRef == null)
                    {

                        SpawnBulletHole(bulletHoleDecal, penetratingHit.point + penetratingHit.normal * 0.1f, Quaternion.LookRotation(-penetratingHit.normal), penetratingHit.collider.transform, E_bulletHoleMat);
                    }
                    RaycastHit postPenetrateHit;
                    if (Physics.Raycast(penetratingHit.point, shootDir, out postPenetrateHit, shootDist, ~ignoreMask))
                    {
                        if (postPenetrateHit.collider.TryGetComponent<IHealth>(out healthRef))
                        {
                            int shootDamagecalc = (int)(-shootDamage / ((1 + penetratingHit.distance) * penetratingDamageFalloff));
                            healthRef.UpdateHealth(shootDamagecalc);
                        }
                    }
                }


            }
            if (hit.collider != null)
            {
                if (penetrated)
                {
                    SpawnBulletHole(bulletHoleDecal, hit.point + hit.normal * 0.1f, Quaternion.LookRotation(-hit.normal), hit.collider.transform, P_bulletHoleMat);
                }
                else if(!penetrated && healthRef == null)
                {

                    SpawnBulletHole(bulletHoleDecal, hit.point + hit.normal * 0.1f, Quaternion.LookRotation(-hit.normal), hit.collider.transform, NP_bulletHoleMat);
                }
            }
            SummonBulletTracer(hit, shootDir);
            yield return wfs;
        }
        FSAtimer += coolDown;
        yield return new WaitForSeconds(coolDown);
        isAttacking = false;
    }

    private GameObject SpawnBulletHole(GameObject _prefab, Vector3 _pos, Quaternion _rotation, Transform _parent, Material[] _mat)
    {

        GameObject res = Instantiate(_prefab, _pos, _rotation, _parent);
        BulletHole temp;
        if(_mat.Length > 0)
        {
            int index = UnityEngine.Random.Range(0, _mat.Length);
            if (res.TryGetComponent<BulletHole>(out temp)) temp.SetMat(_mat[index]);
        }


        return res;
    }
    private void StartMuzzleFlash()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.transform.localScale = Vector3.one * muzzleFlashSize;
            muzzleFlash.SetTrigger("Flash");
            muzzleFlash.gameObject.transform.localEulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0, 180));
        }
        if (sparkParticles != null)
        {
            sparkParticles.Play();
        }
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
        if (playerGun == true) UIManager.instance.AmmoDisplay(currAmmo, clipSizeMax);
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
        Debug.DrawRay(shootPos.position,shootPos.forward * 10, UnityEngine.Color.red);
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
        float timer = 0;
        float perBullVal = reloadSpeed / clipSizeMax;
        timer = currAmmo * perBullVal;
        while (timer < reloadSpeed)
        {
            if (playerGun)
            {
                UIManager.instance.UpdateAmmoFill(timer, reloadSpeed);
            }
            yield return null;
            timer += Time.deltaTime;
        }
        SetAmmo(clipSizeMax);
        isReloading = false;
    }
}
