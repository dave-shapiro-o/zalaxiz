using System.Collections;
using UnityEngine;

public class PurpleTorpedoEnemy : MonoBehaviour 
{
    private readonly float enemySpeed = 0.01F;
    private readonly float bounds = 25;

    [SerializeField] private GameObject enemyProjectilePrefab;
    [SerializeField] private AudioClip enemyShoot;
          
    private Rigidbody enemyBody;
    private Rigidbody enemyProjectileBody;
    private AudioSource FxAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        enemyBody = gameObject.GetComponent<Rigidbody>();
        FxAudioSource = GetComponent<AudioSource>();
        enemyProjectileBody = enemyProjectilePrefab.GetComponent<Rigidbody>();
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
            FxAudioSource.PlayOneShot(enemyShoot, 2.0f);
            Instantiate(enemyProjectilePrefab, transform.position, enemyProjectileBody.transform.rotation);        
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
