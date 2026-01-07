using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNode : MonoBehaviour
{
    private RoomSpawnManager mySpawner;

    public GameObject enemy;
    private GameObject mySpawn;

    private EnemyBody spawnScript;

    public GameObject spawnParticles;

    private bool activated;

    public int myWave = 1;

    void OnEnable()
    {
        mySpawner.SpawnWave += SpawnMyEnemy;
    }
    void OnDisable()
    {
        mySpawner.SpawnWave -= SpawnMyEnemy;
    }

    void Awake()
    {
        mySpawner = GetComponentInParent<RoomSpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnMyEnemy()
    {
        if (!activated && mySpawner.currentWave == myWave)
        {
            Instantiate(spawnParticles, transform.position, Quaternion.identity);
            
            activated = true;
            mySpawn = Instantiate(enemy, transform.position, Quaternion.identity, transform);
            spawnScript = mySpawn.GetComponent<EnemyBody>();
            spawnScript.OnDeath += OnSpawnDeath;
            mySpawner.activeEnemies.Add(mySpawn);
        }
    }

    void OnSpawnDeath(GameObject a, float b, Vector2 c)
    {
        mySpawner.activeEnemies.Remove(mySpawn);
    }
}
