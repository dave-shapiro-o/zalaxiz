using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    public static event Action BossFight;

    [SerializeField] private GameObject greyFighterEnemyPrefab;
    [SerializeField] private GameObject greenTorpedoEnemyPrefab;
    [SerializeField] private GameObject powerUpPrefab;

    private readonly float spawnRange = 25F;
    private readonly float spawnY = 0.5F;
    private readonly float startDelay = 1F;
    private readonly float enemySpawnZ = 40F;
    private readonly float spawnDelay = 5F;
    private readonly float powerUpStartDelay = 15F;
    private readonly float powerUpDelay = 30F;
    private static int enemyCount;

    private GameObject powerUp;

    public static SpawnManager sharedInstance;

    private void Awake()
    {
        if (!sharedInstance) { sharedInstance = this; }
        else { Destroy(gameObject); }
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyCount = 2;
        CreatePowerUp();
        StartInvoke();
    }

    private void CreatePowerUp()
    {
        float randomPowerX = Random.Range(-spawnRange, spawnRange);
        float randomPowerZ = Random.Range(0, spawnRange);

        powerUp = Instantiate(powerUpPrefab, new Vector3
            (randomPowerX, 1.5F, randomPowerZ), Quaternion.identity);
        powerUp.SetActive(false);
    }
    internal void DeactivatePowerUp()
    { 
        powerUp.SetActive(false);
    } 
    internal void StartInvoke()
    {
        InvokeRepeating(nameof(SpawnEnemy), startDelay, spawnDelay);
        InvokeRepeating(nameof(SpawnPowerUp), powerUpStartDelay, powerUpDelay);
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
                ActivateEnemy("FirstLevelBoss", 1);
                BossFight?.Invoke();
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
            "GreyFighter" => new Vector3(Random.Range(-spawnRange, spawnRange), spawnY, enemySpawnZ),
            "PurpleTorpedo" => new Vector3(Random.Range(-spawnRange, spawnRange), spawnY, enemySpawnZ - 20),
            "FirstLevelBoss" => new Vector3(0F, spawnY + 2F, enemySpawnZ + 200),
            _=> new Vector3(Random.Range(-spawnRange, spawnRange), spawnY, enemySpawnZ)
        };
    }

    private void SpawnPowerUp()
    {
        if (!GameManager.isGameOver && !GameManager.isPowerUp)
        {
            float randomPowerX = Random.Range(-spawnRange, spawnRange);
            float randomPowerZ = Random.Range(0, spawnRange);

            powerUp.SetActive(true);
            powerUp.transform.position =  new Vector3 (randomPowerX, 1.5F, randomPowerZ);
        }
    }

    private void ResetEnemyCountWhenLifeLost()
        => enemyCount = enemyCount - 2 > 2 ? enemyCount - 2 : 2;
     
    private void RestartBossFight()
    {
        enemyCount = 13;
        ActivateEnemy("FirstLevelBoss", 1);
    }

    private void OnEnable()
    {
        Player.PoweredUp += DeactivatePowerUp;
        GameManager.LifeLost += DeactivatePowerUp;
        GameManager.Continue += ResetEnemyCountWhenLifeLost;
        GameManager.ContinueBossFight += RestartBossFight;
    }

    private void OnDisable()
    {
        Player.PoweredUp -= DeactivatePowerUp;
        GameManager.LifeLost -= DeactivatePowerUp;
        GameManager.Continue -= ResetEnemyCountWhenLifeLost;
        GameManager.ContinueBossFight -= RestartBossFight;
    }
}
