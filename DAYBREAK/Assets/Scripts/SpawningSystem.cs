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
    [SerializeField] private float spawnDistance = 20;

    [SerializeField] List<SpawnGroup> spawnGroups;
    
    float timer = 0;

    Camera cam;
    Transform playerTransform;
    

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        playerTransform = FindObjectOfType<PlayerBase>().gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnGroups[index].spawnSpeed) {
            Spawn();
            timer = 0;
        }
        
    }

    void Spawn()
    {
        int spawnzone = Random.Range(0, 4);
        
        GameObject sEnemy = spawnGroups[index].enemies[Random.Range(0, spawnGroups[index].enemies.Count)];

        // Randomly select a direction around the player
        Vector3 randomDirection = Random.insideUnitSphere.normalized;
        randomDirection.y = 0; // Keep the enemy on the same plane as the player

        // Calculate the spawn position based on the player's position and the chosen direction
        Vector3 spawnPosition = playerTransform.position + randomDirection * spawnDistance;

        // Spawn the enemy at the calculated position with no rotation
        Instantiate(sEnemy, spawnPosition, Quaternion.identity);

    }
}
