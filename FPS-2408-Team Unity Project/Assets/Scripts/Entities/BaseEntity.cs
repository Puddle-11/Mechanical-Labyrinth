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
       StartCoroutine(changeIndicator(_amount > currentHealth ? Color.green : Color.red));
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

    private IEnumerator changeIndicator(Color _flashCol)
    {
        if(rendRef != null)
        {

        
        originalMaterial = rendRef.material;
        rendRef.material = damageMaterial;
        yield return new WaitForSeconds(damageFlashTime);
            rendRef.material = originalMaterial;
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
