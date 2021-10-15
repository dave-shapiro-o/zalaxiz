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
    private void PlayStartScreenMusic()
        => Play("Game Over");

    private void PlayGameMusic()
    {
        Stop("Game Over");
        Play("Game");
    }

    private void PlayPowerUp()
        => PlayFX("Power Up");

    private void PlayPlayerHit(Collider collider)
    {
        Stop("Game");
        Stop("Boss Fight");
        PlayFX("Player Death");
    }

    private void PlayEnemyHit(Collider collider)
        => PlayFX("Enemy Death");

    private void PlayBossFightMusic()
    {
        Stop("Game");
        Play("Boss Fight");
    }

    private void AllLivesAreLost()
    {
        Stop("Game");
        Stop("Boss Fight");
    }

    private void PlayBossDead(Collider collider)
        => PlayFX("Player Death");

    private void LevelIsComplete()
    {
        Play("Game Over");
        Stop("Boss Fight");
    }

    private void OnEnable()
    {
        Player.PoweredUp += PlayPowerUp;
        Player.PlayerHit += PlayPlayerHit;
        Player.EnemyHit += PlayEnemyHit;
        PlayerProjectile.EnemyHit += PlayEnemyHit;
        PlayerProjectile.BossEnemyHit += PlayEnemyHit;
        GameManager.StartScreen += PlayStartScreenMusic;
        GameManager.AllLivesLost += AllLivesAreLost;
        GameManager.PlayGame += PlayGameMusic;
        GameManager.Continue += PlayGameMusic;
        GameManager.ContinueBossFight += PlayBossFightMusic;
        GameManager.BossDied += PlayBossDead;
        GameManager.LevelComplete += LevelIsComplete;
        SpawnManager.BossFight += PlayBossFightMusic;
    }

    private void OnDisable()
    {
        Player.PoweredUp -= PlayPowerUp;
        Player.PlayerHit -= PlayPlayerHit;
        Player.EnemyHit -= PlayEnemyHit;
        PlayerProjectile.EnemyHit -= PlayEnemyHit;
        PlayerProjectile.BossEnemyHit -= PlayEnemyHit;
        GameManager.StartScreen -= PlayStartScreenMusic;
        GameManager.AllLivesLost -= AllLivesAreLost;
        GameManager.PlayGame -= PlayGameMusic;
        GameManager.Continue -= PlayGameMusic;
        GameManager.ContinueBossFight -= PlayBossFightMusic;
        GameManager.BossDied -= PlayBossDead;
        GameManager.LevelComplete -= LevelIsComplete;
        SpawnManager.BossFight -= PlayBossFightMusic;
    }

}
