using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpDrop : MonoBehaviour
{
    [SerializeField] private int expAmount;
    [SerializeField] private int speed = 1;
    [SerializeField] private GameObject floatingExp;

    public int ExpAmount { get => expAmount; set => expAmount = value; }
    bool moveToPlayer = false;
    GameObject player;
    bool givePlayerEXP = true;

    public AudioClip pickupSound;

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

                AudioSource.PlayClipAtPoint(pickupSound, this.transform.position);
                
                // Spawn UI floating text
                GameObject expText = Instantiate(floatingExp, transform.position, floatingExp.transform.rotation) as GameObject;
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
        }
    }
}
