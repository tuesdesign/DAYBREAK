using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningSystem : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnGroup
    {
        public float duration;
        public float spawnSpeed;
        public List<GameObject> enemies;
    }

    int index=0;

    [Tooltip("List of different stages of enemy spawns you want in game")]
    [SerializeField] List<SpawnGroup> spawnGroups;

    [Tooltip("Distance away from player")]
    [SerializeField] private float spawnDistance = 30;

    float spawnTimer = 0;
    float stageTimer = 0;
    Transform playerTransform;
    

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = FindObjectOfType<PlayerBase>().gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;
        stageTimer += Time.deltaTime;
        if (stageTimer >= spawnGroups[index].duration && index < spawnGroups.Count-1)
        {
            stageTimer = 0;
            index++;
            
        }

        if (spawnTimer >= spawnGroups[index].spawnSpeed) {
            Spawn();
            spawnTimer -= spawnGroups[index].spawnSpeed; 
        }
        
    }

    void Spawn()
    {        
        GameObject sEnemy = spawnGroups[index].enemies[Random.Range(0, spawnGroups[index].enemies.Count)];

        // Randomly select a direction around the player
        Vector3 randomDirection = Random.insideUnitSphere;
        randomDirection.y = 0; // Keep the enemy on the same plane as the player

        // Calculate the spawn position based on the player's position and the chosen direction
        Vector3 spawnPosition = playerTransform.position + randomDirection * spawnDistance;
        
        // Spawn the enemy at the calculated position with no rotation
        Instantiate(sEnemy, spawnPosition, Quaternion.identity);

    }
}
