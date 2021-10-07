using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public delegate void HitEventHandler(Collider collider);
    public static event HitEventHandler EnemyHit;

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
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("BossEnemy"))
        {
            EnemyHit?.Invoke(other);
            gameObject.SetActive(false);
        }
    }

    internal static void Fire(Vector3 playerPosition)
    {
        GameObject projectile = PlayerProjectilePool.sharedInstance.GetPooledObject();
        projectile.transform.position = playerPosition;
        projectile.SetActive(true);
    }
}
