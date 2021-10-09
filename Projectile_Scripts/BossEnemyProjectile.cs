using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyProjectile : MonoBehaviour 
{
    private readonly float enemyProjectileSpeed = 40.0f;
    
    // Update is called once per frame
    void Update()
    {
        transform.Translate(enemyProjectileSpeed * Time.deltaTime * Vector3.back);
        if (transform.position.y != 1)
        {
            transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        }
        if (transform.position.z < -10 
            || transform.position.z > 35
            || transform.position.x < -30
            || transform.position.x > 30)
        {
            gameObject.SetActive(false);
        }
    }

    internal static void Fire(Vector3 position, Quaternion shootRotation)
    {
        GameObject projectile = FirstBossProjectilePool.sharedInstance.GetPooledObject();
        projectile.transform.SetPositionAndRotation(position, shootRotation);
        projectile.SetActive(true);
    }
}
