using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public delegate void HitEventHandler(Collider collider);
    public static event HitEventHandler EnemyHit;
    public static event HitEventHandler BossEnemyHit; 

    private readonly float ProjectileSpeed = 30.0f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(ProjectileSpeed * Time.deltaTime * Vector3.forward);
        if (transform.position.z > 30)
        {
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyHit?.Invoke(other);
            gameObject.SetActive(false);
        }
        if (other.gameObject.CompareTag("BossEnemy"))
        {
            BossEnemyHit?.Invoke(other);
        }                 
    }

    internal static void Fire(Vector3 playerPosition)
    {
        GameObject projectile = PlayerProjectilePool.sharedInstance.GetPooledObject();
        projectile.transform.position = playerPosition;
        projectile.SetActive(true);
    }
}
