using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBossProjectilePool : MonoBehaviour
{ 
    public static FirstBossProjectilePool sharedInstance;
    private List<GameObject> projectilePool;
    [SerializeField] private GameObject projectile;
    [SerializeField] private int poolSize;

    private void Awake()
    {
        if (!sharedInstance) { sharedInstance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        projectilePool = new List<GameObject>();
        GameObject createdObject;

        for (int i = 0; i < poolSize; ++i)
        {
            createdObject = Instantiate(projectile);
            createdObject.SetActive(false);
            projectilePool.Add(createdObject);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < poolSize; ++i)
        {
            if (!projectilePool[i].activeInHierarchy)
            {
                return projectilePool[i];
            }
        }
        return null;
    }
}
