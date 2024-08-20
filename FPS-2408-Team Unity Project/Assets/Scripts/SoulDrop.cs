using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulDrop : MonoBehaviour
{
    [SerializeField] GameObject soul;
    [SerializeField] int healAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.instance.playerRef)
        {
            IHealth healthRef = null;
            GameManager.instance.playerRef.TryGetComponent<IHealth>(out healthRef);
            
            healthRef.UpdateHealth(+healAmount);

            Destroy(soul);
        }
    }
}
