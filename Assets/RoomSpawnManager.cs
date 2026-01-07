using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomSpawnManager : MonoBehaviour
{
    public event Action SpawnWave;

    private bool isActive = false;
    private bool isCleared = false;

    public int numWaves;
    public int currentWave;

    private List<List<Transform>> waves;
    public List<Transform> nodes;

    public List<GameObject> activeEnemies;

    // Start is called before the first frame update
    void Awake()
    {
        foreach(Transform child in transform)
        {
            if (child.CompareTag("Node"))
            {
                nodes.Add(child);

                int childWave = child.GetComponent<EnemyNode>().myWave;
                if (childWave > numWaves)
                    numWaves = childWave;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWave < numWaves && activeEnemies.Count == 0 && isActive)
        {
            NewWave();
            SpawnWave?.Invoke();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isCleared)
        {
            isActive = true;

            if (collider.CompareTag("Player"))
            {
                SpawnWave?.Invoke();
            }
        }
    }

    void NewWave()
    {
        currentWave += 1;
    }
}
