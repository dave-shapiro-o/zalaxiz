using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool sharedInstance;
    private static  List<GameObject> greyEnemyPool;
    private static List<GameObject> purpleTorpedoEnemyPool;
    private static List<GameObject> bossPool;
    private static readonly int poolSize = 40;

    [SerializeField] private GameObject greyEnemyPrefab;
    [SerializeField] private GameObject purpleTorpedoEnemyPrefab;
    [SerializeField] private GameObject firstLevelBossPrefab;

    private void Awake()
    {
        if (!sharedInstance) { sharedInstance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        greyEnemyPool = new List<GameObject>();
        purpleTorpedoEnemyPool = new List<GameObject>();
        bossPool = new List<GameObject>();

        // Create Level 1 enemies
        for (int i = 0; i < poolSize; ++i)
        {
               CreateEnemy("GreyFighter"); 
               CreateEnemy("PurpleTorpedo"); 
        }
        CreateEnemy("FirstLevelBoss");
    }

    private void CreateEnemy(string type)
    {
        GameObject enemyPrefab = new GameObject();
        List<GameObject> pool = new List<GameObject>();
        switch (type)
        {
            case "GreyFighter":
                enemyPrefab = greyEnemyPrefab;
                pool = greyEnemyPool;
                break;
            case "PurpleTorpedo":
                enemyPrefab = purpleTorpedoEnemyPrefab;
                pool = purpleTorpedoEnemyPool;
                break;
            case "FirstLevelBoss":
                enemyPrefab = firstLevelBossPrefab;
                pool = bossPool;
                break;
        }
        GameObject enemy = Instantiate(enemyPrefab);
        enemy.SetActive(false);
        pool.Add(enemy);
    }

    internal static GameObject GetEnemy(string type)
    {
        List<GameObject> pool = new List<GameObject>();
        switch (type)
        {
            case "GreyFighter":
                pool = greyEnemyPool;
                break;
            case "PurpleTorpedo":
                pool = purpleTorpedoEnemyPool;
                break;
            case "FirstLevelBoss":
                pool = bossPool;
                break;
        }
        int size = pool.Count;
        for (int i = 0; i < size; ++i)
        {
            if (!pool[i].activeInHierarchy)
            {
                return pool[i];
            }
        }
        return null;
    }
}
