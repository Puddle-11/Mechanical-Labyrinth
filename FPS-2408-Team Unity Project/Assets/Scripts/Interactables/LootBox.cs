using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBox : MonoBehaviour
{
    [SerializeField] private List<GameObject> guns;
    [SerializeField] private List<GameObject> ammos;
    [SerializeField] private Transform spawnPointForAmmo;
    [SerializeField] private Transform spawnPointForGun;

    private void Start()
    {
        LootInBox();
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
            GameObject ammo = ammos[Random.Range(0, ammos.Count)];
            SpawnLoot(ammo, spawnPointForAmmo);
        }
    }

    public void SpawnLoot(GameObject item, Transform spawnPoint)
    {
        Instantiate(item, spawnPoint.position, spawnPoint.rotation);
    }
}
