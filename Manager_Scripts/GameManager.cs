using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class GameManager : MonoBehaviour
{
    public delegate void GameEventHandler();
    public static event GameEventHandler StartScreen;
    public static event GameEventHandler LifeLost; 
    public static event GameEventHandler AllLivesLost;
    public static event GameEventHandler PlayGame;
    public static event GameEventHandler Continue;
    public static event GameEventHandler ContinueBossFight;
    public static event GameEventHandler LevelComplete;

    public delegate void GameEnemyEventHandler(Collider collider);
    public static event GameEnemyEventHandler BossOnFire;
    public static event GameEnemyEventHandler BossDied; 

    public static GameObject player;
    private HudManager hud;

    public static bool isGameOver;
    public static bool isPlayerAlive;
    public static bool isPowerUp;
    public static bool isBossFight;

    private readonly int scoreValue = 30;
    private int currentScore;
    internal static int hiScore;

    internal static int lives;
    private int bossHitCount;

    public static GameManager sharedInstance;

    private void Awake()
    {
        if (!sharedInstance) { sharedInstance = this; }
        else { Destroy(gameObject); }
    }
    // Start is called before the first frame update
    void Start()
    {
        hud = HudManager.sharedInstance;

        bossHitCount = 0;
        lives = 3;
        hiScore = PlayerPrefs.GetInt("hiScore");
        StartScreen?.Invoke();
    }

    public void OnStartScreen()
    {
        isGameOver = true;
        player = GameObject.FindGameObjectWithTag("Player");
        player.SetActive(false);
    }
    public void Play()
    {
        PlayGame?.Invoke();
        isGameOver = false;
        player.SetActive(true);
        isPlayerAlive = true;
    }

    public void OnPowerUp()
    {
        if (!isPowerUp)
        {
            isPowerUp = true;
            StartCoroutine(nameof(PowerUpTimer));
        }     
    }

    IEnumerator PowerUpTimer()
    {
        yield return new WaitForSeconds(12);
        isPowerUp = false;
    }

    internal void OnBossFight() => isBossFight = true;   

    public void OnEnemyHit(Collider enemyCollider)
    {     
        enemyCollider.gameObject.SetActive(false);
        if (isPlayerAlive) 
        {
            currentScore += scoreValue;
            hud.UpdateScore(currentScore); 
        }
    }

    private void OnBossEnemyHit(Collider enemyCollider)
    {
        ++bossHitCount;
        if (bossHitCount == 20)
        {
            BossDied?.Invoke(enemyCollider);

            if (isPlayerAlive)
            {
                isBossFight = false;
                currentScore += scoreValue * 20;
                hud.UpdateScore(currentScore);
            }
            bossHitCount = 0;
            enemyCollider.gameObject.SetActive(false);

            if (isPlayerAlive) { LevelComplete?.Invoke(); }
        }
        if (bossHitCount == 6) 
        {
            BossOnFire?.Invoke(enemyCollider);
        }
    }

    private void OnLevelComplete()
        => StartCoroutine(nameof(WaitAndRestart));


    private IEnumerator WaitAndRestart()
    {
        yield return new WaitForSeconds(4);
        if (currentScore > hiScore) { hiScore = currentScore; }
        PlayerPrefs.SetInt("hiScore", hiScore);
        RestartGame();
    }

    public void OnPlayerHit(Collider other)
    {
        --lives;
        isPlayerAlive = false;
        player.SetActive(false);
        isPowerUp = false;

        if (lives == 0) 
        {
            AllLivesLost?.Invoke();
            StartCoroutine(nameof(OnAllLivesLost));
        }
        else
        {
            LifeLost?.Invoke();
            StartCoroutine(nameof(OnLifeLost));
        }
    }

    public IEnumerator OnAllLivesLost()
    {
        isGameOver = true;      
        yield return new WaitForSeconds(6);
        if (currentScore > hiScore) { hiScore = currentScore; }
        PlayerPrefs.SetInt("hiScore", hiScore);
        RestartGame();
    }

    private IEnumerator OnLifeLost()
    {
        yield return new WaitForSeconds(4);
        player.SetActive(true);
        player.transform.position = new Vector3(0, 0, 0);
        isPlayerAlive = true;

        if (isBossFight) 
        {
            bossHitCount = 20;
            ContinueBossFight?.Invoke();
        }
        else { Continue?.Invoke(); }      
    }

    private void RestartGame() 
        => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    private void OnEnable()
    {
        Player.PoweredUp += OnPowerUp;
        Player.PlayerHit += OnPlayerHit;
        Player.EnemyHit += OnEnemyHit;
        PlayerProjectile.EnemyHit += OnEnemyHit;
        PlayerProjectile.BossEnemyHit += OnBossEnemyHit;
        SpawnManager.BossFight += OnBossFight;

        StartScreen += OnStartScreen;
        LevelComplete += OnLevelComplete;
    }

    private void OnDisable()
    {
        Player.PoweredUp -= OnPowerUp;
        Player.PlayerHit -= OnPlayerHit;
        Player.EnemyHit -= OnEnemyHit;
        PlayerProjectile.EnemyHit -= OnEnemyHit;
        PlayerProjectile.BossEnemyHit -= OnBossEnemyHit;
        SpawnManager.BossFight += OnBossFight;

        StartScreen -= OnStartScreen;
        LevelComplete -= OnLevelComplete;
    }
}
