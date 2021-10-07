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
        if (sharedInstance == null)
        {
            sharedInstance = this;
        }
        else { Destroy(gameObject); }
    }
    private void OnEnable()
    {
        GameManager.StartScreen += OnStartScreen;
    }

    private void OnDisable()
    {
        GameManager.StartScreen -= OnStartScreen;
    }

    internal void OnStartScreen()
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

    internal void OnGameOver()
    {
        gameOverText.gameObject.SetActive(true);
        livesDisplay.SetActive(false);
    }

    
    internal void OnLevelComplete()
    {
        levelCompleteText.gameObject.SetActive(true);
    }

    internal void OnPlay()
    {
        hiScoreText.gameObject.SetActive(false);
        livesDisplay.SetActive(true);
        titleText.gameObject.SetActive(false);
        startButton.SetActive(false);
        scoreText.text = "Score: 0";
    }
    internal void OnPlayerHit(int lives)
    {
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
                break;
        }
    }
}
