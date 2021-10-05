using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public sealed class GameManager : MonoBehaviour
{
    public static GameObject player;
    public static SpawnManager spawnManager;

    public static bool isGameOver;
    public static bool isPlayerAlive;
    public static bool isPowerUp;
    public static bool isBossFight;
    public static GameObject powerUp; 

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI titleText; 
    [SerializeField] private TextMeshProUGUI hiScoreText;

    [SerializeField] private GameObject canvas; 
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject lifeShip1;
    [SerializeField] private GameObject lifeShip2;
    [SerializeField] private GameObject lifeShip3;
    [SerializeField] private GameObject livesDisplay;


    [SerializeField] private int scoreValue;
    [SerializeField] private int currentScore;
    [SerializeField] private int lives;
    private static int hiScore;
    private static int bossHitCount = 20;

    [SerializeField] private AudioSource gameMusic;
    [SerializeField] private AudioSource gameOverMusic;
    [SerializeField] private AudioSource bossFightMusic; 

    [SerializeField] private AudioSource fxAudioSource;
    [SerializeField] private AudioClip playerDeath;
    [SerializeField] private AudioClip enemyDeath;
    [SerializeField] private AudioClip powerUpAudio;

    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private ParticleSystem bigExplosion;


    public static GameManager sharedInstance; 

    private void Awake()
    {
        if (!sharedInstance) { sharedInstance = this; }
        else { Destroy(gameObject); }
    }
    // Start is called before the first frame update
    void Start()
    {
        hiScore = PlayerPrefs.GetInt("hiScore");
        StartScreen();
    }

    internal void BossFight()
    {
        gameMusic.gameObject.SetActive(false);
        bossFightMusic.gameObject.SetActive(true);
        isBossFight = true;
    }

    public void StartScreen()
    {
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

        isGameOver = true;
        gameOverMusic.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(false);
        hiScoreText.text = $"hi score: {hiScore}";
        hiScoreText.gameObject.SetActive(true);

        titleText.gameObject.SetActive(true);
        startButton.SetActive(true);
        player = GameObject.FindGameObjectWithTag("Player");
        player.SetActive(false);
    }
    public void Play()
    {
        isGameOver = false;
        player.SetActive(true);
        isPlayerAlive = true;
        gameOverMusic.gameObject.SetActive(false);
        hiScoreText.gameObject.SetActive(false);
        gameMusic.gameObject.SetActive(true);
        livesDisplay.SetActive(true);
       
        fxAudioSource = GetComponent<AudioSource>();
        titleText.gameObject.SetActive(false);
        startButton.SetActive(false);

        scoreText.text = "Score: 0";
    }
    public void PowerUp(Collider pup)
    {
        isPowerUp = true;
        StartCoroutine(nameof(PowerUpTimer)); 
        fxAudioSource.PlayOneShot(powerUpAudio);
        Destroy(pup.gameObject);
    }
    IEnumerator PowerUpTimer()
    {
        yield return new WaitForSeconds(12);
        isPowerUp = false;
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

    public void UpdateScore(int value)
    {
        currentScore += value;
        scoreText.text = "Score: " + currentScore;
    }

    public void EnemyHit(Collider enemyCollider)
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
                isBossFight = false;
                enemyCollider.gameObject.SetActive(false);
                gameMusic.gameObject.SetActive(true);
                bossFightMusic.gameObject.SetActive(false);
                SpawnManager.enemyCount = 2;
                spawnManager.StartInvoke();
                fxAudioSource.PlayOneShot(playerDeath, 2.0f);
            }
            if (bossHitCount == 6) { StartCoroutine(BossOnFire(enemyCollider)); }
        }
        else 
        { 
            enemyCollider.gameObject.SetActive(false);
            fxAudioSource.PlayOneShot(enemyDeath, 2.0f);
        }

        if (isPlayerAlive) { UpdateScore(scoreValue); }
    }

    public void PlayerHit()
    {
        --lives;
        explosion.transform.position = player.gameObject.transform.position;
        gameMusic.gameObject.SetActive(false);
        bossFightMusic.gameObject.SetActive(false);
        fxAudioSource.PlayOneShot(playerDeath, 2.0f);

        switch (lives)
        {
            case 2:
                lifeShip1.SetActive(false);
                break;
            case 1:
                lifeShip2.SetActive(false);
                break;
            case 0:
                lifeShip3.SetActive(false);
                StartCoroutine(GameOver());
                return;
        }
        OnLifeLost();
        StartCoroutine(nameof(PlayerNonFinalDeath));
    }

    public IEnumerator GameOver()
    {
        isGameOver = true;
        OnLifeLost();
        
        fxAudioSource.PlayOneShot(playerDeath, 2.0f);
        StartCoroutine(nameof(PlayerFinalExplosion));
        gameMusic.gameObject.SetActive(false);
        bossFightMusic.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(true);
        livesDisplay.SetActive(false);
       
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
            gameMusic.gameObject.SetActive(true);
            SpawnManager.enemyCount = SpawnManager.enemyCount - 2 > 2 ? SpawnManager.enemyCount - 2 : 2;
        }
        else 
        { 
            bossFightMusic.gameObject.SetActive(true);
            SpawnManager.enemyCount = 13;   
            spawnManager.ActivateEnemy("FirstLevelBoss", 1);
        }
        
    }

    private IEnumerator PlayerFinalExplosion()
    {
        explosion.Play();
        yield return new WaitForSeconds(2);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
