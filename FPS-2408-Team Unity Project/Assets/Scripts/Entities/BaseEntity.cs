using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseEntity : MonoBehaviour, IHealth
{

    [SerializeField] protected int maxHealth;
    protected int currentHealth;
    [SerializeField] protected Renderer rendRef;
    [Range(0.1f, 10f)]
    [SerializeField] private float damageFlashTime;
    [SerializeField] protected Material damageMaterial;
    private Material originalMaterial;
    private bool takingDamage;
    [SerializeField] private GameObject[] drops;
    [SerializeField] protected EntityHealthBar healthBar;
    // Start is called before Start (used to initialize variables inside an object, DO NOT use awake to interact with other objects or components, this will crash your unity project


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

    }
    public virtual void Update()
    {
        
    }

    //=================================
    //IHealth interface functions
    //=================================

    #region IHealth Functions
    public virtual void ResetHealth()
    {
        SetHealth(maxHealth);
    }
    public virtual void SetHealth(int _amount)
    {

        _amount = _amount < 0 ? 0 : _amount;
       if(_amount < currentHealth) StartCoroutine(changeIndicator(damageMaterial));
        //clamps the _amount to a min of 0
        if(healthBar != null) healthBar.UpdateHealthBar((float)_amount, (float)maxHealth);
        currentHealth = _amount;
        if (_amount == 0)
        {
            Death();
        }
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
            if (takingDamage) yield break;

            takingDamage = true;
            originalMaterial = rendRef.material;
            rendRef.material = _flashMat;
            yield return new WaitForSeconds(damageFlashTime);
            rendRef.material = originalMaterial;
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
        DropInventory();
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
