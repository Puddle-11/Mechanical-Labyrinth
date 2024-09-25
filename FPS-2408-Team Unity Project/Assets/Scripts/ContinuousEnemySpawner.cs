using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContinuousEnemySpawner : SharedEnemyBehavior
{
    [SerializeField] private Vector3 spawnOffset;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private float spawnInterval = 1;
    private float spawnTimer;
    [SerializeField] private int maxEnemies;
    private List< GameObject> instantiatedEnemies = new List<GameObject>();
    

    public override void Update()
    {

        if (spawnTimer > spawnInterval)
        {
            SpawnEnemy();
            spawnTimer = 0;
        }
        else
        {
            spawnTimer += Time.deltaTime;
        }
        base.Update();
    }
    public List<GameObject> CleanEnemyList()
    {
        
        return instantiatedEnemies.ToArray().Where(c => c != null).ToList();
    }
    public void SpawnEnemy()
    {
        Debug.Log("spawn attempted");
        bool temp = IsInRange();
            Debug.Log("In Range? " + temp);
        if (temp)
        {
            instantiatedEnemies = CleanEnemyList();
            if (instantiatedEnemies.Count > maxEnemies) return;
            int index = Random.Range(0, enemies.Length);
            instantiatedEnemies.Add(Instantiate(enemies[index], transform.position + spawnOffset, Quaternion.identity));
        }

    }

}
