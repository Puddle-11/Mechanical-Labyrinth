using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseEntity : MonoBehaviour, IHealth
{
    [Space]
    [Header("BASE ENTITY GENERAL")]
    [Header("_______________________________")]

    [SerializeField] protected int maxHealth;
    [SerializeField] protected Renderer rendRef;
    [SerializeField] protected Material damageMaterial;
    [SerializeField] protected EntityHealthBar healthBar;
    [Range(0.1f, 10f)]
    [SerializeField] private float damageFlashTime;
    [SerializeField] private GameObject[] drops;
    // Start is called before Start (used to initialize variables inside an object, DO NOT use awake to interact with other objects or components, this will crash your unity project
   
    
    protected int currentHealth;
    private Material[] originalMaterial;
    private bool takingDamage;



    public virtual void Awake()
    {
        if( rendRef == null)
        {
            if (!TryGetComponent<Renderer>(out rendRef))
            {
                Debug.LogWarning("Failed to find renderer on " + gameObject.name);
            }
        }
        ResetHealth();
    }
    // Start is called before the first frame update
    public virtual void Start()
    {
        if (rendRef != null)
        {
            originalMaterial = rendRef.materials;
        }
    }
    public virtual void Update()
    {
        
    }

    //=================================
    //IHealth interface functions
    //=================================

    #region IHealth Functions
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    public int GetMaxHealth()
    {
        return maxHealth;
    }
    public void SetMaxHealth(int _val)
    {
        maxHealth = _val;
    }
    public virtual void ResetHealth()
    {
        SetHealth(maxHealth);
    }
    public virtual void SetHealth(int _amount)
    {
        _amount = Mathf.Clamp(_amount, 0, maxHealth);
        if(_amount < currentHealth && !takingDamage) StartCoroutine(changeIndicator(damageMaterial));
        //clamps the _amount to a min of 0
        if(healthBar != null) healthBar.UpdateHealthBar((float)_amount, (float)maxHealth);
        currentHealth = _amount;
        if (_amount == 0) Death();
    }
    public virtual void UpdateHealth(int _amount)
    {
        SetHealth(currentHealth + _amount);
    }
    #endregion

    public IEnumerator changeIndicator(Material _flashMat)
    {
        if (rendRef != null)
        {
            takingDamage = true;

            Material[] damageArr = new Material[originalMaterial.Length];
            for (int i = 0; i < damageArr.Length; i++)
            {
                damageArr[i] = _flashMat;
            }
            rendRef.materials = damageArr;
            yield return new WaitForSeconds(damageFlashTime);
            rendRef.materials = originalMaterial;
            takingDamage = false;

        }
        else
        {
            Debug.LogWarning("Renderer unassigned on " + gameObject.name);
            yield break;
        }
    }

    public virtual void Death()
    {
        //default death case
        //DropInventory();
        Destroy(gameObject);
    }
    public virtual void DropInventory()
    {
        for (int i = 0; i < drops.Length; i++)
        {
            DropItem(drops[i]);
        }
    }
    public void DropItem(GameObject _drop)
    {
        Instantiate(_drop, transform.position, Quaternion.Euler(_drop.transform.rotation.eulerAngles.x, Random.Range(0, 180), _drop.transform.rotation.eulerAngles.z));

    }
    // Update is called once per frame
}
