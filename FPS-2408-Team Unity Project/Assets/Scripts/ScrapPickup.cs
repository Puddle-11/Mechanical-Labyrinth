using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]

public class ScrapPickup : BasePickup
{
    [SerializeField] private Mesh[] models;
    public void Start()
    {

        transform.rotation = Quaternion.LookRotation((transform.position + Random.insideUnitSphere) - transform.position);
        int modelIndex = Random.Range(0,models.Length);

        GetComponent<MeshFilter>().mesh = models[modelIndex];
    }
    public override string GetStats()
    {
        return "";
    }

    public override void TriggerInteraction()
    {
        if (ScrapInventory.instance != null) ScrapInventory.instance.AddScrap(1);
        Destroy(gameObject);

    }
}
