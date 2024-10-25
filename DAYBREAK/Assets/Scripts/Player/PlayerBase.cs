using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerBase : MonoBehaviour
{
    Rigidbody _rb;
    PlayerIA _playerInputActions;
    PlayerUI _playerUI;

    Vector2 aimPosition = Vector2.zero;
    Vector2 movePosition = Vector2.zero;

    [SerializeField] int maxHealth = 10;
    int curHealth;
    int shield;

    bool canTakeDamage = true;
    float invincibilityTime;

    [Tooltip("The player's movement speed")]
    [SerializeField] float speed = 2.5f;


    //UpgradeModifiers
    //[HideInInspector] 
    public int maxHealthModifier = 0;
    public int shieldModifier = 0;
    public float invincibilityTimeModifier = 0.25f;
    //[HideInInspector] 
    public float speedModifier = 0;



    public int CurHealth { get => curHealth; set => curHealth = value; }
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    
    private UIManager _uiManager;

    // Start is called before the first frame update
    void Start()
    {
        _playerInputActions = new PlayerIA();
        _playerInputActions.Enable();
        _rb = GetComponent<Rigidbody>();

        _playerUI = GetComponent<PlayerUI>();

        _playerInputActions.Game.Move.performed += ctx => movePosition = ctx.ReadValue<Vector2>();
        _playerInputActions.Game.Move.canceled += ctx => movePosition = new Vector2(0,0) ;

        //stat initialization 
        curHealth = maxHealth;

        // FIX THE PLAYER'S PIVOT ORIGIN TO THEIR FEET -_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_
        transform.position = FindObjectOfType<TerrainGenerator>().GetNearestSpawnPos(Vector3.zero);

        _uiManager = (UIManager)FindObjectOfType(typeof(UIManager));
    }


    private void OnDisable()
    {
        _playerInputActions.Disable();
    }

    private void FixedUpdate()
    {

        //Vector3 movement = new Vector3(movePosition.x, 0f, movePosition.y).normalized * speed; //normal movement (non iso)

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f, LayerMask.GetMask("Terrain")))
        {
            /*

             If the normal is too steap we can add a sliding mechanic that will push the player down slopes
                        if (hit.normal.y < 0.7f)
                        {
                            Vector3 slide = new Vector3(hit.normal.x, 0, hit.normal.z);
                            _rb.velocity = slide * speed;
                            return;
                        }

             */

            // Get the direction, disregard the y axis
            Vector3 direction = new Vector3(movePosition.x - movePosition.y, 0f, movePosition.x + movePosition.y).normalized; //iso movement 

            // Get the right and forward vectors by calculating the cross product of the normal of the ground and the direction
            Vector3 right = Vector3.Cross(hit.normal, direction);
            Vector3 forward = Vector3.Cross(right, hit.normal);

            // Move the player with relation to the ground
            _rb.velocity = forward * speed;
        }
        else
        {
            // If the player is not on the ground, move it towards the player by adding a force
            Vector3 movement = new Vector3(movePosition.x - movePosition.y, 0f, movePosition.x + movePosition.y).normalized * speed; //iso movement 
            _rb.AddForce(movement);
        }
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
        curHealth -= damage;
        if (curHealth <= 0)
        {
            Die();
        }
        
        _playerUI.UpdateHealthBar();
        canTakeDamage = false;
        StartCoroutine(InvincibilityFrames());
    }

    void Die()
    {
        _uiManager.DisplayWinLoss(true);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UpdateMaxHealth(int health, bool increaseCurHealth)
    {
        maxHealthModifier += health;

        if (increaseCurHealth)
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage((int)collision.gameObject.GetComponent<EnemyBase>().GetDamage());
        }
    }

    IEnumerator InvincibilityFrames()
    {
        yield return new WaitForSeconds(invincibilityTime);
        canTakeDamage = true;

    }
}