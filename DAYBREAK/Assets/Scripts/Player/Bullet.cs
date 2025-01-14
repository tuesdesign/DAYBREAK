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

    [Space]
    [SerializeField] bool enableTerrainGlueForce;
    [SerializeField] float glueDistance;
    [SerializeField] float gluePower;

    Vector3 glueForce;
    Vector3 glueCast;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bulletHandling = FindObjectOfType<BulletApplicationHandling>();
    }

    void FixedUpdate()
    {
        if (enableTerrainGlueForce) GlueBulletToTerrain();
    }

    void CheckShootStatusAffliction()
    {

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
