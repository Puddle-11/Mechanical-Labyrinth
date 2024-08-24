using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int maxEnemyCount;
    [SerializeField] private EnemyType[] enemyList;
    private int currentEnemyCount;
    [SerializeField] private Vector3 offset;
    [SerializeField] private int patrolPointsCount;
    private List<Vector3> allPositions = new List<Vector3>();
    private void Start()
    {
        ChunkGrid.instance.EndLoad += RunSystem;
    }
    private void OnDisable()
    {
        ChunkGrid.instance.EndLoad -= RunSystem;

    }
    public void RunSystem()
    {
        Texture2D _texture = ChunkGrid.instance.GetRoomTexture();
        _texture.wrapMode = TextureWrapMode.Clamp;
        allPositions = new List<Vector3>();
        for (int x = 0; x < _texture.width; x++)
        {
            for (int y = 0; y < _texture.height; y++)
            {
                if(_texture.GetPixel(x,y) != Color.black)
                {
                    allPositions.Add(ChunkGrid.instance.GridToWorld(new Vector3Int(x, 0, y)) + offset);
                }
            }
        }
        while (currentEnemyCount < maxEnemyCount)
        {
            int index = UnityEngine.Random.Range(0, allPositions.Count);
            SpawnEnemy(allPositions[index], 1);
        }
    }
    [System.Serializable]
    private struct EnemyType
    {
        public Vector3 enemyScale;
        public GameObject enemyPrefab;
        public float frequency;
    }
    public void SpawnEnemy(Vector3 _pos, float _probability)
    {
        if (currentEnemyCount >= maxEnemyCount) return;
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
