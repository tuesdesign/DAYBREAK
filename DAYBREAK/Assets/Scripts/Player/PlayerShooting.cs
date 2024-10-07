using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    PlayerIA _playerInputActions;
    PlayerUI _playerUI;
    Vector2 aimPosition = Vector2.zero;
    Vector2 movePosition = Vector2.zero;

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

    bool hasAmmo = true;
    bool isReloading = false;
    bool canShoot = true;
    [SerializeField] float reloadTime = .75f;

    [Header("Sound Effects")]
    [SerializeField] List<AudioClip> shootSounds = new List<AudioClip>();
    [SerializeField] List<AudioClip> reloadSounds = new List<AudioClip>();

    public int MaxAmmo { get => maxAmmo; set => maxAmmo = value; }
    public int AmmoCount { get => ammoCount; set => ammoCount = value; }

    // Start is called before the first frame update
    void Start()
    {
        _playerInputActions = new PlayerIA();
        _playerInputActions.Enable();
        _playerUI = GetComponent<PlayerUI>();

        ammoCount = maxAmmo;

        
        //_playerInputActions.Game.Move.canceled += ctx => movePosition = new Vector2(0, 0);

        if (twinStick)
        {
            _playerInputActions.Game.Fire.performed += ctx => aimPosition = ctx.ReadValue<Vector2>();
            //_playerInputActions.Game.Fire.canceled += ctx => aimPosition = new Vector2(0, 0);
        

            _playerInputActions.Game.Fire.started += ctx => StartShooting();
            _playerInputActions.Game.Fire.canceled += ctx => StopShooting();
        }
        else
        {

            _playerInputActions.Game.Move.performed += ctx => movePosition = ctx.ReadValue<Vector2>();
            _playerInputActions.Game.Move.started += ctx => StartShooting();
            _playerInputActions.Game.Move.canceled += ctx => StopShooting();
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
            yield return new WaitForSeconds(shootDelay); // Delay between shots
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
        _playerUI.UpdateAmmoCount();
    }
}
