using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldBullet : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float damage;

    [Header("Properties and Multipliers")]
    [SerializeField] bool canBurn;
    [SerializeField] float burnChance;

    [SerializeField] bool isexplosive;
    [SerializeField] float explosionRange;
    [SerializeField] float explosionDamage;

    [SerializeField] bool canFreeze;
    [SerializeField] float freezeChance;

    [SerializeField] bool canBounce;
    [SerializeField] float bounceAmount;

    [SerializeField] float canPierce;
    [SerializeField] float pierceAmount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<OldEnemyBase>().TakeDamage(damage);
            Destroy(this.gameObject);
        }
    }
}
