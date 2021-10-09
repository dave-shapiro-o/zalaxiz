using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource gameMusic;
    [SerializeField] private AudioSource gameOverMusic;
    [SerializeField] private AudioSource bossFightMusic;

    [SerializeField] private AudioSource fxAudioSource;
    [SerializeField] private AudioClip playerShoot;
    [SerializeField] private AudioClip enemyShoot;
    [SerializeField] private AudioClip torpedoEnemyShoot;
    [SerializeField] private AudioClip bossEnemyShoot; 

    [SerializeField] private AudioClip playerDeath;
    [SerializeField] private AudioClip enemyDeath;
    [SerializeField] private AudioClip powerUpAudio;

    public static AudioManager sharedInstance;

    private void Awake()
    {
        if (!sharedInstance) { sharedInstance = this; }
        else { Destroy(gameObject); }
    }

    internal void Play(string title, bool on = true)
    {
        AudioSource music = title switch
        {
            "Game Over" => gameOverMusic,
            "Game" => gameMusic,
            "Boss Fight" => bossFightMusic,
            _ => gameOverMusic
        };
         music.gameObject.SetActive(on); 
    }

    internal void Stop(string title)
        => Play(title, false);

    internal void PlayFX(string sound)
    {
        AudioClip clip = sound switch
        {
            "Player Shoot" => playerShoot,
            "Enemy Shoot" => enemyShoot,
            "Torpedo Enemy Shoot" => torpedoEnemyShoot,
            "Boss Enemy Shoot" => bossEnemyShoot,
            "Power Up" => powerUpAudio,
            "Player Death" => playerDeath,
            "Enemy Death" => enemyDeath,
            _ => powerUpAudio
        };
        if(clip != null) { fxAudioSource.PlayOneShot(clip); }        
    }
    private void OnStartScreen()
        => Play("Game Over");

    private void OnPlay()
    {
        Stop("Game Over");
        Play("Game");
    }

    private void OnPowerUp(Collider other)
        => PlayFX("Power Up");

    private void OnPlayerHit(Collider collider)
    {
        Stop("Game");
        Stop("Boss Fight");
        PlayFX("Player Death");
    }

    private void OnEnemyHit(Collider collider)
        => PlayFX("Enemy Death");

    private void OnBossFight()
    {
        Stop("Game");
        Play("Boss Fight");
    }

    private void OnAllLivesLost()
    {
        Stop("Game");
        Stop("Boss Fight");
    }

    private void OnContinue()
        => Play("Game");

    private void OnBossDied(Collider collider)
        => PlayFX("Player Death");

    private void OnLevelComplete()
        => Play("Game Over");

    private void OnEnable()
    {
        Player.PoweredUp += OnPowerUp;
        Player.PlayerHit += OnPlayerHit;
        Player.EnemyHit += OnEnemyHit;
        PlayerProjectile.EnemyHit += OnEnemyHit;
        PlayerProjectile.BossEnemyHit += OnEnemyHit;
        GameManager.StartScreen += OnStartScreen;
        GameManager.AllLivesLost += OnAllLivesLost;
        GameManager.PlayGame += OnPlay;
        GameManager.Continue += OnContinue;
        GameManager.ContinueBossFight += OnBossFight;
        GameManager.BossDied += OnBossDied;
        GameManager.LevelComplete += OnLevelComplete;
        SpawnManager.BossFight += OnBossFight;
    }

    private void OnDisable()
    {
        Player.PoweredUp -= OnPowerUp;
        Player.PlayerHit -= OnPlayerHit;
        Player.EnemyHit -= OnEnemyHit;
        PlayerProjectile.EnemyHit -= OnEnemyHit;
        PlayerProjectile.BossEnemyHit -= OnEnemyHit;
        GameManager.StartScreen -= OnStartScreen;
        GameManager.AllLivesLost -= OnAllLivesLost;
        GameManager.PlayGame -= OnPlay;
        GameManager.Continue -= OnContinue;
        GameManager.ContinueBossFight -= OnBossFight;
        GameManager.BossDied -= OnBossDied;
        GameManager.LevelComplete -= OnLevelComplete;
        SpawnManager.BossFight -= OnBossFight;
    }

}
