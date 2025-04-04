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

    public AudioClip burnSFX;
    public AudioClip freezeSFX;
    public AudioClip slowSFX;
    public AudioClip poisonSFX;

    //bulletinformation
    bool canBurn;
    bool canFreeze;
    bool canSlow;
    bool canPoision;
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
    public bool CanPoision { get => canPoision; set => canPoision = value; }
    public bool CanSlow { get => canSlow; set => canSlow = value; }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bulletHandling = FindObjectOfType<BulletApplicationHandling>();
        Destroy(this.gameObject, 4);
    }

    void FixedUpdate()
    {
        if (enableTerrainGlueForce) GlueBulletToTerrain();
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
                enemy.TickDamageClaculation(2);
                AudioSource.PlayClipAtPoint(burnSFX,transform.position);
                
            }

            if (canPoision)
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
                enemy.TriggerFreeze();
                AudioSource.PlayClipAtPoint(freezeSFX, transform.position);
            }
            
            if (canSlow)
            {
                enemy.TriggerSlow();

            }

            if (peirceAmount <= 0)
            {
                collision.gameObject.GetComponent<EnemyBase>().TakeDamage(damage);
                Destroy(this.gameObject);
            }
            else
            {
                peirceAmount--;
            }
        }
        
        if (collision.gameObject.tag == "Destructible")
        {
            collision.gameObject.GetComponent<DamagableObjects>().TakeDamage(damage);
        }
        
        // Destroys bullets if they hit a wall
        if (collision.contacts[0].normal.y < 0.5f)
            Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rb.position, rb.position + glueForce);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(rb.position, glueCast);
    }
}


