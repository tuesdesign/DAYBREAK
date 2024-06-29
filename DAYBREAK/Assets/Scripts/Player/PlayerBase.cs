using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerBase : MonoBehaviour
{
    Rigidbody2D _rb;
    PlayerIA _playerInputActions;

    Vector2 aimPosition = Vector2.zero;
    Vector2 movePosition = Vector2.zero;

    [Header("Player Base")]
    [SerializeField] float maxHealth = 10;
    float curHealth;
    [Tooltip("The player's movement speed")]
    [SerializeField] float speed = 2.5f;
    [Tooltip("Where the bullets should spawn from the player")]
    [SerializeField] Transform shootPosition;

    
    [Header("Projectile Stats")]
    [Tooltip("The time in seconds betweenshots")]
    [SerializeField] float shootDelay = 0.5f;
    [Tooltip("the prefab used for the bullet")]
    [SerializeField] GameObject bulletType;
    [SerializeField] float bulletSpeed = 10f;
    [SerializeField] int maxAmmo = 6;
    int ammoCount;

    bool hasAmmo = true;
    bool isReloading = false;
    bool canShoot = true;
    [SerializeField] float reloadTime = 1f;

    [Header("UI")]
    [Tooltip("")]
    [SerializeField] Slider healthBar;
    [Tooltip("The text bar to describe how much ammo the player has in comparisson to their maximum ammo")]
    [SerializeField] TMP_Text ammoTextBar;



    // Start is called before the first frame update
    void Start()
    {
        _playerInputActions = new PlayerIA();
        _playerInputActions.Enable();
        _rb = GetComponent<Rigidbody2D>();

        _playerInputActions.Game.Move.performed += ctx => movePosition = ctx.ReadValue<Vector2>();
        _playerInputActions.Game.Move.canceled += ctx => movePosition = new Vector2(0,0) ;

        _playerInputActions.Game.Fire.performed += ctx => aimPosition = ctx.ReadValue<Vector2>();
        _playerInputActions.Game.Fire.performed += ctx => Shoot();
        _playerInputActions.Game.Fire.canceled += ctx => aimPosition = new Vector2(0,0);


        //stat initialization 
        curHealth = maxHealth;
        ammoCount = maxAmmo;

        //UI Call
        if(healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = curHealth;
        }

        if(ammoTextBar != null)
        {
            ammoTextBar.text = ammoCount + " / " + maxAmmo;
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

    void Die()
    {

    }

    private void FixedUpdate()
    {
        _rb.velocity = (movePosition * speed) ;
    }

    private void Shoot()
    {
        if (canShoot && hasAmmo) //if can shoot and has ammo
        {
            GameObject b = Instantiate(bulletType, shootPosition);
            
            b.GetComponent<Rigidbody2D>().velocity = Vector3.Normalize(aimPosition) * bulletSpeed; //normalizes the aim direction and then fires it at bullet speed
            Destroy(b, 20);

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

    IEnumerator ShootTiming()
    {
        yield return new WaitForSeconds(shootDelay); 
        canShoot = true;
    }

    IEnumerator ReloadTiming()
    {
        isReloading = true; //variable ensures that it does not attempt to reload while already reloading
        yield return new WaitForSeconds(reloadTime);
        ammoCount = maxAmmo;
        hasAmmo = true;
        isReloading = false;
        UpdateAmmoCount();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            TakeDamage(collision.gameObject.GetComponent<EnemyBase>().GetDamage);
        }

    }
}
