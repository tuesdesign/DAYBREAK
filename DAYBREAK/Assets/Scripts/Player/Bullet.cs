 using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Bullet : MonoBehaviour
{
    public Rigidbody rb;
    BulletApplicationHandling bulletHandling;

    [SerializeField] float speed;
    [SerializeField] float damage;
    [SerializeField] GameObject hitEffect;

    [Header ("Properties")]
    public int burnDamage;
    public int explosiveDamage;
    public float explosionRaduis;

    [Space]
    [SerializeField] bool enableTerrainGlueForce;
    [SerializeField] float glueDistance;
    [SerializeField] float gluePower;

    Vector3 glueForce;
    Vector3 glueCast;

    //bulletinformation
    bool canBurn;
    bool canFreeze;
    bool canbounce;
    bool canExplode;
    int explosionDamage = 2;
    int peirceAmount = 0;

    public bool CanBurn { get => canBurn; set => canBurn = value; }
    public bool CanFreeze { get => canFreeze; set => canFreeze = value; }
    public bool Canbounce { get => canbounce; set => canbounce = value; }
    public int PeirceAmount { get => peirceAmount; set => peirceAmount = value; }
    public float Damage { get => damage; set => damage = value; }
    public bool CanExplode { get => canExplode; set => canExplode = value; }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bulletHandling = FindObjectOfType<BulletApplicationHandling>();
    }

    void FixedUpdate()
    {
        if (enableTerrainGlueForce) GlueBulletToTerrain();
    }


    void CheckHitStatusAffliction()
    {

    }

    void GlueBulletToTerrain()
    {
        if (Physics.Raycast(rb.position, Vector3.down, out RaycastHit hit, glueDistance * 3, LayerMask.GetMask("Terrain")))
            rb.AddForce(glueForce = Vector3.up * (glueDistance - Vector3.Distance(rb.position, glueCast = hit.point)) * gluePower, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            /*
            We should generaly 'destroy' the bullet when it hits any object. And only apply damage when it hits an enemy.

            */
            GameObject hVFX = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(hVFX,0.5f);
            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();
            


            //add the connection between enemy status afflictions and the bullet handler
            if (canBurn)
            {
                enemy.TickDamageClaculation(1);
            }

            if (CanExplode)
            {
                //create a trigger component on bullet radius that allows 
                enemy.TakeDamage(explosionDamage);
            }
            
            if (canFreeze)
            {

            }


            if (peirceAmount <= 0)
            {
                
            }
            else
            {
                peirceAmount--;
            }

            collision.gameObject.GetComponent<EnemyBase>().TakeDamage(damage);
            Destroy(this.gameObject);

            /*
            Instead of Destroying the bullet, we should use object pooling.
            What we want to do is create a queue of bullets that are not in use.
            When a bullet is fired, we take the first bullet in the queue and set it to active. Setting its position and velocity.
            When the bullet hits an enemy or is 'destroyed', we set it to inactive and add it to the queue.
            We should only need to Instanciate new bullets if the queue is empty.

            If you need help with this, let me know. I can help you with it.
            - Dan <3

            ~ AAAAAA i think i can do pooling - Alannis
            */
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rb.position, rb.position + glueForce);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(rb.position, glueCast);
    }
}
