using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using MoreMountains.Feedbacks;

public class PlayerShooting : MonoBehaviour
{
    PlayerIA _playerInputActions;
    private InputAction playerShootActions;
    private CharacterAnimController _playerAnimController;

    PlayerUI _playerUI;
    
    Vector2 movePosition = Vector2.up;

    [Tooltip("Where the bullets should spawn from the player")]
    [SerializeField] Transform shootPosition;


    [SerializeField] Transform shootVisualizer;
    [SerializeField] float visualizerDistance = 3f;

    [Tooltip("Should you use twinstick controls \n if on it uses left and right analog sticks \n if off it only uses the move direction")]
    [SerializeField] bool twinStick = true;
    [SerializeField] bool mouseAim = true;
    public Vector2 aimPosition;
    [SerializeField]  Camera cam;


    [Tooltip("The time in seconds between shots")]
    [SerializeField] float shootDelay = 0.5f;
    [Tooltip("the prefab used for the bullet")]
    [SerializeField] GameObject bulletType;
    [Tooltip("The speed that bullets are fired")]
    [SerializeField] float bulletSpeed = 10f;
    [SerializeField] int maxAmmo = 6;
    int ammoCount;

    [SerializeField] int bulletsPerShot = 1;
    [SerializeField] float bulletSpread = 15f;


    bool hasAmmo = true;
    bool isReloading = false;
    bool canShoot = true;
    [SerializeField] float reloadTime = .75f;

    //MODIFIERS FOR UPGRADES
    [HideInInspector] public float bspeedMod = 0;
    [HideInInspector] public float shootdelayMod = 0 ;
    [HideInInspector] public float reloadTimeMod = 0 ;
    [HideInInspector] public int maxAmmoMod = 0;


    public static event Action<GameObject> OnBulletShot;



    private Canvas reloadBar;

    public int MaxAmmo { get => maxAmmo; set => maxAmmo = value; }
    public int AmmoCount { get => ammoCount; set => ammoCount = value; }
    public float BulletSpeed { get => bulletSpeed; set => bulletSpeed = value; }
    public float ShootDelay { get => shootDelay; set => shootDelay = value; }
    public GameObject BulletType { get => bulletType; set => bulletType = value; }
    public int BulletsPerShot { get => bulletsPerShot; set => bulletsPerShot = value; }
    public float BulletSpread { get => bulletSpread; set => bulletSpread = value; }

    MMF_Player _player;
    
    public Vector2 inputDirection;
    
    
    private void Awake()
    {
        ammoCount = maxAmmo;
        _player = GetComponent<MMF_Player>();
        _player.Initialization();
        _playerAnimController = GetComponentInChildren<CharacterAnimController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerInputActions = new PlayerIA();
        _playerInputActions.Enable();
        _playerUI = GetComponent<PlayerUI>();

        canShoot = true;
        
        reloadBar = FindObjectOfType<Canvas>();
        
        CheckTwinstick();
        CheckMouseAim();
    }

    private void OnDisable()
    {
        _playerInputActions.Disable();
    }

    private void Shoot()
    {
        if (MenuStateManager.Instance.isMobile)
        {
            //inputDirection = ConvertToIsometric(inputDirection);
        }
        else if (mouseAim)
        {
            MouseShooting();
            inputDirection = aimPosition;
        }
        else if (!MenuStateManager.Instance.isMobile)
        {
            inputDirection = playerShootActions.ReadValue<Vector2>();
            inputDirection = ConvertToIsometric(inputDirection);
        }
        
        
        
        AimVisualizer(inputDirection);
        if (canShoot && hasAmmo) //if can shoot and has ammo
        {
            SoundFXManager.Instance.PlayRandomSoundFXClip(AudioClipManager.Instance.shootSounds, transform, 1f);

            float angleStep = bulletsPerShot > 1 ? bulletSpread / (bulletsPerShot - 1) : 0;
            float startingAngle = -bulletSpread / 2;

            
            if (bulletsPerShot == 1)
            {

                GameObject b = Instantiate(bulletType, shootPosition.position, Quaternion.identity);
                b.GetComponent<Rigidbody>().velocity = (new Vector3(inputDirection.x, 0, inputDirection.y)).normalized * bulletSpeed;

                if (PlayerPrefs.GetInt("ToggleVibration") == 1)
                {
                    _player.PlayFeedbacks();
                }
                    
                //HapticPatterns.PlayEmphasis(1.0f, 0.0f);

                OnBulletShot?.Invoke(b);

                //Destroy(b, 10);

                ammoCount--;
                PlayerPrefs.SetInt("ShotsFired", PlayerPrefs.GetInt("ShotsFired") + 1);
                
                if (ammoCount <= 0)
                {
                    hasAmmo = false;
                    //HapticPatterns.PlayEmphasis(1.0f, 0.0f);
                    StartCoroutine(ReloadTiming());
                }

            }
            else
            {
                for (int i = 0; i < bulletsPerShot; i++)
                {
                    // Calculate bullet direction with spread
                    Vector3 baseDirection = new Vector3(inputDirection.x, 0, inputDirection.y).normalized;

                    // Rotate direction by the spread angle
                    Quaternion rotation = Quaternion.Euler(0, startingAngle + (i * angleStep), 0);
                    Vector3 bulletDirection = rotation * baseDirection;

                    // Spawn bullet 
                    GameObject b = Instantiate(bulletType, shootPosition.position, Quaternion.identity);
                    Rigidbody rb = b.GetComponent<Rigidbody>();
                    rb.velocity = bulletDirection * (bulletSpeed + bspeedMod);

                    // Align bullet rotation with direction
                    b.transform.forward = rb.velocity.normalized;

                    // Destroy bullet after 10 seconds
                    //Destroy(b, 10);

                    OnBulletShot?.Invoke(b);

                    ammoCount--;
                    PlayerPrefs.SetInt("ShotsFired", PlayerPrefs.GetInt("ShotsFired") + 1);
                    
                    if (ammoCount <= 0)
                    {
                        //HapticPatterns.PlayEmphasis(1.0f, 0.0f);
                        ammoCount = 0;
                        hasAmmo = false;
                        StartCoroutine(ReloadTiming());
                        break;
                    }
                }
            }
            
            canShoot = false;
            StartCoroutine(ShootTiming());
        }
       
        // Update Ammo UI Text
        _playerUI.UpdateAmmoCount();
        
        // Update Ammo UI Images
        _playerUI.UpdateAmmoDisplayRemove(bulletsPerShot);
    }

    void MouseShooting()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
   
        Ray ray = cam.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 hitPosition = hit.point;
            Debug.DrawLine(shootPosition.position, hitPosition, Color.red,0.1f,true);

            aimPosition = new Vector2((hitPosition.x - shootPosition.transform.position.x), (hitPosition.z - shootPosition.transform.position.z));
        }
    }

    public void CheckTwinstick()
    {
        if (playerShootActions != null)
        {
            playerShootActions.Disable();
            
            playerShootActions.started -= ctx => StartShooting();
            playerShootActions.canceled -= ctx => StopShooting();
        }

        if (PlayerPrefs.GetInt("ToggleTwinStick") == 1)
        {
            playerShootActions = _playerInputActions.Game.Fire;
            playerShootActions.Enable();

            playerShootActions.started += ctx => StartShooting();
            playerShootActions.canceled += ctx => StopShooting();
        }
        else
        {
            playerShootActions = _playerInputActions.Game.Move;
            playerShootActions.Enable();

            playerShootActions.started += ctx => StartShooting();
            playerShootActions.canceled += ctx => StopShooting();
        }
    }

    public void CheckMouseAim()
    {
        if (PlayerPrefs.GetInt("MouseAim") == 1)
        {
            playerShootActions.Disable();
            StartShooting();
            mouseAim = true;
        }
        else
        {
            StopShooting();
            playerShootActions.Enable();
            mouseAim = false;
        }
    }

    void AimVisualizer(Vector2 aimDir)
    {
        if (aimDir.sqrMagnitude > 0.01f) 
        {
            aimDir.Normalize(); 
            
            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
            Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * visualizerDistance;
            shootVisualizer.position = shootPosition.position + offset;
        }
    }

    public static Vector2 ConvertToIsometric(Vector2 vector)
    {
        float cos45 = Mathf.Sqrt(2) / 2; // Approx 0.707
        float sin45 = Mathf.Sqrt(2) / 2; // Approx 0.707

        // Apply the isometric transformation
        float isoX = vector.x * cos45 - vector.y * sin45;
        float isoY = vector.x * sin45 + vector.y * cos45;

        return new Vector2(isoX, isoY);
    }

    private Coroutine shootingCoroutine;

    public void StartShooting()
    {
        if (shootingCoroutine == null)
        {
            shootingCoroutine = StartCoroutine(ShootContinuously());
            _playerAnimController.isShooting = true;
        }
    }

    public void StopShooting()
    {
        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
            _playerAnimController.isShooting = false;
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

    public void ForceReload()
    {
        StartCoroutine(ReloadTiming());
    }

    IEnumerator ReloadTiming()
    {
        SoundFXManager.Instance.PlaySoundFXClip(AudioClipManager.Instance.reloadStartSound, transform, 1f);
        
        isReloading = true; //variable ensures that it does not attempt to reload while already reloading
        StartCoroutine(ReloadTick());

        reloadBar.enabled = true;
        
        yield return new WaitForSeconds(reloadTime + reloadTimeMod);

        ammoCount = maxAmmo + maxAmmoMod;
        hasAmmo = true;
        isReloading = false;
        _playerUI.UpdateAmmoCount();
    }

    IEnumerator ReloadTick()
    {
        if (isReloading && ammoCount < maxAmmo + maxAmmoMod)
        {
            _playerUI.UpdateAmmoDisplayAdd();
            
            ammoCount++;
            yield return new WaitForSeconds((reloadTime+reloadTimeMod) / (maxAmmo+maxAmmoMod));
            
            SoundFXManager.Instance.PlaySoundFXClip(AudioClipManager.Instance.reloadSound, transform, 1f);
            _playerUI.UpdateAmmoCount();
            
            StartCoroutine(ReloadTick());
        }
        else
        {
            SoundFXManager.Instance.PlaySoundFXClip(AudioClipManager.Instance.reloadStopSound, transform, 1f);
            yield return new WaitForSeconds((reloadTime + reloadTimeMod) / (maxAmmo + maxAmmoMod));
        }
    }
}
