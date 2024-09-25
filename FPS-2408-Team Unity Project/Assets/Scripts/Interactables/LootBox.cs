using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBox : MonoBehaviour, IInteractable
{
    [SerializeField] private List<GameObject> guns;
    [SerializeField] private GameObject healthDrop;
    [SerializeField] private Transform spawnPointForHealth;
    [SerializeField] private Transform spawnPointForGun;
    [SerializeField] private Animator openAnamation;

    private bool LootHasSpawned = false;
    public void TriggerInteraction()
    {
        if (!LootHasSpawned)
        {
            OpenLootBox();
        }
    }

    public string GetStats()
    {
        return "";
    }
    public void LootInBox()
    {
        float chance = Random.value;

        if (chance <= 0.3f)
        {
            GameObject gun = guns[Random.Range(0, guns.Count)];
            SpawnLoot(gun, spawnPointForGun);
        }
        else
        {
            SpawnLoot(healthDrop, spawnPointForHealth);
        }
    }

    public void SpawnLoot(GameObject item, Transform spawnPoint)
    {
        Instantiate(item, spawnPoint.position, spawnPoint.rotation);
    }

    public void OpenLootBox()
    {
        openAnamation.SetTrigger("Open");

        StartCoroutine(SpawnLootAfterAnimation());
    }

    private IEnumerator SpawnLootAfterAnimation()
    {
        yield return new WaitForSeconds(1.4f);
        LootInBox();
        LootHasSpawned = true;
    }

}
