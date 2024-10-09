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

    //Player 
    [Header("Player Base")]
    [SerializeField] int maxHealth = 10;
    int curHealth;
    [Tooltip("The player's movement speed")]
    [SerializeField] float speed = 2.5f;


    //UpgradeModifiers
    [HideInInspector] public int maxHealthModifier = 0;
    [HideInInspector] public int speedModifier = 0;


    

    public int CurHealth { get => curHealth; set => curHealth = value; }
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
   


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
    }


    private void OnDisable()
    {
        _playerInputActions.Disable();
    }

    private void FixedUpdate()
    {

        //Vector3 movement = new Vector3(movePosition.x, 0f, movePosition.y).normalized * speed; //normal movement (non iso)
        Vector3 movement = new Vector3(movePosition.x - movePosition.y, 0f, movePosition.x + movePosition.y).normalized * speed; //iso movement 
        _rb.MovePosition(_rb.position + movement * Time.fixedDeltaTime);
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
    }

    void Die() //empty for now
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        if (collision.gameObject.tag == "Enemy")
        {
            TakeDamage((int)collision.gameObject.GetComponent<EnemyBase>().GetDamage());
        }

    }

}
