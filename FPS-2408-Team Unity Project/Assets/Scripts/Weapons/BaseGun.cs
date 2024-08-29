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
    [SerializeField] private Barrel[] shootPos;
    [SerializeField] private GameObject bulletTrail;
    [SerializeField] private GameObject bulletHoleDecal;
    [SerializeField] private Material[] NP_bulletHoleMat;
    [SerializeField] private Material[] P_bulletHoleMat;
    [SerializeField] private Material[] E_bulletHoleMat;
    private int currBarrel =0;
    [SerializeField] protected int burstSize;
    [SerializeField] protected int clipSizeMax;
    [SerializeField] protected float reloadSpeed;
    [SerializeField] protected float barrelDelay;
    [SerializeField] protected float muzzleFlashSize;

    [SerializeField] protected float penetratingDistance;
    [SerializeField] protected float penetratingDamageFalloff;
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
    [System.Serializable]
    public struct Barrel {
        public Transform _pos;
        public Animator _muzzleFlash;
        public ParticleSystem[] _sparks;
    }

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
    public override bool CanAttack()
    {
        return !(isAttacking || isReloading);
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
        if (playerGun) UIManager.instance.UpdateCrosshairSpread(FSAccuracy * FSAOverTime.Evaluate(FSAtimer/ FSATimerMax));


    }
    #region Getters Setters
 
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
    //This is the only function that needs to be overridden for different types of weapons

    public override IEnumerator AttackDelay()
    {
        isAttacking = true;
        int size = 1;
        if (shotType == GunType.Burst) { size = burstSize; }
        WaitForSeconds wfs = new WaitForSeconds(barrelDelay);
        FSAtimer += coolDown;

        for (int i = 0; i < size; i++)
        {
            if (currAmmo == 0) break;
            if (shootPos.Length > 1)
            {
                currBarrel++;
                if(currBarrel > shootPos.Length - 1)
                {
                    currBarrel = 0;
                }
            }
            #region Variable Setup
            float normalizedTimer = FSAtimer / FSATimerMax;
            FSAtimer += barrelDelay;
            bool penetrated = false;
            Vector3 tempForward = CameraController.instance.transform.forward;
            Vector3 shootDir = playerGun ? tempForward : shootPos[currBarrel]._pos.forward;
            shootDir += new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)) * FSAccuracy * FSAOverTime.Evaluate(normalizedTimer);
            RaycastHit hit;
            IHealth healthRef = null;
            #endregion

            UpdateAmmo(-1);
            StartMuzzleFlash();
            if (Physics.Raycast(playerGun ? Camera.main.transform.position : shootPos[currBarrel]._pos.position, shootDir, out hit, shootDist, ~ignoreMask))
            {
                if (hit.collider.TryGetComponent<IHealth>(out healthRef))
                {

                    if (playerGun)  GameManager.instance.UpdateDamageDealt(shootDamage);
                    healthRef.UpdateHealth(-shootDamage);
                }
                RaycastHit penetratingHit;

                if (penetratingDistance > 0 && Physics.Raycast(hit.point + shootDir.normalized * penetratingDistance, -shootDir, out penetratingHit, penetratingDistance, ~GameManager.instance.penetratingIgnore))
                {
                    if (penetratingHit.collider == hit.collider)
                    {

                        RaycastHit postPenetrateHit;
                        Physics.Raycast(penetratingHit.point, shootDir, out postPenetrateHit, shootDist, ~ignoreMask);


                        if (postPenetrateHit.collider == null || postPenetrateHit.distance > 0.01f)
                        {
                            penetrated = true;
                            if (healthRef == null)
                            {
                                //spawn exit hole
                                SpawnBulletHole(bulletHoleDecal, penetratingHit.point + penetratingHit.normal * 0.1f, Quaternion.LookRotation(-penetratingHit.normal), penetratingHit.collider.transform, E_bulletHoleMat);
                            }
                        }
                        if (penetrated == true && postPenetrateHit.collider != null && postPenetrateHit.collider.TryGetComponent<IHealth>(out healthRef))
                        {
                            int shootDamagecalc = (int)(shootDamage / ((1 + penetratingHit.distance) * penetratingDamageFalloff));

                            if (playerGun) GameManager.instance.UpdateDamageDealt(shootDamagecalc);
                            healthRef.UpdateHealth(-shootDamagecalc);
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
                else if (!penetrated && healthRef == null)
                {

                    SpawnBulletHole(bulletHoleDecal, hit.point + hit.normal * 0.1f, Quaternion.LookRotation(-hit.normal), hit.collider.transform, NP_bulletHoleMat);
                }
            }
            SummonBulletTracer(hit, shootDir);

            if (playerGun && barrelDelay > 0)
            {
                CameraController.instance.StartCamShake(barrelDelay <= 0 ? coolDown : barrelDelay, 0);
                CameraController.instance.SetOffsetPos(new Vector2(0, -maxRecoil * normalizedTimer));
            }
            yield return wfs;
        }
        float nTimer = FSAtimer / FSATimerMax;

        if (playerGun && barrelDelay <= 0)
        {
            CameraController.instance.StartCamShake(barrelDelay <= 0 ? coolDown : barrelDelay, 0);
            CameraController.instance.SetOffsetPos(new Vector2(0, -maxRecoil * nTimer));
        }
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
        if (shootPos[currBarrel]._muzzleFlash != null)
        {
            shootPos[currBarrel]._muzzleFlash.transform.localScale = Vector3.one * muzzleFlashSize;
            shootPos[currBarrel]._muzzleFlash.SetTrigger("Flash");
            shootPos[currBarrel]._muzzleFlash.gameObject.transform.localEulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0, 180));
        }
        if (shootPos[currBarrel]._sparks.Length > 0)
        {
            foreach (ParticleSystem PS in shootPos[currBarrel]._sparks)
            {
                PS.Play();
            }
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
        GameObject trailRef = Instantiate(bulletTrail, shootPos[currBarrel]._pos.transform.position, Quaternion.identity);
        BulletTracer BT;
        if (trailRef.TryGetComponent<BulletTracer>(out BT))
        {
            if (playerGun)
            {
                BT.SetPositions(shootPos[currBarrel]._pos.transform.position, _path.collider != null ? _path.point : Camera.main.transform.position + _dir * shootDist);
            }
            else
            {
                BT.SetDirection(shootPos[currBarrel]._pos.transform.position, _dir);
            }
        }
    }


    public override void Attack()
    {
        if (gameObject.activeInHierarchy)
        {
            if (!isAttacking && currAmmo > 0 && !isReloading)
            {
                StartCoroutine(AttackDelay());
                
                PlayAttacksound();
            }
            else if (currAmmo <= 0 && !isAttacking)
            {
                StartCoroutine(Reload());
            }
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
