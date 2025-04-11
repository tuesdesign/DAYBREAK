using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum DamagableType
{
    Wood,
    Metal,
    Rock
}

public class DamagableObjects : MonoBehaviour
{
    [SerializeField] float health = 2;
    [SerializeField] GameObject drop;

    [SerializeField] public DamagableType damagableType;

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
        if (drop != null)
        {
            Instantiate(drop, this.transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
        
    }
}
