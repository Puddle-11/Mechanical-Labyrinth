using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBox : MonoBehaviour
{
    [SerializeField] private List<GameObject> guns;
    [SerializeField] private List<GameObject> ammos;
    [SerializeField] private Transform spawnPoint;

    public void OpenLootBox()
    {
        float chance = Random.value;

        if (chance <= 0.3f)
        {
            GameObject gun = guns[Random.Range(0, guns.Count)];
            SpawnLoot(gun);
        }
        else
        {
            GameObject ammo = ammos[Random.Range(0, ammos.Count)];
            SpawnLoot(ammo);
        }
    }

    public void SpawnLoot(GameObject item)
    {
        Instantiate(item, spawnPoint.position, spawnPoint.rotation);
    }
}
