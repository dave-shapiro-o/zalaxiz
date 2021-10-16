using System.Collections;
using UnityEngine;

public class GreyFighterEnemy : MonoBehaviour
{
    private readonly float enemySpeed = 0.01F;
    private readonly float bounds = 25;

    private Rigidbody enemyBody;
    private new AudioManager audio;     
    private float currentX;

    // Start is called before the first frame update
    void Start()
    {
        enemyBody = gameObject.GetComponent<Rigidbody>();
        audio = AudioManager.sharedInstance;
    }

    private void OnEnable()
    {
        StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        while (!GameManager.isGameOver)
        {
            yield return new WaitForSeconds(Random.Range(0, 6));
            if (GameManager.isPlayerAlive)
            {
                audio.PlayFX("Enemy Shoot");
                EnemyProjectile.Fire(transform.position);
            }                    
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Enemy Attack
        if (GameManager.isPlayerAlive && !GameManager.isBossFight)
        {
            currentX = transform.position.x;
            enemyBody.AddForce((GameManager.player.transform.position - transform.position) * enemySpeed, ForceMode.Impulse);
            if (transform.position.z < 1)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, 1);
                enemyBody.AddForce(new Vector3(0, 0, 35) * enemySpeed, ForceMode.Impulse);
            }
            if (transform.position.z > bounds)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, bounds);
            }
            if (transform.position.x < -bounds)
            {
                transform.position = new Vector3(-bounds, transform.position.y, transform.position.z);
            }
            if (transform.position.x > bounds)
            {
                transform.position = new Vector3(bounds, transform.position.y, transform.position.z);
            }
            if (transform.position.x > currentX) { transform.Rotate(0.0f, 0.0f, 0.2F, Space.World); }
            if (transform.position.x < currentX) { transform.Rotate(0.0f, 0.0f, -0.2F, Space.World); }
        }
        // Enemy Retreat
        else
        {
            enemyBody.AddForce(new Vector3(0, 0, 25) * enemySpeed, ForceMode.Impulse);
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
