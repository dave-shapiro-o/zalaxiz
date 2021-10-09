using System.Collections;
using UnityEngine;

public class FirstLevelBoss : MonoBehaviour
{ 
    private readonly float enemySpeed = 0.02F;
    private readonly float bounds = 25;

    private Rigidbody enemyBody;
    private new AudioManager audio;

    // Start is called before the first frame update
    void Start()
    {
        audio = AudioManager.sharedInstance;
        enemyBody = gameObject.GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        while (!GameManager.isGameOver)
        {
            int i = 0;
            while (i++ < 8)
            {
                yield return new WaitForSeconds(Random.Range(0, 1F));
                Fire();
            }
            yield return new WaitForSeconds(Random.Range(0, 1F));
            i = 0;
            while (i++ < 100)
            {
                yield return new WaitForSeconds(0.02F);
                Fire();
            }
            yield return new WaitForSeconds(2);
        }
    }

    private void Fire()
    {
        if (gameObject.activeInHierarchy)
        {
            audio.PlayFX("Boss Enemy Shoot");
            Quaternion shootRotation = transform.rotation * Quaternion.Euler(0, 180f, 0);
            BossEnemyProjectile.Fire(transform.position, shootRotation);
        }      
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Move Player
        if (GameManager.isPlayerAlive)
        {                 
            Quaternion lookRotation = Quaternion.LookRotation(GameManager.player.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime);

            if (transform.position.z < 7)
            {
                enemyBody.AddForce(new Vector3(0, 0, 20) * enemySpeed, ForceMode.Impulse);
            }
            if (transform.position.z > 22)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, 22);
            }
            if (transform.position.x < -bounds + 5)
            {
                transform.position = new Vector3(-bounds + 5, transform.position.y, transform.position.z);
            }
            if (transform.position.x > bounds - 5)
            {
                transform.position = new Vector3(bounds - 5, transform.position.y, transform.position.z);
            }
                enemyBody.AddForce(3 * enemySpeed * (GameManager.player.transform.position - transform.position), ForceMode.Force);            
        }
        // Retreat Player
        else
        {
            enemyBody.AddForce(new Vector3(0, 0, 30) * enemySpeed, ForceMode.Impulse);
            if (transform.position.z < -10
                || transform.position.z > bounds + 10
                || transform.position.x < -bounds - 10
                || transform.position.x > bounds + 10)
            {
                gameObject.SetActive(false);
            }
        }
    }

}
