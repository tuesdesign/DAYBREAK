using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyBase : MonoBehaviour
{
    [Tooltip("Enemy movement speed")]
    [SerializeField] float speed = 5f;
    [Tooltip("Health")]
    [SerializeField] float maxHealth = 3f;
    float curHealth;
    [Tooltip("damage")]
    [SerializeField] float damage=2;

    [Tooltip("The type of exp that the enemy drops when kiled")]
    [SerializeField] GameObject expDrop;
    [SerializeField][Range(1f, 100f)] float expDropChance = 100;
    


    Rigidbody _rb;
    
    Transform playerPosition;
    Vector3 movePos = Vector2.zero;
    

    public float GetDamage { get => damage; set => damage = value; }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        playerPosition = FindObjectOfType<PlayerBase>().gameObject.transform;
        curHealth = maxHealth;
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
        _rb.velocity = movePos * speed;
    }

    public void TakeDamage(float damage)
    {
        curHealth -= damage;
        if (curHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (expDrop != null) //if this enemy drops exp on kill
        {
            if (Random.Range(0, 100) <= expDropChance) //random drop if the number is under the drop chance
            {
                GameObject exp = Instantiate(expDrop, this.transform);
                exp.transform.parent = this.transform.parent;
            }
            
        }
        
        Destroy(this.gameObject);
    }
    
}
