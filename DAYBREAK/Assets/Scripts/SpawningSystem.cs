using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.Simple_Scripts;

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
    
    [SerializeField] TerrainGenerator terrainGenerator;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = FindObjectOfType<PlayerBase>().gameObject.transform;
        if (!terrainGenerator) terrainGenerator = FindObjectOfType<TerrainGenerator>();
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
        Transform pTransform = FindObjectOfType<PlayerBase>().gameObject.transform;
        GameObject sEnemy = spawnGroups[index].enemies[Random.Range(0, spawnGroups[index].enemies.Count)];

        // Randomly select a direction around the player
        Vector3 randomDirection = Random.onUnitSphere;
        randomDirection.y = 0; // Keep the enemy on the same plane as the player
        randomDirection.Normalize();

        if (randomDirection.x == 0)
        {
            randomDirection.x = 1;
        }

        if (randomDirection.y == 0)
        {
            randomDirection.y = 1;
        }

        // Calculate the spawn position based on the player's position and the chosen direction
        //Vector3 spawnPosition = terrainGenerator ? terrainGenerator.GetNearestSpawnPos(pTransform.position + randomDirection * spawnDistance) : pTransform.position + randomDirection * spawnDistance;

        Vector3 spawnPosition = pTransform.position + (randomDirection * spawnDistance);

        // Spawn the enemy at the calculated position with no rotation
        //Instantiate(sEnemy, spawnPosition, Quaternion.identity);
        SsObjectPool.GetObject(sEnemy.name, sEnemy, spawnPosition, Quaternion.identity);

    }
}
