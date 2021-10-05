using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private AudioClip playerShoot;
    [SerializeField] private GameObject ProjectilePrefab;
    
    private AudioSource FxAudioSource;

    private readonly float playerSpeed = 50;
    private readonly float bounds = 25;

    // Start is called before the first frame update
    void Start()
    {
        FxAudioSource = GetComponent<AudioSource>();
        FxAudioSource.clip = playerShoot;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        if (Input.GetKey(KeyCode.Space) && GameManager.isPowerUp)
        {
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }
    void MovePlayer()
    {
        float forwardInput = Input.GetAxis("Vertical");
        float sidewaysInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.forward * playerSpeed * Time.deltaTime * forwardInput);
        transform.Translate(Vector3.right * playerSpeed * Time.deltaTime * sidewaysInput);
        if (transform.position.z < 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
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
    }
    void Shoot()
    {
        FxAudioSource.PlayOneShot(playerShoot, 2.0f);
        GameObject projectile = PlayerProjectilePool.sharedInstance.GetPooledObject();
        projectile.transform.position = transform.position;
        projectile.SetActive(true);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PowerUp"))
        {
            GameManager.sharedInstance.PowerUp(other);
        }
        
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("BossEnemy"))
        {
            GameManager.sharedInstance.PlayerHit();
            GameManager.sharedInstance.EnemyHit(other);
        }       
    }
   
    
    
}
