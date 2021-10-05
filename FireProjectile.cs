using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{
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
            GameManager.sharedInstance.EnemyHit(other);
            gameObject.SetActive(false);
        }
    }   
}
