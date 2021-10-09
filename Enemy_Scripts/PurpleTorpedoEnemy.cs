using System.Collections;
using UnityEngine;

public class PurpleTorpedoEnemy : MonoBehaviour 
{
    private readonly float enemySpeed = 0.01F;
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
            yield return new WaitForSeconds(Random.Range(0, 4));
            audio.PlayFX("Torpedo Enemy Shoot");
            TorpedoEnemyProjectile.Fire(transform.position);
        }

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        // Enemy Attack
        if (GameManager.isPlayerAlive && !GameManager.isBossFight)
        {
            transform.Rotate(0.0f, 0.0f, 4.0f, Space.World);
            if (transform.position.z < 7)
            {
                enemyBody.AddForce(new Vector3(0, 0, 30) * enemySpeed, ForceMode.Impulse);
            }
            if (transform.position.x < -bounds)
            {
                transform.position = new Vector3(-bounds, transform.position.y, transform.position.z);
            }
            if (transform.position.x > bounds)
            {
                transform.position = new Vector3(bounds, transform.position.y, transform.position.z);
            }
            if (transform.position.z <= 10)
            {
                enemyBody.AddForce(0.5f * enemySpeed * -(GameManager.player.transform.position - transform.position), ForceMode.Impulse);
            }
            else if (transform.position.z > 10)
            {
                enemyBody.AddForce((GameManager.player.transform.position - transform.position) * enemySpeed, ForceMode.Impulse);
            }
        }
        // Enemy Retreat
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
