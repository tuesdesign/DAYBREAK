using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerShooting : MonoBehaviour
{
    PlayerIA _playerInputActions;
    private InputAction playerShootActions;

    PlayerUI _playerUI;
    Vector2 aimPosition = Vector2.up;
    Vector2 movePosition = Vector2.up;

    [Tooltip("Where the bullets should spawn from the player")]
    [SerializeField] Transform shootPosition;
    [Tooltip("should you use twinstick controls \n if on it uses left and right analog sticks \n if off it only uses the move direction")]
    [SerializeField] bool twinStick = true;

    [Tooltip("The time in seconds betweenshots")]
    [SerializeField] float shootDelay = 0.5f;
    [Tooltip("the prefab used for the bullet")]
    [SerializeField] GameObject bulletType;
    [Tooltip("The speed that bullets are fired")]
    [SerializeField] float bulletSpeed = 10f;
    [SerializeField] int maxAmmo = 6;
    int ammoCount;
    [SerializeField] int bulletsPerShot;

    bool hasAmmo = true;
    bool isReloading = false;
    bool canShoot = true;
    [SerializeField] float reloadTime = .75f;

    [Header("Sound Effects")]
    [SerializeField] List<AudioClip> shootSounds = new List<AudioClip>();
    [SerializeField] AudioClip reloadStartSound = null;
    [SerializeField] List<AudioClip> reloadSounds = new List<AudioClip>();
    [SerializeField] AudioClip reloadStopSound = null;

    //MODIFIERS FOR UPGRADES
    [HideInInspector] public float bspeedMod = 0;
    [HideInInspector] public float shootdelayMod = 0 ;
    [HideInInspector] public float reloadTimeMod = 0 ;
    [HideInInspector] public int maxAmmoMod = 0;

    


    private Canvas reloadBar;

    public int MaxAmmo { get => maxAmmo; set => maxAmmo = value; }
    public int AmmoCount { get => ammoCount; set => ammoCount = value; }
    public float BulletSpeed { get => bulletSpeed; set => bulletSpeed = value; }
    public float ShootDelay { get => shootDelay; set => shootDelay = value; }
    public GameObject BulletType { get => bulletType; set => bulletType = value; }
    public int BulletsPerShot { get => bulletsPerShot; set => bulletsPerShot = value; }

    // Start is called before the first frame update
    void Start()
    {
        _playerInputActions = new PlayerIA();
        _playerInputActions.Enable();
        _playerUI = GetComponent<PlayerUI>();

        ammoCount = maxAmmo;
        canShoot = true;
        
        reloadBar = FindObjectOfType<Canvas>();

        if (twinStick)
        {
            _playerInputActions.Game.Fire.performed += ctx => aimPosition = ctx.ReadValue<Vector2>();
            _playerInputActions.Game.Fire.started += ctx => StartShooting();
            _playerInputActions.Game.Fire.canceled += ctx => StopShooting();

            playerShootActions = _playerInputActions.Game.Fire;
        }
        else
        {
            _playerInputActions.Game.Move.performed += ctx => movePosition = ctx.ReadValue<Vector2>();
            _playerInputActions.Game.Move.started += ctx => StartShooting();
            _playerInputActions.Game.Move.canceled += ctx => StopShooting();

            playerShootActions = _playerInputActions.Game.Move;
        }
    }

    private void OnDisable()
    {
        _playerInputActions.Disable();
    }
    private void Shoot()
    {

        if (canShoot && hasAmmo) //if can shoot and has ammo
        {
            PlaySoundEffect(shootSounds);
            GameObject b = Instantiate(bulletType, shootPosition);

            //Vector2 direction = twinStick ? aimPosition : movePosition;
            Vector2 direction = playerShootActions.ReadValue<Vector2>();
            direction = ConvertToIsometric(direction);

            b.GetComponent<Rigidbody>().velocity = (new Vector3(direction.x , 0, direction.y)).normalized * bulletSpeed;
            

            Destroy(b, 10);    //destroys the bullet after 10 seconds

            ammoCount--;
            canShoot = false;

            if (ammoCount <= 0)
            {
                hasAmmo = false;
                StartCoroutine(ReloadTiming());
            }

            StartCoroutine(ShootTiming());

        }
       
        _playerUI.UpdateAmmoCount();
    }

    void ToggleTwinstick()
    {
        twinStick = !twinStick;

    }

    void PlaySoundEffect(List<AudioClip> soundList)
    {
        if (soundList != null)
        {
            AudioSource.PlayClipAtPoint(soundList[Random.Range(0, soundList.Count)], this.transform.position);
        }
    }

    void PlaySoundEffect(AudioClip sound)
    {
        if (sound != null) 
        {
            AudioSource.PlayClipAtPoint(sound, this.transform.position);
        
        }
    }

    public static Vector2 ConvertToIsometric(Vector2 vector)
    {
        
        float cos45 = Mathf.Sqrt(2) / 2; // Approx 0.707
        float sin45 = Mathf.Sqrt(2) / 2; // Approx 0.707

        float cos35 = Mathf.Cos(35f * Mathf.Deg2Rad); // Cosine of 35 degrees
        float sin35 = Mathf.Sin(35f * Mathf.Deg2Rad);

        // Apply the isometric transformation
        float isoX = vector.x * cos35 - vector.y * sin35;
        float isoY = vector.x * sin45 + vector.y * cos45;

        return new Vector2(isoX, isoY);
    }

    private Coroutine shootingCoroutine;

    private void StartShooting()
    {
        if (shootingCoroutine == null)
        {
            shootingCoroutine = StartCoroutine(ShootContinuously());
        }
    }

    private void StopShooting()
    {
        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
    }

    private IEnumerator ShootContinuously()
    {
        while (true)
        {
            Shoot();
            yield return null;
            //yield return new WaitForSeconds(shootDelay + shootdelayMod); // Delay between shots
        }
    }

    IEnumerator ShootTiming()
    {
        yield return new WaitForSeconds(shootDelay + shootdelayMod);
        canShoot = true;
    }

    IEnumerator ReloadTiming()
    {
        PlaySoundEffect(reloadStartSound);

        isReloading = true; //variable ensures that it does not attempt to reload while already reloading
        StartCoroutine(ReloadTick());

        reloadBar.enabled = true;
        
        yield return new WaitForSeconds(reloadTime + reloadTimeMod);

        //reloadBar.enabled = false;
        ammoCount = maxAmmo + maxAmmoMod;
        hasAmmo = true;
        isReloading = false;
        _playerUI.UpdateAmmoCount();
    }

    IEnumerator ReloadTick()
    {
        if (isReloading)
        {
            ammoCount++;
            yield return new WaitForSeconds((reloadTime+reloadTimeMod)/ (maxAmmo+maxAmmoMod));
            
            PlaySoundEffect(reloadSounds);
            _playerUI.UpdateAmmoCount();
            
            StartCoroutine(ReloadTick());

        }
        else
        {
            PlaySoundEffect(reloadStopSound);
            yield return new WaitForSeconds(reloadTime + reloadTimeMod);
            
        }
    }
}
