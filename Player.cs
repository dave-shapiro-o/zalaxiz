using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public delegate void HitEventHandler(Collider collider);
    public delegate void PowerUpEventHandler();

    public static event HitEventHandler PlayerHit;
    public static event HitEventHandler EnemyHit;
    public static event HitEventHandler BossEnemyHit;
    public static event PowerUpEventHandler PoweredUp;
  
    private readonly float playerSpeed = 75;
    private readonly float bounds = 25;

    private new AudioManager audio;
    public static Player sharedInstance;

    private void Awake()
    {
        if (!sharedInstance) { sharedInstance = this; }
        else { Destroy(gameObject); }
    }
    // Start is called before the first frame update
    void Start()
    {
        audio = AudioManager.sharedInstance;
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
        transform.Translate(forwardInput * playerSpeed * Time.deltaTime * Vector3.forward);
        transform.Translate(playerSpeed * sidewaysInput * Time.deltaTime * Vector3.right);
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
        audio.PlayFX("Player Shoot");
        PlayerProjectile.Fire(transform.position);
    }
    
    private void OnTriggerEnter(Collider other)
    {   
        if (other.gameObject.CompareTag("Enemy")) 
        {
            PlayerHit?.Invoke(GetComponent<Collider>());
            EnemyHit?.Invoke(other);
        }   
        if (other.gameObject.CompareTag("BossEnemy"))
        {
            BossEnemyHit?.Invoke(other);
        }
        if (other.gameObject.CompareTag("EnemyProjectile"))
        {
            PlayerHit?.Invoke(GetComponent<Collider>());
        }
        if (other.gameObject.CompareTag("PowerUp"))
        {
            PoweredUp?.Invoke();
        }
    }   
}
