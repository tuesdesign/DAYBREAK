using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class OldEnemyBase : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float maxHealth = 3f;
    float curHealth;
    [SerializeField] float damage=2;

    Rigidbody2D _rb;
    

    Transform playerPosition;
    Vector2 movePos = Vector2.zero;

    public float GetDamage { get => damage; set => damage = value; }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        playerPosition = FindObjectOfType<OldPlayerBase>().gameObject.transform;
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
        Destroy(this.gameObject);
    }
    
}
