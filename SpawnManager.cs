using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject greyFighterEnemyPrefab;
    [SerializeField] private GameObject greenTorpedoEnemyPrefab;
    [SerializeField] private GameObject powerUpPrefab;


    private readonly float spawnRange = 25F;
    private readonly float spawnY = 0.5F;
    private readonly float startDelay = 1F;
    private readonly float enemySpawnZ = 40F;
    private readonly float spawnDelay = 5F;
    private readonly float powerUpDelay = 30F;

    public static int enemyCount;

    // Start is called before the first frame update
    void Start()
    {
        enemyCount = 13;
        StartInvoke();
    }
    internal void StartInvoke()
    {
        InvokeRepeating(nameof(SpawnEnemy), startDelay, spawnDelay);
        InvokeRepeating(nameof(SpawnPowerUp), startDelay, powerUpDelay);
    }

    private void SpawnEnemy()
    {
        if (!GameManager.isGameOver && GameManager.isPlayerAlive)
        {
            if (enemyCount < 6)
            {
                ActivateEnemy("GreyFighter", enemyCount);
            }
            else if (enemyCount < 10)
            {
                ActivateEnemy("PurpleTorpedo", enemyCount);
            }
            else if (enemyCount <13)
            {
                ActivateEnemy("GreyFighter", enemyCount / 3);
                ActivateEnemy("PurpleTorpedo", enemyCount / 3);          
            }   
            else 
            {
                CancelInvoke();
                GameManager.sharedInstance.BossFight();
                ActivateEnemy("FirstLevelBoss", 1);
            }
        }      
    }

    internal void ActivateEnemy(string type, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject enemy = EnemyPool.GetEnemy(type);
            enemy.transform.position = SetSpawnPosition(type);         
            enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;
            enemy.SetActive(true);
        }
        ++enemyCount;
    }

    private Vector3 SetSpawnPosition(string type)
    {
        return type switch
        {
            "PurpleTorpedo" => new Vector3(Random.Range(-spawnRange, spawnRange), spawnY, enemySpawnZ - 20),
            "FirstLevelBoss" => new Vector3(0F, spawnY + 2F, enemySpawnZ),
            _=> new Vector3(Random.Range(-spawnRange, spawnRange), spawnY, enemySpawnZ)
        };
    }

    private void SpawnPowerUp()
    {
        if (!GameManager.isGameOver)
        {
            float randomPowerX = Random.Range(-spawnRange, spawnRange);
            float randomPowerZ = Random.Range(0, spawnRange);

            GameManager.powerUp = Instantiate(powerUpPrefab, new Vector3
                (randomPowerX, 1F, randomPowerZ), greyFighterEnemyPrefab.transform.rotation);            
        }                    
    }
}
