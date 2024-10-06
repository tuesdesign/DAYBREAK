using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] public EnemySO enemySO;
    Rigidbody _rb;
    Transform playerPosition;

    float curHealth;
    [Tooltip("damage")]
    [SerializeField] int curdamage=2;
    [SerializeField] float curspeed;
    Vector3 movePos = Vector2.zero;

    [SerializeField] List<AudioClip> hurtSounds = new List<AudioClip>();


    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        playerPosition = FindObjectOfType<PlayerBase>().gameObject.transform;

        curHealth = enemySO.maxHealth;
        curspeed = enemySO.speed;
        curdamage = enemySO.damage;

    }

    // Update is called once per frame
    void Update()
    {
        if(playerPosition != null)
        {
            MoveTowardsPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        //transform.position = Vector2.MoveTowards(transform.position, playerPosition.position, speed * Time.deltaTime);
        
        movePos = (playerPosition.position - transform.position).normalized;
        _rb.velocity = movePos * curspeed;
    }

    public void TakeDamage(float damage)
    {
        curHealth -= damage;
        PlaySoundEffect(hurtSounds);
        if (curHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (enemySO.expDrop != null) //if this enemy drops exp on kill
        {
            if (Random.Range(0, 100) <= enemySO.expDropChance) //random drop if the number is under the drop chance
            {
                GameObject exp = Instantiate(enemySO.expDrop, this.transform);
                exp.transform.parent = this.transform.parent;
            }
            
        }
        
        Destroy(this.gameObject);
    }

    public int GetDamage()
    {
        return curdamage;
    }

    void PlaySoundEffect(List<AudioClip> soundList)
    {
        if (soundList != null)
        {
            AudioSource.PlayClipAtPoint(soundList[Random.Range(0, soundList.Count)], this.transform.position);
        }
    }
}
