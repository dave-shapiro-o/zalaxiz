using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class GameManager : MonoBehaviour
{
    public delegate void GameEventHandler();
    public static event GameEventHandler StartScreen;

    public static GameObject player;
    public static SpawnManager spawnManager;

    public static bool isGameOver;
    public static bool isPlayerAlive;
    public static bool isPowerUp;
    public static bool isBossFight;
    public static GameObject powerUp;

    private readonly int scoreValue = 30;
    private int currentScore;
    internal static int hiScore;

    private int lives;
    private int bossHitCount;

    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private ParticleSystem bigExplosion;

    public static GameManager sharedInstance;
    private new AudioManager audio;
    private HudManager hud;

    private void Awake()
    {
        if (!sharedInstance) { sharedInstance = this; }
        else { Destroy(gameObject); }
    }
    // Start is called before the first frame update
    void Start()
    {
        audio = AudioManager.sharedInstance;
        hud = HudManager.sharedInstance;
        lives = 3;
        bossHitCount = 20;
        hiScore = PlayerPrefs.GetInt("hiScore");
        StartScreen?.Invoke();
    }

    public void OnStartScreen()
    {
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

        isGameOver = true;
        audio.Play("Game Over");

        player = GameObject.FindGameObjectWithTag("Player");
        player.SetActive(false);
    }
    public void Play()
    {
        isGameOver = false;
        player.SetActive(true);
        isPlayerAlive = true;
        audio.Stop("Game Over");
        audio.Play("Game");

        hud.OnPlay();
    }
    public void OnPowerUp(Collider pup)
    {
        isPowerUp = true;
        StartCoroutine(nameof(PowerUpTimer));
        Destroy(powerUp);
    }
    IEnumerator PowerUpTimer()
    {
        yield return new WaitForSeconds(12);
        isPowerUp = false;
    }

    internal void BossFight()
    {
        audio.Stop("Game");
        audio.Play("Boss Fight");
        isBossFight = true;
    }  

    public void OnEnemyHit(Collider enemyCollider)
    {
        explosion.transform.position = enemyCollider.gameObject.transform.position;
        explosion.Play();
        if (enemyCollider.gameObject.CompareTag("BossEnemy"))
        {
            --bossHitCount;
            if (bossHitCount == 0)
            {
                bigExplosion.transform.position = enemyCollider.gameObject.transform.position;
                bigExplosion.Play();
                if (isPlayerAlive) 
                { 
                    isBossFight = false;
                    currentScore += scoreValue * 20;
                    hud.UpdateScore(currentScore);
                }
                enemyCollider.gameObject.SetActive(false);
                audio.Stop("Boss Fight");
                audio.PlayFX("Player Death");
                if (isPlayerAlive) { StartCoroutine(nameof(LevelComplete)); }
            }
            if (bossHitCount == 6) { StartCoroutine(BossOnFire(enemyCollider)); }
        }
        else 
        { 
            enemyCollider.gameObject.SetActive(false);
            audio.PlayFX("Enemy Death");
        }

        if (isPlayerAlive) 
        {
            currentScore += scoreValue;
            hud.UpdateScore(currentScore); 
        }
    }

    internal IEnumerator BossOnFire(Collider enemyCollider)
    {
        while (enemyCollider.gameObject.activeInHierarchy)
        {
            explosion.transform.position = enemyCollider.gameObject.transform.position;
            explosion.Play();
            yield return new WaitForSeconds(0.5F);
        }
    }

    private IEnumerator LevelComplete()
    {
        audio.Play("Game Over");
        hud.OnLevelComplete();
        yield return new WaitForSeconds(4);
        if (currentScore > hiScore) { hiScore = currentScore; }
        PlayerPrefs.SetInt("hiScore", hiScore);
        RestartGame();
    }

    public void OnPlayerHit(Collider other)
    {
        --lives;
        explosion.transform.position = player.transform.position;
        audio.Stop("Game");
        audio.Stop("Boss Fight");
        audio.PlayFX("Player Death");

        if (lives == 0) 
        { 
            StartCoroutine(nameof(GameOver));
            return;
        }
        hud.OnPlayerHit(lives);
        
        OnLifeLost();
        StartCoroutine(nameof(PlayerNonFinalDeath));
    }

    public IEnumerator GameOver()
    {
        isGameOver = true;
        OnLifeLost();

        audio.PlayFX("Player Death");
        explosion.Play();
        yield return new WaitForSeconds(2);
        audio.Stop("Game");
        audio.Stop("Boss Fight");
        hud.OnGameOver();
       
        yield return new WaitForSeconds(4);
        if (currentScore > hiScore) { hiScore = currentScore; }
        PlayerPrefs.SetInt("hiScore", hiScore);
        RestartGame();
    }
    
    private void OnLifeLost()
    {
        isPlayerAlive = false;
        player.SetActive(false);
        Destroy(powerUp);
        isPowerUp = false;
    }

    private IEnumerator PlayerNonFinalDeath()
    {
        explosion.Play();
        yield return new WaitForSeconds(4);
        player.SetActive(true);
        player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = new Vector3(0, 0, 0);
        isPlayerAlive = true;
        if (!isBossFight)
        {
            audio.Play("Game");
            SpawnManager.enemyCount = SpawnManager.enemyCount - 2 > 2 ? SpawnManager.enemyCount - 2 : 2;
        }
        else 
        {
            audio.Play("Boss Fight");
            SpawnManager.enemyCount = 13;
            yield return new WaitForSeconds(1.5F);
            bossHitCount = 20;
            spawnManager.ActivateEnemy("FirstLevelBoss", 1);
        }     
    }

    public void RestartGame() =>
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    private void OnEnable()
    {
        Player.PoweredUp += OnPowerUp;
        Player.PlayerHit += OnPlayerHit;
        Player.EnemyHit += OnEnemyHit;
        PlayerProjectile.EnemyHit += OnEnemyHit;
        StartScreen += OnStartScreen;
    }

    private void OnDisable()
    {
        Player.PoweredUp -= OnPowerUp;
        Player.PlayerHit -= OnPlayerHit;
        Player.EnemyHit -= OnEnemyHit;
        PlayerProjectile.EnemyHit -= OnEnemyHit;
        StartScreen -= OnStartScreen;
    }
}
