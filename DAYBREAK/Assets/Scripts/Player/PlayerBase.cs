using System.Collections;
using System.Collections.Generic;
using UI.Scripts;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    Rigidbody _rb;
    PlayerIA _playerInputActions;
    PlayerUI _playerUI;
    PlayerShooting _playerShooting;
    CharacterAnimController _playerAnimController;

    Vector2 movePosition = Vector2.zero;

    [SerializeField] int maxHealth = 10;
    int curHealth;
    public int shield;
    bool hasSheild;
    [SerializeField] public float dodgeChange;

    bool canTakeDamage = true;
    bool tickDamageActive = false;
    float invincibilityTime = 0.2f;

    [Tooltip("The player's movement speed")]
    [SerializeField] float speed = 2.5f;


    [Header("VFX")]
    [SerializeField] GameObject dodgeEffect;
    [SerializeField] GameObject sheildEffect;

    //UpgradeModifiers
    [HideInInspector] 
    public int maxHealthModifier = 0;
    [HideInInspector]
    public float invincibilityTimeModifier = 0;
    [HideInInspector] 
    public float speedModifier = 0;

    bool isTakeingWaterDamage = false;

    float waterLevel = 0;

    public int CurHealth { get => curHealth; set => curHealth = value; }
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    
    private UIManager _uiManager;

    // Start is called before the first frame update
    void Awake()
    {
        _playerInputActions = new PlayerIA();
        _playerInputActions.Enable();
        _rb = GetComponent<Rigidbody>();
        _playerAnimController = GetComponentInChildren<CharacterAnimController>();

        _playerUI = GetComponent<PlayerUI>();
        _playerShooting = GetComponent<PlayerShooting>();

        _playerInputActions.Game.Move.performed += ctx => movePosition = ctx.ReadValue<Vector2>();
        _playerInputActions.Game.Move.canceled += ctx => movePosition = new Vector2(0,0) ;

        //stat initialization 
        curHealth = maxHealth;

        // FIX THE PLAYER'S PIVOT ORIGIN TO THEIR FEET -_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_
        transform.position = FindObjectOfType<TerrainGenerator>().GetNearestSpawnPos(Vector3.zero);

        _uiManager = (UIManager)FindObjectOfType(typeof(UIManager));

        waterLevel = FindObjectOfType<TerrainGenerator>().terrainDataObject.waterLevel;
    }


    private void OnDisable()
    {
        _playerInputActions.Disable();
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 2f, LayerMask.GetMask("Terrain")))
        {
            // Get the direction, disregard the y axis
            Vector3 direction = new Vector3(movePosition.x - movePosition.y, 0f, movePosition.x + movePosition.y).normalized; //iso movement 

            // Get the right and forward vectors by calculating the cross product of the normal of the ground and the direction
            Vector3 right = Vector3.Cross(hit.normal, direction);
            Vector3 forward = Vector3.Cross(right, hit.normal);

            // Move the player with relation to the ground
            _rb.velocity = forward * (speed + speedModifier);
        }
        else if (Physics.Raycast(transform.position + Vector3.up * 100, Vector3.down, out hit, 200f, LayerMask.GetMask("Terrain")))
        {
            _rb.position = hit.point + Vector3.up;
        }

        //if the player is moving, play the moving animation
        _playerAnimController.isMoving = movePosition != Vector2.zero;
        _playerAnimController.moveDirection = movePosition;

        if (transform.position.y < waterLevel && !isTakeingWaterDamage) StartCoroutine(WaterKillTimer());
        if (transform.position.y > waterLevel) isTakeingWaterDamage = false;
    }

    public void ApplyCharacterStats(PlayerSO playerStats)
    {
        maxHealth = playerStats.maxHealth;
        speed = playerStats.speed;

        //sets player shooting stats
        _playerShooting.MaxAmmo = playerStats.maxammo;
        _playerShooting.BulletType = playerStats.bulletType;
        _playerShooting.BulletSpeed = playerStats.bulletspeed;
        _playerShooting.ShootDelay = playerStats.shootDelay;
    }

    #region Health Related Code (Heal, Damage, Death)
    public void Heal(int amount)
    {
        curHealth += amount;
        if(curHealth > maxHealth + maxHealthModifier)
        {
            curHealth = maxHealth + maxHealthModifier;
        }
        _playerUI.UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        if (canTakeDamage) //if the player can be damaged currently
        {
            if (dodgeChange >= Random.Range(0, 100)) { //if player's dodge chance triggers  (out of 100%)
                
                canTakeDamage = false;
                StartCoroutine(InvincibilityFrames());  //player dodges for length of invincibility frames. 
            }
            else if (shield > 0) //checks if the player has any sheild
            {
                shield -= damage;
                if (shield < 0) //if the player takes more damage than they have sheild
                {
                    curHealth += shield;
                    shield = 0;
                }
                ToggleSheild();
            }
            else
            {
                curHealth -= damage;
            }

            if (curHealth <= 0)
            {
                Die();
            }

            _playerUI.UpdateHealthBar();
            canTakeDamage = false;
            StartCoroutine(InvincibilityFrames());
        }
    }

    public void TakeDamage(int damage, GameObject enemy)
    {
        if (canTakeDamage) //if the player can be damaged currently
        {
            Vector3 hitDirection = transform.position - enemy.transform.position;
            hitDirection.y = 0;
            hitDirection.Normalize();

            if (dodgeChange >= Random.Range(0, 100))
            { //if player's dodge chance triggers  (out of 100%)
                dodgeEffect.gameObject.SetActive(true);
                _rb.AddForce(hitDirection * 100, ForceMode.Impulse);
                canTakeDamage = false;
                StartCoroutine(InvincibilityFrames());  //player dodges for length of invincibility frames. 
            }
            else if (shield > 0) //checks if the player has any sheild
            {
                shield -= damage;
                if (shield < 0) //if the player takes more damage than they have sheild
                {
                    curHealth += shield;
                    shield = 0;
                }
                ToggleSheild();
            }
            else
            {
                curHealth -= damage;
            }

            if (curHealth <= 0)
            {
                Die();
            }

            _playerUI.UpdateHealthBar();
            canTakeDamage = false;
            StartCoroutine(InvincibilityFrames());
        }
    }

    void Die()
    {
        _uiManager.DisplayWinLoss(true);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UpdateMaxHealth(int health, bool increaseCurHealth)
    {
        maxHealthModifier += health;

        if (increaseCurHealth) //this variable is for if the health upgrade should also heal the player of tthat amount
        {
            Heal(health);
        }
        _playerUI.UpdateHealthBar();
    }
    #endregion

    void PlaySoundEffect(List<AudioClip> soundList)
    {
        if (soundList != null)
        {
            AudioSource.PlayClipAtPoint(soundList[Random.Range(0, soundList.Count)], this.transform.position);
        }
    }

    void DodgeEffect()
    {
        dodgeEffect.SetActive(true);
        StartCoroutine(DodgeEffectDelay());
    }

    public void ToggleSheild()
    {
        if (sheildEffect != null)
        {
            if (shield > 0)
            {
                sheildEffect.SetActive(true);
            }
            else
            {
                sheildEffect.SetActive(false);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage((int)collision.gameObject.GetComponent<EnemyBase>().GetDamage(), collision.gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !tickDamageActive)
        {
            StartCoroutine(TickDamage((int)collision.gameObject.GetComponent<EnemyBase>().GetDamage(),collision.gameObject));
            tickDamageActive = true;
        }
    }

    IEnumerator TickDamage(int damage)
    {
        TakeDamage(damage);
        yield return new WaitForSeconds(.75f);
        
        tickDamageActive = false;
    }
    IEnumerator TickDamage(int damage, GameObject enemy)
    {
        TakeDamage(damage, enemy.gameObject);
        yield return new WaitForSeconds(.75f);
        
        tickDamageActive = false;
    }

    IEnumerator InvincibilityFrames() //this coroutine runs for the amount of seconds that the player should be invincible for and then allows them to take damage
    {
        yield return new WaitForSeconds(invincibilityTime);
        canTakeDamage = true;
        dodgeEffect.gameObject.SetActive(false);
    }

    IEnumerator DodgeEffectDelay()
    {
        yield return new WaitForSeconds(1f);
        dodgeEffect.SetActive(false);
    }

    IEnumerator WaterKillTimer() 
    {
        isTakeingWaterDamage = true;
        while (isTakeingWaterDamage)
        {
            yield return new WaitForSeconds(1f);
            if (isTakeingWaterDamage) TakeDamage(1);
            else break;
        }
    }
}