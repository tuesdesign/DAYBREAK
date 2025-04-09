using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableObjects : MonoBehaviour
{
    [SerializeField] float health = 2;
    [SerializeField] GameObject drep;


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
        if (drep != null)
        {
            GameObject d = Instantiate(drep, this.transform, true);
        }
        Destroy(gameObject);
        
    }
}
