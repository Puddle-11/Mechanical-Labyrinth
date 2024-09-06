using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//====================================
//REWORKED
//====================================
public class BaseEntity : MonoBehaviour, IHealth
{
    [Space]
    [Header("BASE ENTITY GENERAL")]
    [Header("_______________________________")]

    [SerializeField] protected int maxHealth;
    [SerializeField] protected Material damageMaterial;
    [SerializeField] protected EntityHealthBar healthBar;
    [SerializeField] protected ParticleSystem[] deathParticles;
    [SerializeField] protected GameObject hitParticles;
    [Range(0.1f, 10f)] [SerializeField] private float damageFlashTime;
    [SerializeField] private GameObject[] drops;   
    
    [SerializeField] protected RenderContainer[] rendRef;
    protected int currentHealth;
    private bool takingDamage;

    #region Custom Structs
    [System.Serializable]
    public struct RenderContainer
    {
        public Renderer currRenderer;
        [HideInInspector] public Material[] renderOriginMaterial;
        [HideInInspector] public Material damageMaterial;
        public void InitializeMaterials() { renderOriginMaterial = currRenderer.materials;}
        public void InitializeMaterials(Material[] _val) {
            currRenderer.materials = _val;
            InitializeMaterials();
        }
        public void ResetMaterials() {currRenderer.materials  = renderOriginMaterial;}
        public void SetMaterials(Material[] _newMats) { currRenderer.materials = _newMats;}
        public void SetMaterials(Material _newMat)
        {
            Material[] tempArr = new Material[currRenderer.materials.Length];
            for (int i = 0; i < tempArr.Length; i++)
            {
                tempArr[i] = _newMat;
            }
            currRenderer.materials = tempArr;
        }
        public Material[] GetMaterials() { return currRenderer.materials; }
    }
    #endregion

    #region IHealth Methods

    //=================================
    //IHealth interface functions
    public int GetCurrentHealth() { return currentHealth; }
    public int GetMaxHealth() { return maxHealth;}
    public void SetMaxHealth(int _val) { maxHealth = _val;}
    public virtual void ResetHealth() {  SetHealth(maxHealth);}
    public virtual void UpdateHealth(int _amount) { SetHealth(currentHealth + _amount); }
    public virtual void SetHealth(int _amount)
    {

        _amount = Mathf.Clamp(_amount, 0, maxHealth);


        if(_amount < currentHealth && !takingDamage && damageMaterial != null) StartCoroutine(ChangeIndicator(damageMaterial));

        if(healthBar != null) healthBar.UpdateHealthBar((float)_amount, (float)maxHealth);

        currentHealth = _amount;
        if (_amount == 0) Death();
    }

    public void SetHealthAfterDelay(int _newHealth, float _delay)
    {
        StartCoroutine(SetHealthDelay(_newHealth, _delay));
    }
    public void UpdateHealthAfterDelay(int _newHealth, float _delay)
    {
        SetHealthAfterDelay(currentHealth+ _newHealth, _delay);
    }
    //=================================
    #endregion

    #region MonoBehvaior Methods
    public virtual void Awake()
    {
        ResetHealth();
    }
    public virtual void Start()
    {
        for (int i = 0; i < rendRef.Length; i++)
        {
            rendRef[i].InitializeMaterials();
        }
    }
    #endregion

    #region Getters and Setters
    public void SetRenderMaterials(Material _mat) { SetRenderMaterials(new Material[] { _mat }); }
    public void SetRenderMaterials(Material[] _mats)
    {
        for (int i = 0; i < rendRef.Length; i++)
        {
            if (_mats.Length == 1) rendRef[i].SetMaterials(_mats[0]);
            else rendRef[i].SetMaterials(_mats);
        }
    }
    #endregion

    #region Virtual Methods
    public virtual void Update() { }
    public virtual void Death()
    {
        if (gameObject != null)
        {
            if(deathParticles != null)
            for (int i = 0; i < deathParticles.Length; i++)
            {
                deathParticles[i].Play();
                deathParticles[i].gameObject.transform.parent = null;
                Destroy(deathParticles[i].gameObject, deathParticles[i].main.duration + deathParticles[i].main.startLifetime.constantMax);
            }
            Destroy(gameObject);
        }
    }
    public void PlayHitParticles(Vector3 _hitPos)
    {
        if (hitParticles == null || hitParticles.GetComponent<ParticleSystem>() == null) return;

        GameObject x = Instantiate(hitParticles, _hitPos, Quaternion.LookRotation(_hitPos - transform.position));
        ParticleSystem ps = x.GetComponent<ParticleSystem>();
        ps.Play();
        Destroy(x, ps.main.duration + ps.main.startLifetime.constantMax);

    }
    public virtual void DropInventory()
    {
        for (int i = 0; i < drops.Length; i++)
        {
            DropItem(drops[i]);
        }
    }
    #endregion


    private IEnumerator SetHealthDelay(int _amount, float _time)
    {
        yield return new WaitForSeconds(_time);
        SetHealth(_amount);
    }


    public IEnumerator ChangeIndicator(Material _flashMat)
    {
        if (rendRef == null) yield break;

        takingDamage = true;

        for (int i = 0; i < rendRef.Length; i++)
        {
            rendRef[i].SetMaterials(damageMaterial);
        }

        yield return new WaitForSeconds(damageFlashTime);

        for (int i = 0; i < rendRef.Length; i++)
        {
            rendRef[i].ResetMaterials();
        }

        takingDamage = false;
    }

    public void DropItem(GameObject _drop)
    {
        Instantiate(_drop, transform.position, Quaternion.Euler(_drop.transform.rotation.eulerAngles.x, Random.Range(0, 180), _drop.transform.rotation.eulerAngles.z));
    }
}
