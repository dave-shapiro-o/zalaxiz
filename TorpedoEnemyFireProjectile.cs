using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoEnemyFireProjectile : MonoBehaviour
{
    private readonly float enemyProjectileSpeed = 15.0f;
           
    // Update is called once per frame
    void Update()
    {
        transform.Translate(enemyProjectileSpeed * Time.deltaTime * Vector3.back);
        if (transform.position.z < -10)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.sharedInstance.PlayerHit();
            gameObject.SetActive(false);
        }
    }
}
