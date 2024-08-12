using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseEntity : MonoBehaviour, IHealth
{

    [SerializeField] private int maxHealth;
    private int currentHealth;
    [SerializeField] protected Renderer rendRef;
    [Range(0.1f, 10f)]
    [SerializeField] private float damageFlashTime;
    [SerializeField] private Material damageMaterial;
    private Material originalMaterial;
    private bool takingDamage;
    // Start is called before Start (used to initialize variables inside an object, DO NOT use awake to interact with other objects or components, this will crash your unity project
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
        currentHealth = maxHealth;
    }
    public virtual void SetHealth(int _amount)
    {

        _amount = _amount < 0 ? 0 : _amount;
        StartCoroutine(changeIndicator(damageMaterial));
        //clamps the _amount to a min of 0

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

    private IEnumerator changeIndicator(Material _flashMat)
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

        Destroy(gameObject);
    }

    // Update is called once per frame
}