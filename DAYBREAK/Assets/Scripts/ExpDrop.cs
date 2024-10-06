using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpDrop : MonoBehaviour
{
    [SerializeField] int expAmount;
    [SerializeField] int speed = 1;

    public int ExpAmount { get => expAmount; set => expAmount = value; }
    bool moveToPlayer = false;
    GameObject player;

    private void Update()
    {
        if (moveToPlayer)
        {
            transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, speed);
            if (Vector3.Distance(this.transform.position, player.transform.position) < 1)
            {
                player.GetComponent<PlayerExpHandler>().GainEXP(expAmount);
                Destroy(this.gameObject);
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
