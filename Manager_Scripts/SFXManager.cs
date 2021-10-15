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
    
    private void Explosion(ParticleSystem explosion, Collider collider)
    {
        explosion.transform.position = collider.transform.position;
        explosion.Play();
    }

    private void SmallExplosion(Collider collider)
        => Explosion(smallExplosion, collider);

    private void BigExplosion(Collider collider)
        => Explosion(bigExplosion, collider);        

    private void BossOnFireExplosions(Collider collider)
        => StartCoroutine(nameof(WhileBossIsOnFire), collider);

    private IEnumerator WhileBossIsOnFire(Collider bossCollider)
    {
        while (bossCollider.gameObject.activeInHierarchy)
        {
            SmallExplosion(bossCollider);
            yield return new WaitForSeconds(0.5F);
        }
    }

    private void OnEnable()
    {
        Player.PlayerHit += SmallExplosion;
        Player.EnemyHit += SmallExplosion;
        PlayerProjectile.EnemyHit += SmallExplosion;
        PlayerProjectile.BossEnemyHit += SmallExplosion;
        GameManager.BossOnFire += BossOnFireExplosions;
        GameManager.BossDied += BigExplosion;
    }

    private void OnDisable()
    {
        Player.PlayerHit -= SmallExplosion;
        Player.EnemyHit -= SmallExplosion;
        PlayerProjectile.EnemyHit -= SmallExplosion;
        PlayerProjectile.BossEnemyHit -= SmallExplosion;
        GameManager.BossOnFire -= BossOnFireExplosions;
        GameManager.BossDied -= BigExplosion;
    }
}
