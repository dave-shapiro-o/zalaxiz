using System;
using TMPro;
using UnityEngine;

public class HudManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI levelCompleteText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI hiScoreText;

    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject lifeShip1;
    [SerializeField] private GameObject lifeShip2;
    [SerializeField] private GameObject lifeShip3;
    [SerializeField] private GameObject livesDisplay;


    [SerializeField] private int scoreValue;
    private static int currentScore;

    public static HudManager sharedInstance;

    private void Awake()
    {
        if (!sharedInstance) { sharedInstance = this; }
        else { Destroy(gameObject); }
    }

    private void ActivateStartScreen()
    {
        gameOverText.gameObject.SetActive(false);
        hiScoreText.text = $"hi score: {GameManager.hiScore}";
        hiScoreText.gameObject.SetActive(true);

        titleText.gameObject.SetActive(true);
        startButton.SetActive(true);
    }

    internal void UpdateScore(int score)
    {
        currentScore = score;
        scoreText.text = "Score: " + currentScore;
    }

    private void ActivateGameOverScreen()
    {
        if (!GameManager.isLevelComplete) { gameOverText.gameObject.SetActive(true); }
        livesDisplay.SetActive(false);
    }

    private void ActivateLevelCompleteScreen()
    {
        if (GameManager.isPlayerAlive) 
        { 
            levelCompleteText.gameObject.SetActive(true); 
        }
    }
        
    private void ActivatePlayScreen()
    {
        hiScoreText.gameObject.SetActive(false);
        livesDisplay.SetActive(true);
        titleText.gameObject.SetActive(false);
        startButton.SetActive(false);
        scoreText.text = "Score: 0";
    }
    private void RemoveLifeShip(Collider collider)
    {
        switch (GameManager.lives)
        {
            case 3:
                lifeShip1.SetActive(false);
                break;
            case 2:
                lifeShip2.SetActive(false);
                break;
            case 1:
                lifeShip3.SetActive(false);
                break;
        }
    }

    private void OnEnable()
    {
        Player.PlayerHit += RemoveLifeShip;
        GameManager.StartScreen += ActivateStartScreen;
        GameManager.AllLivesLost += ActivateGameOverScreen;
        GameManager.PlayGame += ActivatePlayScreen;
        GameManager.LevelComplete += ActivateLevelCompleteScreen;
    }

    private void OnDisable()
    {
        Player.PlayerHit -= RemoveLifeShip;
        GameManager.StartScreen -= ActivateStartScreen;
        GameManager.AllLivesLost -= ActivateGameOverScreen;
        GameManager.PlayGame -= ActivatePlayScreen;
        GameManager.LevelComplete -= ActivateLevelCompleteScreen;
    }
}
