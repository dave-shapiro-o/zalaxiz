using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class GameManager : MonoBehaviour
{
    public static event Action StartScreen;
    public static event Action LifeLost; 
    public static event Action AllLivesLost;
    public static event Action PlayGame;
    public static event Action Continue;
    public static event Action ContinueBossFight;
    public static event Action LevelComplete;

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

    private void ActivateStartScreen()
    {
        isGameOver = true;
        player = GameObject.FindGameObjectWithTag("Player");
        player.SetActive(false);
    }
    internal void Play()
    {
        PlayGame?.Invoke();
        isGameOver = false;
        player.SetActive(true);
        isPlayerAlive = true;
    }

    private void ActivatePowerUp()
    {
        if (!isPowerUp)
        {
            isPowerUp = true;
            StartCoroutine(nameof(PowerUpTimer));
        }     
    }

    private IEnumerator PowerUpTimer()
    {
        yield return new WaitForSeconds(12);
        isPowerUp = false;
    }

    private void ActivateBossFight() => isBossFight = true;   

    private void EnemyIsHit(Collider enemyCollider)
    {     
        enemyCollider.gameObject.SetActive(false);
        if (isPlayerAlive) 
        {
            currentScore += scoreValue;
            hud.UpdateScore(currentScore); 
        }
    }

    private void BossEnemyIsHit(Collider bossCollider)
    {
        ++bossHitCount;
        if (bossHitCount == 20)
        {
            BossDied?.Invoke(bossCollider);

            if (isPlayerAlive)
            {
                isBossFight = false;
                currentScore += scoreValue * 20;
                hud.UpdateScore(currentScore);
            }
            bossHitCount = 0;
            bossCollider.gameObject.SetActive(false);

            if (isPlayerAlive) { LevelComplete?.Invoke(); }
        }
        if (bossHitCount == 6) 
        {
            BossOnFire?.Invoke(bossCollider);
        }
    }

    private void LevelIsComplete()
        => StartCoroutine(nameof(WaitAndRestart));


    private IEnumerator WaitAndRestart()
    {
        yield return new WaitForSeconds(4);
        if (currentScore > hiScore) { hiScore = currentScore; }
        PlayerPrefs.SetInt("hiScore", hiScore);
        RestartGame();
    }

    private void PlayerIsHit(Collider other)
    {
        --lives;
        isPlayerAlive = false;
        player.SetActive(false);
        isPowerUp = false;

        if (lives == 0) 
        {
            StartCoroutine(nameof(OnAllLivesLost));
        }
        else
        {
            StartCoroutine(nameof(OnLifeLost));
        }
    }

    private IEnumerator OnAllLivesLost()
    {
        AllLivesLost?.Invoke();
        isGameOver = true;      
        yield return new WaitForSeconds(6);
        if (currentScore > hiScore) { hiScore = currentScore; }
        PlayerPrefs.SetInt("hiScore", hiScore);
        RestartGame();
    }

    private IEnumerator OnLifeLost()
    {
        LifeLost?.Invoke();
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
        Player.PoweredUp += ActivatePowerUp;
        Player.PlayerHit += PlayerIsHit;
        Player.EnemyHit += EnemyIsHit;
        PlayerProjectile.EnemyHit += EnemyIsHit;
        PlayerProjectile.BossEnemyHit += BossEnemyIsHit;
        SpawnManager.BossFight += ActivateBossFight;

        StartScreen += ActivateStartScreen;
        LevelComplete += LevelIsComplete;
    }

    private void OnDisable()
    {
        Player.PoweredUp -= ActivatePowerUp;
        Player.PlayerHit -= PlayerIsHit;
        Player.EnemyHit -= EnemyIsHit;
        PlayerProjectile.EnemyHit -= EnemyIsHit;
        PlayerProjectile.BossEnemyHit -= BossEnemyIsHit;
        SpawnManager.BossFight += ActivateBossFight;

        StartScreen -= ActivateStartScreen;
        LevelComplete -= LevelIsComplete;
    }
}
