using System.Collections;
using System.Collections.Generic;
using Audio;
using UI.Scripts;
using UI.Scripts.PauseMenu;
using UnityEngine;
using Utility.Simple_Scripts;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] public EnemySO enemySO;
    Rigidbody _rb;
    Transform playerPosition;

    float curHealth;
    
    int curdamage=2;
    float curspeed;
    float speedMod = 1;
    Vector3 movePos = Vector2.zero;

    [SerializeField] private GameObject floatingDamageNum;

    //enemy effects
    bool takeTickDamage;
    [SerializeField] float timeBetweenTicks = 1f;
    float tdTimer = 0;
    int curTickDamage = 1;
    float tickDamageDuration = 5;
    float freezeTime = 3.5f;
    float slowTime = 5f;

    public bool onFire;
    
    public GameObject fireVFX;
    public GameObject freezeVFX;
    public GameObject slowVFX;
    public GameObject poisonVFX;

    public List<Coroutine> coroutines = new List<Coroutine>();
    
    
    Transform playerTrans;

    //public float GetDamage { get => damage; set => damage = value; }

    
    private PauseMenuManager _pauseManager;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        playerTrans = FindObjectOfType<PlayerBase>().gameObject.transform;

        curHealth = enemySO.maxHealth;
        curspeed = enemySO.speed;
        curdamage = enemySO.damage;
        
        _pauseManager = FindObjectOfType(typeof(PauseMenuManager)) as PauseMenuManager;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerTrans != null)
        {
            MoveTowardsPlayer();
        }

        if (takeTickDamage)
        {
            tdTimer += Time.deltaTime;
            if (tdTimer >= timeBetweenTicks)
            {
                TakeDamage(curTickDamage);
                tdTimer -= timeBetweenTicks;
            }
        }
    }

    void MoveTowardsPlayer()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f, LayerMask.GetMask("Terrain")))
        {
            // Get the direction to the player, disregard the y axis
            Vector3 direction = movePos = new Vector3(playerTrans.position.x - transform.position.x, 0, playerTrans.position.z - transform.position.z).normalized;

            // Get the right and forward vectors by calculating the cross product of the normal of the ground and the direction
            Vector3 right = Vector3.Cross(hit.normal, direction);
            Vector3 forward = Vector3.Cross(right, hit.normal);

            // Move the enemy in the direction of the player with relation to the ground
            _rb.velocity = forward * enemySO.speed * speedMod;
        }
        else if (Physics.Raycast(transform.position + Vector3.up * 100, Vector3.down, out hit, 200f, LayerMask.GetMask("Terrain")))
        {
            _rb.position = hit.point + Vector3.up;
        }
    }

    public void TakeDamage(float damage)
    {
        curHealth -= damage;
        
        SoundFXManager.Instance.PlayRandomSoundFXClip(AudioClipManager.Instance.hurtSounds, transform, 1f);
        
        // Spawn UI floating damage numbers
        GameObject dmgText = Instantiate(floatingDamageNum, transform.position, floatingDamageNum.transform.rotation);
        dmgText.transform.GetChild(0).GetComponent<TextMesh>().color = Color.red;
        dmgText.transform.GetChild(0).GetComponent<TextMesh>().text = damage.ToString();
        
        if (curHealth <= 0)
        {
            Die();
        }
    }

    public void TickDamageClaculation(int damage = 2 )
    {

        takeTickDamage = true;
        curTickDamage = damage;

        if (fireVFX)
        {
            fireVFX.SetActive(true);
        }
        StartCoroutine(TickDamageEffectTime());
        
        //need to make this more robust to handle multiple status effects at the same time
        //also need to add vfx and sounds to thie mix

        //have a coroutine dedicated for each status ailment possible and start and stop them based on each one
    }

    public void TriggerFreeze()
    {
        if (freezeVFX)
        {
            freezeVFX.SetActive(true);
        }
        StartCoroutine(FreezeEffect());
    }

    public void TriggerSlow()
    {
        if (slowVFX)
        {
            slowVFX.SetActive(true );
        }
        StartCoroutine(SlowEffect());
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
            else if (enemySO.altExpDrop != null) 
            {
                GameObject exp = Instantiate(enemySO.altExpDrop, this.transform);
                exp.transform.parent = this.transform.parent;
            }
        }
        
        // Add to kill counter
        _pauseManager.killCounter += 1;
        PlayerPrefs.SetInt("EnemiesKilled", PlayerPrefs.GetInt("EnemiesKilled") + 1);
        
        Destroy(this.gameObject);
        //SsObjectPool.PoolObject(enemySO.name, this.gameObject);
    }

    public int GetDamage()
    {
        return curdamage;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(transform.position, transform.position + _rb.velocity);
    }

    private IEnumerator TickDamageEffectTime()
    {
        yield return new WaitForSeconds(tickDamageDuration);
        fireVFX.SetActive(false);

        takeTickDamage = false;
        curTickDamage = 0;

        tdTimer = 0;
    }

    private IEnumerator SlowEffect()
    {
        speedMod = 0.5f;
        yield return new WaitForSeconds(slowTime);
        slowVFX.SetActive(false);
        speedMod = 1;
    }

    private IEnumerator FreezeEffect()
    {
        speedMod = 0;
        yield return new WaitForSeconds(freezeTime);
        freezeVFX.SetActive(false);
        speedMod = 1;
    }
}
