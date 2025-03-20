using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableObjects : MonoBehaviour
{
    [SerializeField] float health = 2;


    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            DestroySelf();
        }
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
