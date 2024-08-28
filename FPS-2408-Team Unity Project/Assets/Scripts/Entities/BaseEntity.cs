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
    [SerializeField] protected RenderContainer[] rendRef;
    [SerializeField] protected Material damageMaterial;
    [SerializeField] protected EntityHealthBar healthBar;
    [Range(0.1f, 10f)]
    [SerializeField] private float damageFlashTime;
    [SerializeField] private GameObject[] drops;
    // Start is called before Start (used to initialize variables inside an object, DO NOT use awake to interact with other objects or components, this will crash your unity project
   
    
    protected int currentHealth;
    private bool takingDamage;

    public struct RenderContainer
    {
        public Renderer currRenderer;
        [HideInInspector] public Material[] renderOriginMaterial;
        [HideInInspector] public Material damageMaterial;
        public void ResetMaterials()
        {
            Material[] tempArr = new Material[currRenderer.materials.Length];
            for (int i = 0; i < tempArr.Length; i++)
            {
                tempArr[i] = renderOriginMaterial[i];

            }
            currRenderer.materials  = tempArr;
        }
        public void SetMaterials(Material _newMat)
        {
            Material[] tempArr = new Material[currRenderer.materials.Length];
            for (int i = 0; i < tempArr.Length; i++)
            {
                tempArr[i] = _newMat;
            }
            currRenderer.materials = tempArr;
        }
        public void GetMaterials()
        {
            renderOriginMaterial = currRenderer.materials;
        }
    }

    public virtual void Awake()
    {
        Renderer[] tempArr = GetComponentsInChildren<Renderer>();
        rendRef = new RenderContainer[tempArr.Length];
        for (int i = 0; i < rendRef.Length; i++)
        {
            rendRef[i].currRenderer = tempArr[i];
        }
        ResetHealth();
    }
    // Start is called before the first frame update
    public virtual void Start()
    {
        for (int i = 0; i < rendRef.Length; i++)
        {
            rendRef[i].GetMaterials();
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
        else
        {
            Debug.LogWarning("Renderer unassigned on " + gameObject.name);
            yield break;
        }
    }

    public virtual void Death()
    {
        if (gameObject != null)
        {
            //default death case
        Destroy(gameObject);
        }
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
