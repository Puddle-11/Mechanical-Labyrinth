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



    private void OnDisable()
    {
        ChunkGrid.instance.EndLoad -= RunSystem;

    }
    private void Start()
    {
        enemyDensity *= (GameManager.instance.currentStats.S_Level + 1);
        enemyDensity = (int)Mathf.Clamp(enemyDensity, minMaxEnemies.x, minMaxEnemies.y);

        ChunkGrid.instance.EndLoad += RunSystem;
    }




    public void RunSystem()
    {
        allPositions = ChunkGrid.instance.GetRoomGenerator().GetPositions();
        
        while (currentEnemyCount < enemyDensity)
        {
            int index = UnityEngine.Random.Range(0, allPositions.Count);
            SpawnEnemy(allPositions[index], 1);
        }
    }

    public void SpawnEnemy(Vector3 _pos, float _probability)
    {
        if (currentEnemyCount >= enemyDensity) return;
        if (UnityEngine.Random.Range(0.0f, 1.0f) < _probability)
        {
            EnemyType ET = enemyList[UnityEngine.Random.Range(0, enemyList.Length)];

           GameObject enemyObj = Instantiate(ET.enemyPrefab, _pos, Quaternion.identity);
            BaseEnemy baseEnRef;
            if(enemyObj.TryGetComponent<BaseEnemy>(out baseEnRef))
            {
                List<Vector3> enemyPatrolPoints = new List<Vector3>();
                for (int i = 0; i < patrolPointsCount; i++)
                {
                    int index = UnityEngine.Random.Range(0, allPositions.Count);
                   enemyPatrolPoints.Add( allPositions[index]);
                    allPositions.RemoveAt(index);
                }
                baseEnRef.SetPatrolPoints(enemyPatrolPoints.ToArray());
            }
            currentEnemyCount++;
        }


    }
}
