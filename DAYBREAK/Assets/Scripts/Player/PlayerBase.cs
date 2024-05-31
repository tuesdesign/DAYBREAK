using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBase : MonoBehaviour
{
    Rigidbody2D _rb;
    PlayerIA _playerInputActions;

    Vector2 aimPosition = Vector2.zero;
    Vector2 movePosition = Vector2.zero;

    [Header("Player Base")]
    [SerializeField] float maxHealth = 10;
    [SerializeField] float curHealth;
    [SerializeField] float speed = 5;
    [SerializeField] Transform shootPosition;

    
    
    [Header("Projectiles")]
    [SerializeField] float shootDelay = 0.5f;
    [SerializeField] GameObject bulletType;
    [SerializeField] float bulletSpeed = 5f;
    bool isShooting;

    

    [Header("UI")]
    [SerializeField] Joystick moveJoystick;
    [SerializeField] Joystick shootJoystick;



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
        if (shootJoystick.isTouched)
        {
            Shoot();
        }
    }

    void Heal(float amount)
    {
        curHealth += amount;
        if(curHealth > maxHealth)
        {
            curHealth = maxHealth;
        }
    }

    void TakeDamage(float damage)
    {
        curHealth -= damage;
        if (curHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {

    }

    private void FixedUpdate()
    {
        GetJSMoveDirection();
        _rb.velocity = movePosition;
    }

    private void Shoot()
    {
        GameObject b = Instantiate(bulletType,shootPosition);
        GetShootDirection();
        b.GetComponent<Rigidbody2D>().velocity = new Vector3(aimPosition.x,aimPosition.y,0);

    }


    void GetJSMoveDirection()
    {
        if (moveJoystick)
        {
            if (moveJoystick.isTouched)
                movePosition = moveJoystick.GetJoystickVector();
        }
        
        
    }
    void GetShootDirection()
    {  
        if (shootJoystick)
        {
            if (shootJoystick.isTouched)
                aimPosition = shootJoystick.GetJoystickVector();
        }
        
    }
}
