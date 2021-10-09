using System;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private readonly float enemyProjectileSpeed = 33.0f;

    // Update is called once per frame
    void Update()
    {        
        transform.Translate(enemyProjectileSpeed * Time.deltaTime * Vector3.back);        
        if (transform.position.z < -10)
        {
            gameObject.SetActive(false);
        }
    }

    internal static void Fire(Vector3 position)
    {
        GameObject projectile = GreyEnemyProjectilePool.sharedInstance.GetPooledObject();
        projectile.transform.position = position;
        projectile.SetActive(true);
    }
}
