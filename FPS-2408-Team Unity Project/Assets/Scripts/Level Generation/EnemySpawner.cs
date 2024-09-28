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
        public Vector3 enemyScale;
        public GameObject enemyPrefab;
        public float frequency;
    }
    #endregion

    #region MonoBehavior Methods
    private void OnDisable()
    {
        ChunkGrid.instance.EndLoad -= RunSystem;

    }
    private void Start()
    {
       if(GameManager.instance != null) enemyDensity *= (GameManager.instance.GetCurrentLevel() + 1);
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

            SpawnEnemy(allPositions[index], 1);
        }
    }

    public void SpawnEnemy(Vector3 _pos, float _probability)
    {
        if (currentEnemyCount >= enemyDensity) return;

        if (UnityEngine.Random.Range(0.0f, 1.0f) < _probability)
        {
            int index = prevEnemyIndex;

            for (int i = 0; i < 10; i++)
            {
                index = UnityEngine.Random.Range(0, enemyList.Length);
                if (index != prevEnemyIndex)
                {
                    prevEnemyIndex = index;
                    break;
                }
            }
                EnemyType ET = enemyList[index];

            GameObject enemyObj = Instantiate(ET.enemyPrefab, _pos, Quaternion.identity);
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
