using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.Rendering.UI;
//====================================
//REWORKED
//====================================
public class LootGenerator : MonoBehaviour
{
    [SerializeField] private LootTable[] allLoot;
    [SerializeField] private float lootDensity;
    [SerializeField] private int minLoot;
    [SerializeField] private GameObject defaultLoot;
    private List<Vector3> allPositions = new List<Vector3>();

    #region Custom Structs and Enums
    [System.Serializable]
    public struct LootTable
    {
        public Vector2Int levelRange;
        public lootObj[] Loot;
    }
    [System.Serializable]

    public struct lootObj
    {
        public GameObject _prefab;
        public int _frequency;
    }
    #endregion


    #region MonoBehavior Methods
    private void Start()
    {
        ChunkGrid.instance.EndLoad += RunSystem;
    }
    private void OnDisable()
    {
        ChunkGrid.instance.EndLoad -= RunSystem;

    }
    #endregion
    public void RunSystem()
    {
        allPositions = ChunkGrid.instance.GetRoomGenerator().GetPositions();
        Instantiate(defaultLoot, allPositions[Random.Range(0, allPositions.Count)], defaultLoot.transform.rotation);
        GenerateLoot(GameManager.instance.GetCurrentLevel());
    }
    public void GenerateLoot(int _level)
    {
        int amount = (int)(lootDensity * allPositions.Count);
        int currentLootTableIndex = 0;
        List<GameObject> FilteredLootTable = new List<GameObject>();
        //===================================
        //Get Loot Table of current level
        for (int i = 0; i < allLoot.Length; i++)
        {
            if (_level <= allLoot[i].levelRange.y && _level >= allLoot[i].levelRange.x)
            {
                currentLootTableIndex = i;
                break;
            }
            else if(i == allLoot.Length - 1)
            {
                Debug.LogWarning("Couldnt find a sutable loot table for level " + _level);
                return;
            }
        }
        //===================================

        for (int i = 0; i < allLoot[currentLootTableIndex].Loot.Length; i++)
        {
            for (int j = 0; j < allLoot[currentLootTableIndex].Loot[i]._frequency; j++)
            {
                FilteredLootTable.Add(allLoot[currentLootTableIndex].Loot[i]._prefab);
            }
        }
        if (amount < minLoot) amount = minLoot;
        for (int i = 0; i < amount; i++)
        {
            if (allPositions.Count <= 0) break;
            int positionindex = Random.Range(0, allPositions.Count);
            int lootIndex = Random.Range(0, FilteredLootTable.Count);
            Instantiate(FilteredLootTable[lootIndex], allPositions[positionindex],FilteredLootTable[lootIndex].transform.rotation);
            allPositions.RemoveAt(positionindex);
        }
    }
}
