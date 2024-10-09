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

<<<<<<< Updated upstream
=======
    //enemy effects
    
    Rigidbody _rb;
    
    Transform playerTrans;
    Vector3 movePos = Vector2.zero;

    public float GetDamage { get => damage; set => damage = value; }
>>>>>>> Stashed changes

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
<<<<<<< Updated upstream
        playerPosition = FindObjectOfType<PlayerBase>().gameObject.transform;

        curHealth = enemySO.maxHealth;
        curspeed = enemySO.speed;
        curdamage = enemySO.damage;

=======
        playerTrans = FindObjectOfType<PlayerBase>().gameObject.transform;
        curHealth = maxHealth;
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
        if(playerTrans != null)
        {
            MoveTowardsPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        //transform.position = Vector2.MoveTowards(transform.position, playerPosition.position, speed * Time.deltaTime);
        
<<<<<<< Updated upstream
        movePos = (playerPosition.position - transform.position).normalized;
        _rb.velocity = movePos * curspeed;
=======
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
        {
            /*

             If the normal is too steap we can add a sliding mechanic that will push enemies down slopes
                        if (hit.normal.y < 0.7f)
                        {
                            Vector3 slide = new Vector3(hit.normal.x, 0, hit.normal.z);
                            _rb.velocity = slide * speed;
                            return;
                        }

             */

            // Get the direction to the player, disregard the y axis
            Vector3 direction = movePos = new Vector3(playerTrans.position.x - transform.position.x, 0, playerTrans.position.z - transform.position.z).normalized;

            // Get the right and forward vectors by calculating the cross product of the normal of the ground and the direction
            Vector3 right = Vector3.Cross(hit.normal, direction);
            Vector3 forward = Vector3.Cross(right, hit.normal);

            // Move the enemy in the direction of the player with relation to the ground
            _rb.velocity = forward * speed;
        }
        else
        {
            // If the enemy is not on the ground, move it towards the player by adding a force
            movePos = (playerTrans.position - transform.position).normalized;
            _rb.AddForce(movePos * speed);
        }
>>>>>>> Stashed changes
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + _rb.velocity);
    }
}
