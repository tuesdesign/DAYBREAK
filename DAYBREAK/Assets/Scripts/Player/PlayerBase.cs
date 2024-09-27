using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerBase : MonoBehaviour
{
    Rigidbody _rb;
    PlayerIA _playerInputActions;

    Vector2 aimPosition = Vector2.zero;
    Vector2 movePosition = Vector2.zero;

    //Player 
    [Header("Player Base")]
    [SerializeField] float maxHealth = 10;
    float curHealth;
    [Tooltip("The player's movement speed")]
    [SerializeField] float speed = 2.5f;
    [Tooltip("Where the bullets should spawn from the player")]
    [SerializeField] Transform shootPosition;
    [Tooltip("should you use twinstick controls \n if on it uses left and right analog sticks \n if off it only uses the move direction")]
    [SerializeField] bool twinStick = true;

    //EXP
    [Header("EXP System")]
    int exp = 0;
    [SerializeField] int level = 1;
    [Tooltip("This determines how much it takes to level up initially")]
    [SerializeField] int levelIncrement = 100;
    [Tooltip("NOT IMPLEMENTED \n Rate of increase of exp needed for each level")]
    [SerializeField] AnimationCurve incrementRate; //does nothing for now

    //Projectile
    [Header("Projectile Stats")]
    [Tooltip("The time in seconds betweenshots")]
    [SerializeField] float shootDelay = 0.5f;
    [Tooltip("the prefab used for the bullet")]
    [SerializeField] GameObject bulletType;
    [Tooltip("The speed that bullets are fired")]
    [SerializeField] float bulletSpeed = 10f;
    [SerializeField] int maxAmmo = 6;
    int ammoCount;

    bool hasAmmo = true;
    bool isReloading = false;
    bool canShoot = true;
    [SerializeField] float reloadTime = 1f;

    //UI
    [Header("UI")]
    [Tooltip("This is the health bar.")]
    [SerializeField] Slider healthBar;
    [Tooltip("This is the exp bar.")]
    [SerializeField] Slider expBar;
    [Tooltip("The text bar to describe how much ammo the player has in comparisson to their maximum ammo")]
    [SerializeField] TMP_Text ammoTextBar;

    //sound effects
    [Header("Sound Effects")]
    [SerializeField] List<AudioClip> shootSounds = new List<AudioClip>();
    [SerializeField] List<AudioClip> reloadSounds = new List<AudioClip>();


    // Start is called before the first frame update
    void Start()
    {
        _playerInputActions = new PlayerIA();
        _playerInputActions.Enable();
        _rb = GetComponent<Rigidbody>();

        _playerInputActions.Game.Move.performed += ctx => movePosition = ctx.ReadValue<Vector2>();
        _playerInputActions.Game.Move.canceled += ctx => movePosition = new Vector2(0,0) ;

        if (twinStick)
        {
            _playerInputActions.Game.Fire.performed += ctx => aimPosition = ctx.ReadValue<Vector2>();
            _playerInputActions.Game.Fire.performed += ctx => Shoot();
            _playerInputActions.Game.Fire.canceled += ctx => aimPosition = new Vector2(0, 0);
        }
        else
        {
            _playerInputActions.Game.Move.performed += ctx => Shoot();
        }
        


        //stat initialization 
        curHealth = maxHealth;
        ammoCount = maxAmmo;

        //UI initialization
        if(healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = curHealth;
        }

        if(ammoTextBar != null)
        {
            ammoTextBar.text = ammoCount + " / " + maxAmmo;
        }

        if (expBar != null)
        {
            expBar.maxValue = levelIncrement;
            expBar.value = exp;
        }
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        _playerInputActions.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Health Related Code (Heal, Damage, Death)
    public void Heal(float amount)
    {
        curHealth += amount;
        if(curHealth > maxHealth)
        {
            curHealth = maxHealth;
        }
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        curHealth -= damage;
        if (curHealth <= 0)
        {
            Die();
        }
        UpdateHealthBar();
    }

    void Die() //empty for now
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion 

    private void FixedUpdate()
    {

        //Vector3 movement = new Vector3(movePosition.x, 0f, movePosition.y).normalized * speed; //normal movement (non iso)
        Vector3 movement = new Vector3(movePosition.x - movePosition.y, 0f, movePosition.x + movePosition.y).normalized * speed; //iso movement 
        _rb.MovePosition(_rb.position + movement * Time.fixedDeltaTime);
    }

    private void Shoot()
    {
        if (canShoot && hasAmmo) //if can shoot and has ammo
        {
            PlaySoundEffect(shootSounds);
            GameObject b = Instantiate(bulletType, shootPosition);
            
            //b.GetComponent<Rigidbody>().velocity = Vector3.Normalize( new Vector3 (aimPosition.x,0, aimPosition.y)) * bulletSpeed; //normalizes the aim direction and then fires it at bullet speed
            if (twinStick)
            {
                b.GetComponent<Rigidbody>().velocity = Vector3.Normalize(new Vector3(aimPosition.x - aimPosition.y, 0, aimPosition.x + aimPosition.y)) * bulletSpeed; //normalizes the aim direction and then fires it at bullet speed
            }
            else
            {
                b.GetComponent<Rigidbody>().velocity = Vector3.Normalize(new Vector3(movePosition.x - movePosition.y, 0, movePosition.x + movePosition.y)) * bulletSpeed; //normalizes the aim direction and then fires it at bullet speed
            }
            
            Destroy(b, 20);    //destroys the bullet after 20 seconds

            ammoCount--;
            canShoot = false;

            if (ammoCount <= 0)
            {
                hasAmmo = false;
            }

            StartCoroutine(ShootTiming()); //handles delay betweenshots

        }
        else if (canShoot && !hasAmmo && !isReloading) //checks if its trying to shoot but has no ammo (while not currently reloading)
        {
            StartCoroutine(ReloadTiming());
        }
        UpdateAmmoCount();
    }

    void ToggleTwinstick()
    {
        twinStick = !twinStick;
    }

    private void GainEXP(int amount)
    {
        exp += amount;
        UpdateEXPBar();
        if (exp >= levelIncrement)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        exp -= levelIncrement;
        level++;
        // INSERT A CALL TO SPAWN THE UPGRADE MENU AND PAUSE THE TIME  (ALSO ENSURE THAT AFTER SELECTING THE UPGRADE MENU THAT TIME REVERTS)

        if (exp >= levelIncrement)
        {
            LevelUp();
        }
        UpdateEXPBar();
    }

    #region UI Handling
    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = curHealth;
        }
    }

    void UpdateAmmoCount()
    {
        if (ammoTextBar != null)
        {
            ammoTextBar.text = ammoCount + " / " + maxAmmo;
        }
    }

    void UpdateEXPBar()
    {
        if (expBar != null)
        {
            expBar.value = exp;
        }
    }

    #endregion

    void PlaySoundEffect(List<AudioClip> soundList)
    {
        if (soundList != null)
        {
            AudioSource.PlayClipAtPoint(soundList[Random.Range(0, soundList.Count)], this.transform.position);
        }
    }

    IEnumerator ShootTiming()
    {
        yield return new WaitForSeconds(shootDelay); 
        canShoot = true;
    }

    IEnumerator ReloadTiming()
    {
        isReloading = true; //variable ensures that it does not attempt to reload while already reloading
        PlaySoundEffect(reloadSounds);
        yield return new WaitForSeconds(reloadTime);
        ammoCount = maxAmmo;
        hasAmmo = true;
        isReloading = false;
        UpdateAmmoCount();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            TakeDamage(collision.gameObject.GetComponent<EnemyBase>().GetDamage);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "EXP")
        {
            GainEXP(other.gameObject.GetComponent<ExpDrop>().ExpAmount);
            Destroy(other.gameObject); 
        }
    }
}
