using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicBox : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject mimicPrefab;

    public string GetStats()
    {
        return "";
    }

    public void TriggerInteraction()
    {
        Instantiate(mimicPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
