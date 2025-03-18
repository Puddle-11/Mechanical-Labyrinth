using System;
using System.Collections;
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

    [SerializeField] protected int burstSize;
    [SerializeField] protected int clipSizeMax;
    [SerializeField] protected float reloadSpeed;
    [SerializeField] protected float barrelDelay;
    [SerializeField] protected Barrel[] barrels;
    [SerializeField] protected float penetratingDistance;
    [SerializeField] protected float penetratingDamageFalloff;
    [SerializeField] protected float shieldPenetration;
    [Space]
    [Header("Accuracy Variables")]
    [Space]
    [SerializeField] private float FSAccuracy;
    [SerializeField] private AnimationCurve FSAOverTime;
    [SerializeField] private AnimationCurve Recoilx;
    [SerializeField] private AnimationCurve Recoily;
    [SerializeField] private float recoilCooldownFactor;
    [SerializeField] private float recoilFactorx;
    [SerializeField] private float recoilFactory;
    [Space]
    [Header("Cosmetics")]
    [Space]
    [SerializeField] protected float muzzleFlashSize;
    [SerializeField] private GameObject bulletTrail;
    [SerializeField] private GameObject bulletHoleDecal;
    [SerializeField] private Material[] NP_bulletHoleMat;
    [SerializeField] private Material[] P_bulletHoleMat;
    [SerializeField] private Material[] E_bulletHoleMat;
    [SerializeField] private Animator GunAnimator;
    [SerializeField] private Animator reloadAnim;
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
    public static bool Compare(BaseGun _gun1, BaseGun _gun2)
    {
        if(_gun1.shotType < _gun2.shotType)
        {
            return true;
        }
        else if(_gun1.shotType > _gun2.shotType)
        {
            return false;
        }
        else if(_gun1.shotType == _gun2.shotType)
        {
            if(_gun1.GetFireRate() * _gun1.shootDamage != _gun2.GetFireRate() * _gun2.shootDamage)
            {
                return _gun1.GetFireRate() * _gun1.shootDamage > _gun2.GetFireRate() * _gun2.shootDamage;
            }
            else
            {
                return _gun1.GetMaxClipSize() > _gun2.GetMaxClipSize();
            }
        }
        return true;
    }
    public override Pickup.PStats GetPStats()
    {
        return new Pickup.PStats(currAmmo);
    }
    public override void SetPStats(Pickup.PStats _val)
    {
        if (_val.uses < 0)
        {

            SetAmmo(GetMaxClipSize());
        }
        else
        {
            SetAmmo(_val.uses);
        }
    }

    public void OpenAmmoUI()
    {
        if (playerWeapon) UIManager.instance.UpdateExternalAmmoInv(true, (int)ammoType);
    }


    public override void Update()
    {   
        if (ShootConditional()) Attack();

        if (!isAttacking)
        {
    
            FSAtimer = Mathf.Clamp(FSAtimer - Time.deltaTime * recoilCooldownFactor, 0, Mathf.Infinity);
        }
       
        if (playerWeapon) UIManager.instance.UpdateCrosshairSpread(FSAccuracy * FSAOverTime.Evaluate(FSAtimer/ FSATimerMax));
    }
    #region Getters Setters
    public float GetFireRate() 
    {
       if(barrelDelay == 0)
        {
            return 1 / coolDown;

        }
        return 1 / barrelDelay;
    }
    public int GetDamage() {return shootDamage; }
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
        FSAtimer = 0;
        isReloading = false;
        isAttacking = false;
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

                    if (playerWeapon)
                    {
                        GameManager.instance.UpdateDamageDealt(shootDamage);
                        StartCoroutine(UIManager.instance.CallHitmarker());
                    }
                    healthRef.UpdateHealthAfterDelay(-shootDamage, Vector3.Distance(barrels[currBarrel].shootObj.transform.position, hit.collider.transform.position) / bulletTrail.GetComponent<BulletTracer>().GetSpeed(), shieldPenetration);
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

                            healthRef.UpdateHealthAfterDelay(-shootDamagecalc, Vector3.Distance(barrels[currBarrel].shootObj.transform.position, postPenetrateHit.collider.transform.position) / bulletTrail.GetComponent<BulletTracer>().GetSpeed(), shieldPenetration);
                            
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
                CameraController.instance.SetOffset(new Vector3(-Recoily.Evaluate(normalizedTimer) * recoilFactory, Recoilx.Evaluate(normalizedTimer) * recoilFactorx, 0));
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
            CameraController.instance.SetOffset(new Vector3(-Recoily.Evaluate(nTimer) * recoilFactory, Recoilx.Evaluate(nTimer) * recoilFactorx, 0));
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
        int fillAmount = clipSizeMax;

        if (playerWeapon)
        {

            fillAmount = AmmoInventory.instance.ammoCounts[(int)ammoType] > clipSizeMax ? clipSizeMax - currAmmo : AmmoInventory.instance.ammoCounts[(int)ammoType];

            if (!(AmmoInventory.instance.ammoCounts[(int)ammoType] > 0))
            {
                isReloading = false;
                yield break;
            }
            if(AudioManager.instance != null) AudioManager.instance.PlaySound(reloadStart, SettingsController.soundType.player, reloadVolume);
            if (reloadAnim != null) reloadAnim.SetBool("Reloading", true);
        }
        int finalFill = fillAmount + currAmmo;

        float timer = 0;
        float perBullVal = reloadSpeed / clipSizeMax;
        timer = currAmmo * perBullVal;




        while (timer < (finalFill) * perBullVal)
        {
            if (playerWeapon)
            {
                UIManager.instance.UpdateAmmoFill(timer / reloadSpeed);
            }
            timer += Time.deltaTime;
            yield return null;
        }
        if (playerWeapon)
        {
            AudioManager.instance.PlaySound(reloadEnd, SettingsController.soundType.player, reloadVolume);
            AmmoInventory.instance.UpdateAmmoInventory(ammoType, -fillAmount);
            UIManager.instance.UpdateExternalAmmoInv(true, (int)ammoType);
            if (reloadAnim != null) reloadAnim.SetBool("Reloading", false);
        }


        SetAmmo(finalFill);
        isReloading = false;


    }
}
