using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int enemyDensity;
    [SerializeField] private Vector2Int minMaxEnemies;
    [SerializeField] private EnemyType[] enemyList;
    [SerializeField] private int patrolPointsCount;
    private int currentEnemyCount;
    private int prevEnemyIndex;

    private List<Vector3> allPositions = new List<Vector3>();


    #region Custom Structs and Enums
    [System.Serializable]
    private struct EnemyType
    {
        public Vector2Int levelRange;
        public GameObject[] enemyPrefab;

    }
    #endregion

    #region MonoBehavior Methods
    private void OnDisable()
    {
        ChunkGrid.instance.EndLoad -= RunSystem;

    }
    private void Start()
    {
        if (GameManager.instance != null) enemyDensity *= (GameManager.instance.GetCurrentLevel() + 1);
        enemyDensity = (int)Mathf.Clamp(enemyDensity, minMaxEnemies.x, minMaxEnemies.y);

        ChunkGrid.instance.EndLoad += RunSystem;
    }
    #endregion



    public void RunSystem()
    {
        allPositions = ChunkGrid.instance.GetRoomGenerator().GetPositions();


        while (currentEnemyCount < enemyDensity)
        {
            int index = UnityEngine.Random.Range(0, allPositions.Count);
            if (allPositions.Count <= 0) break;

            SpawnEnemy(allPositions[index], 1, GameManager.instance != null ? GameManager.instance.GetCurrentLevel() : 1);
        }
    }

    public void SpawnEnemy(Vector3 _pos, float _probability, int _level)
    {



        if (currentEnemyCount >= enemyDensity) return;

        if (UnityEngine.Random.Range(0.0f, 1.0f) < _probability)
        {


            int currentTableIndex = 0;
            for (int i = 0; i < enemyList.Length; i++)
            {
                if (_level <= enemyList[i].levelRange.y && _level >= enemyList[i].levelRange.x)
                {
                    currentTableIndex = i;
                    break;
                }
                else if (i == enemyList.Length - 1)
                {
                    Debug.LogWarning("Couldnt find a sutable enemy table for level " + _level);
                    currentEnemyCount++;

                    return;
                }
            }

            int index = prevEnemyIndex;

            for (int i = 0; i < 10; i++)
            {
                index = UnityEngine.Random.Range(0, enemyList[currentTableIndex].enemyPrefab.Length);
                if (index != prevEnemyIndex)
                {
                    prevEnemyIndex = index;
                    break;
                }
            }
            GameObject ET = enemyList[currentTableIndex].enemyPrefab[index];

            GameObject enemyObj = Instantiate(ET, _pos, Quaternion.identity);
            BaseEnemy baseEnRef;
            if (enemyObj.TryGetComponent(out baseEnRef))
            {
                List<Vector3> enemyPatrolPoints = new List<Vector3>();
                for (int i = 0; i < patrolPointsCount; i++)
                {
                    int pindex = UnityEngine.Random.Range(0, allPositions.Count);
                    enemyPatrolPoints.Add(allPositions[pindex]);
                    allPositions.RemoveAt(pindex);
                }
                baseEnRef.SetPatrolPoints(enemyPatrolPoints.ToArray());
            }
            currentEnemyCount++;
        }


    }
}
