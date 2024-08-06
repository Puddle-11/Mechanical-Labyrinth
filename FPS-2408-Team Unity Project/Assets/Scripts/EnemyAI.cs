using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] private Renderer model;

    [SerializeField] private int maxHealth;
    private int currentHealth;

    private Color originalMatColor;





    private void Awake()
    {
        if(!TryGetComponent<Renderer>(out model))
        {
            Debug.LogWarning("Failed to get renderer on gameobject " + gameObject.name);
        }
        currentHealth = maxHealth;
        originalMatColor = model.material.color;
    }

    public void TakeDamage(int _amount)
    {
        currentHealth -= _amount;

        StartCoroutine(flashRed());


        if(currentHealth<= 0)
        {
            Death();
        }
    }
    void Death()
    {
        Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = originalMatColor;


    }
}
