using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldSpawningSystem : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnGroup
    {
        public float duration;
        public float spawnSpeed;
        public List<GameObject> enemies;
    }
    int index=0;
    [SerializeField] List<SpawnGroup> spawnGroups;
    
    float timer = 0;

    Camera cam;
    Transform playerTransform;
    

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        playerTransform = FindObjectOfType<OldPlayerBase>().gameObject.transform;
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
        if(spawnzone == 0)
        {
            //Instantiate(sEnemy, new Vector3(Random.Range(0, cam.pixelWidth), cam.pixelHeight),this.transform.rotation);
            Instantiate(sEnemy, new Vector3(Random.Range(playerTransform.position.x-10, playerTransform.position.x+10), playerTransform.position.y + 15),this.transform.rotation);
        }
        else if (spawnzone == 1)
        {
            Instantiate(sEnemy, new Vector3(Random.Range(playerTransform.position.x - 10, playerTransform.position.x + 10), playerTransform.position.y - 15), this.transform.rotation);

        }
        else if (spawnzone == 2)
        {
            Instantiate(sEnemy, new Vector3(playerTransform.position.x + 20, Random.Range(playerTransform.position.y - 10, playerTransform.position.y + 10)), this.transform.rotation);
        }
        else if (spawnzone == 3)
        {
            Instantiate(sEnemy, new Vector3(playerTransform.position.x - 20, Random.Range(playerTransform.position.y - 10, playerTransform.position.y + 10)), this.transform.rotation);
        }

    }
}
