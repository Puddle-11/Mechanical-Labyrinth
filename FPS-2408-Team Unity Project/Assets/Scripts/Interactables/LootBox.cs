using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBox : MonoBehaviour, IInteractable
{
    [SerializeField] private List<GameObject> guns;
    [SerializeField] private List<GameObject> ammos;
    [SerializeField] private Transform spawnPointForAmmo;
    [SerializeField] private Transform spawnPointForGun;
    [SerializeField] private Animator openAnamation;
    private bool isOpened = false;
    public void TriggerInteraction()
    {
        OpenLootBox();

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
            GameObject ammo = ammos[Random.Range(0, ammos.Count)];
            SpawnLoot(ammo, spawnPointForAmmo);
        }
    }

    public void SpawnLoot(GameObject item, Transform spawnPoint)
    {
        Instantiate(item, spawnPoint.position, spawnPoint.rotation);
    }

    public void OpenLootBox()
    {
        isOpened = true;

        openAnamation.SetTrigger("Open");

        StartCoroutine(SpawnLootAfterAnimation());
    }

    private IEnumerator SpawnLootAfterAnimation()
    {
        yield return new WaitForSeconds(0.2f);
        LootInBox();
    }

}
