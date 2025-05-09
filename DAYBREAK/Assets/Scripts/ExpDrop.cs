using System.Collections;
using System.Collections.Generic;
using Audio;
using UI.Scripts.Upgrades;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Feedbacks;

public class ExpDrop : MonoBehaviour
{
    [SerializeField] private int expAmount;
    [SerializeField] private int speed = 1;
    [SerializeField] private GameObject floatingExp;

    public int ExpAmount { get => expAmount; set => expAmount = value; }
    bool moveToPlayer = false;
    GameObject player;
    bool givePlayerEXP = true;

    public MMF_Player MMF_Player;

    // UI exp colors
    private readonly Color _exp1 = new Color32(0, 220, 255, 255);
    private readonly Color _exp2 = new Color32(4, 125, 255, 255);
    private readonly Color _exp5 = new Color32(7, 24, 255, 255);
    private readonly Color _exp10 = new Color32(121, 6, 255, 255);

    private LevelUpEffect _flashEffect;
    
    private void Awake()
    {
        _flashEffect = FindObjectOfType<LevelUpEffect>();
    }

    private void OnEnable()
    {
        MMF_Player = GetComponentInChildren<MMF_Player>();
        MMF_Player.Initialization();

        if (Physics.Raycast(transform.position + Vector3.up * 100, Vector3.down, out RaycastHit hit, 200f, LayerMask.GetMask("Terrain")))
        {
            transform.position = hit.point + Vector3.up;
        }
    }

    private void Update()
    {
        if (moveToPlayer)
        {
            transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, speed);
            if (Vector3.Distance(this.transform.position, player.transform.position) < 1 && givePlayerEXP)
            {
                player.GetComponent<PlayerExpHandler>().GainEXP(expAmount);
                GetComponent<SphereCollider>().enabled = false;
                Destroy(gameObject, .01f);
                givePlayerEXP = false;

                SoundFXManager.Instance.PlaySoundFXClip(AudioClipManager.Instance.pickupSound, transform, 1f);
                
                // Spawn UI floating text + pulse EXP bar
                GameObject expText = Instantiate(floatingExp, transform.position, floatingExp.transform.rotation);
                
                
                // Change color based on amount of exp
                switch (expAmount)
                {
                    case 1:
                        expText.transform.GetChild(0).GetComponent<TextMesh>().color = _exp1;
                        _flashEffect.PulseColor(Color.white, _exp1);
                        break;
                    case 2:
                        expText.transform.GetChild(0).GetComponent<TextMesh>().color = _exp2;
                        _flashEffect.PulseColor(Color.white, _exp2);
                        break;
                    case 5:
                        expText.transform.GetChild(0).GetComponent<TextMesh>().color = _exp5;
                        _flashEffect.PulseColor(Color.white, _exp5);
                        break;
                    case 10:
                        expText.transform.GetChild(0).GetComponent<TextMesh>().color = _exp10;
                        _flashEffect.PulseColor(Color.white, _exp10);
                        break;
                }
                
                expText.transform.GetChild(0).GetComponent<TextMesh>().text = expAmount.ToString();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            moveToPlayer = true;
            
            if (PlayerPrefs.GetInt("ToggleVibration") == 1)
                MMF_Player.PlayFeedbacks();
        }
    }
}
