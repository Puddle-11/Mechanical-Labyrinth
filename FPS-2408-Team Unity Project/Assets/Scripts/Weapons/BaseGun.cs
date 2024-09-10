using System;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseGun : Weapon
{
    [Space]
    [Header("General Gun Variables")]
    [Space]
    [SerializeField] private GunType shotType;
    [SerializeField] private AmmoInventory.bulletType ammoType;
    [SerializeField] private float scopeInZoom;
    [SerializeField] private int shootDamage;
    [SerializeField] private float shootDist;
    [SerializeField] private GameObject bulletTrail;
    [SerializeField] private GameObject bulletHoleDecal;
    [SerializeField] private Material[] NP_bulletHoleMat;
    [SerializeField] private Material[] P_bulletHoleMat;
    [SerializeField] private Material[] E_bulletHoleMat;

    [SerializeField] protected int burstSize;
    [SerializeField] protected int clipSizeMax;
    [SerializeField] protected float reloadSpeed;
    [SerializeField] protected float barrelDelay;
    [SerializeField] protected float muzzleFlashSize;
    [SerializeField] protected Barrel[] barrels;
    [SerializeField] protected float penetratingDistance;
    [SerializeField] protected float penetratingDamageFalloff;
    [Space]
    [Header("Accuracy Variables")]
    [Space]
    [SerializeField] private float FSAccuracy;
    [SerializeField] private AnimationCurve FSAOverTime;
    [SerializeField] private float recoilCooldownFactor;
    [SerializeField] private float maxRecoil;


    [Space]
    [Header("Cosmetics")]
    [Space]
    [SerializeField] private Animator GunAnimator;
    private float FSATimerMax;
    private float FSAtimer;
    private int currAmmo;
    private bool isReloading = false;
    private bool offTrigger;
    private int currBarrel;
    [System.Serializable]
     public struct Barrel
    {
        public Transform shootObj;
        public Animator muzzleFlash;
        public ParticleSystem[] sparks;
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
        OpenAmmoUI();
    }

    public override int GetUses()
    {
        return currAmmo;
    }
    public override void SetUses(int _val)
    {
        SetAmmo(_val);
    }

    public void OpenAmmoUI()
    {
        if (playerWeapon) UIManager.instance.UpdateExternalAmmoInv(true, (int)ammoType);
    }


    private void Update()
    {   
        if (ShootConditional()) Attack();

        if (!isAttacking)
        {
            if (playerWeapon)
            {
                CameraController.instance.ResetOffset(true);

            }
            FSAtimer = Mathf.Clamp(FSAtimer - Time.deltaTime * recoilCooldownFactor, 0, Mathf.Infinity);
        }
        else
        {
            if (playerWeapon) CameraController.instance.ResetOffset(false);

        }
        if (playerWeapon) UIManager.instance.UpdateCrosshairSpread(FSAccuracy * FSAOverTime.Evaluate(FSAtimer/ FSATimerMax));


    }
    #region Getters Setters
    public float GetZoomAmount(){ return scopeInZoom;}
    public int GetMaxClipSize() { return clipSizeMax; }
    public int GetCurrAmmo() { return currAmmo; }
    public override string GetItemStats() { return "\nSpeed: " + coolDown + "\n\nDamage: " + shootDamage + "\n\nAmmo Type: " + ammoType; }
    public override bool CanAttack() { return !(isAttacking || isReloading); }
    public void SetShootPos(Transform _pos){barrels[currBarrel].shootObj = _pos;}
    public AmmoInventory.bulletType GetAmmoType(){return ammoType;}
    public void SetPlayerGun(bool _val){ playerWeapon = _val;}

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
        if (BootLoadManager.instance != null) BootLoadManager.instance.stopLoadEvent += OpenAmmoUI;
        if (isAttacking) isAttacking = false; //safegaurding against edgecases with the AttackDelay Ienumerator
        if (playerWeapon) UIManager.instance.UpdateExternalAmmoInv(true, (int)GetAmmoType());

    }
    private void OnDisable()
    {
        if(BootLoadManager.instance!=null)BootLoadManager.instance.stopLoadEvent -= OpenAmmoUI;

        if (playerWeapon) UIManager.instance.UpdateExternalAmmoInv(false);

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

            #region Variable Setup
            float normalizedTimer = FSAtimer / FSATimerMax;
            FSAtimer += barrelDelay;
            bool penetrated = false;
            Vector3 tempForward = CameraController.instance.transform.forward;
            Vector3 shootDir = playerWeapon ? tempForward : barrels[currBarrel].shootObj.forward;
            if (!playerWeapon || (playerWeapon && !GameManager.instance.playerControllerRef.GetPlayerHand().GetIsAiming()))
            {
                shootDir += new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)) * FSAccuracy * FSAOverTime.Evaluate(normalizedTimer);
            }


            RaycastHit hit;
            IHealth healthRef = null;
            #endregion
            currBarrel++;
            if (currBarrel > barrels.Length - 1)
            {
                currBarrel = 0;
            }
            if (shootsounds.Length > 0 && AudioManager.instance != null) AudioManager.instance.PlaySound(shootsounds[UnityEngine.Random.Range(0, shootsounds.Length)], (playerWeapon ? SettingsController.soundType.player : SettingsController.soundType.enemy), attackVolume);
            UpdateAmmo(-1);
            StartMuzzleFlash();
            if(GunAnimator != null) GunAnimator.SetTrigger("Shoot");

            if (Physics.Raycast(playerWeapon ? Camera.main.transform.position : barrels[currBarrel].shootObj.position, shootDir, out hit, shootDist, ~ignoreMask))
            {
                if (hit.collider.TryGetComponent(out healthRef))
                {

                    if (playerWeapon) GameManager.instance.UpdateDamageDealt(shootDamage);
                    healthRef.UpdateHealthAfterDelay(-shootDamage, Vector3.Distance(barrels[currBarrel].shootObj.transform.position, hit.collider.transform.position) / bulletTrail.GetComponent<BulletTracer>().GetSpeed());
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

                            if (playerWeapon) GameManager.instance.UpdateDamageDealt(shootDamagecalc);

                            healthRef.UpdateHealthAfterDelay(-shootDamagecalc, Vector3.Distance(barrels[currBarrel].shootObj.transform.position, postPenetrateHit.collider.transform.position) / bulletTrail.GetComponent<BulletTracer>().GetSpeed());
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

            if (playerWeapon && barrelDelay > 0)
            {
                CameraController.instance.StartCamShake(barrelDelay <= 0 ? coolDown : barrelDelay, 0);
                CameraController.instance.SetOffsetPos(new Vector2(0, -maxRecoil * normalizedTimer));
            }
            if (barrelDelay != 0)
            {
                yield return wfs;
            }
        }
            float nTimer = FSAtimer / FSATimerMax;

        if (playerWeapon && barrelDelay <= 0)
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
        if (barrels[currBarrel].muzzleFlash != null)
        {
            barrels[currBarrel].muzzleFlash.transform.localScale = Vector3.one * muzzleFlashSize;
            barrels[currBarrel].muzzleFlash.SetTrigger("Flash");
            barrels[currBarrel].muzzleFlash.gameObject.transform.localEulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0, 180));
        }
        if (barrels[currBarrel].sparks.Length > 0)
        {
            foreach (ParticleSystem PS in barrels[currBarrel].sparks)
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
        if (playerWeapon == true) {
            UIManager.instance.AmmoDisplay(currAmmo, clipSizeMax);
            UIManager.instance.UpdateExternalAmmoInv(true,(int)ammoType);

        }
    }
    private void SummonBulletTracer(RaycastHit _path, Vector3 _dir)
    {
        GameObject trailRef = Instantiate(bulletTrail, barrels[currBarrel].shootObj.position, Quaternion.identity);
        BulletTracer BT;
        if (trailRef.TryGetComponent<BulletTracer>(out BT))
        {
            if (playerWeapon)
            {
                BT.SetPositions(barrels[currBarrel].shootObj.position, _path.collider != null ? _path.point : Camera.main.transform.position + _dir * shootDist);
            }
            else
            {
                BT.SetDirection(barrels[currBarrel].shootObj.position, _dir);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Debug.DrawRay(barrels[currBarrel].shootObj.position, barrels[currBarrel].shootObj.forward * 10, UnityEngine.Color.red);
    }

    public override void Attack()
    {
        if (gameObject.activeInHierarchy)
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
    }
    public IEnumerator Reload()
    {
        if (isReloading) yield break;
        if (currAmmo == clipSizeMax) yield break;
        isReloading = true;


        if (playerWeapon)
        {
            if (!(AmmoInventory.instance.ammoCounts[(int)ammoType] >= clipSizeMax))
            {
                isReloading = false;
                yield break;
            }
            AmmoInventory.instance.UpdateAmmoInventory(ammoType, -clipSizeMax);
            UIManager.instance.UpdateExternalAmmoInv(true, (int)ammoType);
        }

        float timer = 0;
        float perBullVal = reloadSpeed / clipSizeMax;
        timer = currAmmo * perBullVal;
        while (timer < reloadSpeed)
        {
            if (playerWeapon)
            {
                UIManager.instance.UpdateAmmoFill(timer / reloadSpeed);
            }
            yield return null;
            timer += Time.deltaTime;
        }
        
        SetAmmo(clipSizeMax);
        isReloading = false;
    }
}
