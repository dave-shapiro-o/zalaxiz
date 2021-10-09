using System;
using System.Collections;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem smallExplosion;
    [SerializeField] private ParticleSystem bigExplosion;

    public static SFXManager sharedInstance;

    private void Awake()
    {
        if(!sharedInstance) { sharedInstance = this; }
        else { Destroy(gameObject); }
    }

    internal void Explosion(ParticleSystem explosion, Collider collider)
    {
        explosion.transform.position = collider.transform.position;
        explosion.Play();
    }

    internal void SmallExplosion(Collider collider)
        => Explosion(smallExplosion, collider);

    internal void BigExplosion(Collider collider)
        => Explosion(bigExplosion, collider);        

    private void OnPlayerHit(Collider collider)
        => SmallExplosion(collider);

    private void OnEnemyHit(Collider collider)
        => SmallExplosion(collider);

    private void OnBossOnFire(Collider collider)
        => StartCoroutine(nameof(WhileBossIsOnFire), collider);

    internal IEnumerator WhileBossIsOnFire(Collider bossCollider)
    {
        while (bossCollider.gameObject.activeInHierarchy)
        {
            SmallExplosion(bossCollider);
            yield return new WaitForSeconds(0.5F);
        }
    }

    private void OnBossEnemyHit(Collider collider)
        => SmallExplosion(collider);

    private void OnBossDied(Collider bossCollider)
        => BigExplosion(bossCollider);

    private void OnEnable()
    {
        Player.PlayerHit += SmallExplosion;
        Player.EnemyHit += SmallExplosion;
        PlayerProjectile.EnemyHit += SmallExplosion;
        PlayerProjectile.BossEnemyHit += SmallExplosion;
        GameManager.BossOnFire += OnBossOnFire;
        GameManager.BossDied += BigExplosion;
    }

    private void OnDisable()
    {
        Player.PlayerHit -= SmallExplosion;
        Player.EnemyHit -= SmallExplosion;
        PlayerProjectile.EnemyHit -= SmallExplosion;
        PlayerProjectile.BossEnemyHit -= SmallExplosion;
        GameManager.BossOnFire -= OnBossOnFire;
        GameManager.BossDied -= BigExplosion;
    }
}
