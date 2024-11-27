using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletApplicationHandling : MonoBehaviour
{
    public float bulletDamageMod;

    public bool canBurn;
    public int shotsBetweenBurn;
    public float burnChance;
    public float burnTick;

    public bool isExplosive;
    public int shotsBetweenExplosive;
    public float explosionRange;
    public float explosionDamage;

    public bool canFreeze;
    public int shotsBetweenFreeze;
    public float freezeChance;
    public float freezeTime;

    public bool canBounce;
    public float bounceAmount;

    public bool canPierce;
    public float pierceAmount;

    public bool castWind;
    public int shotsBetweenWind;

    [Header("Particles")]
    public GameObject burnParticles;


    // Update is called once per frame
    void Update()
    {
        
    }

    public void BulletShotEffects(GameObject bullet)
    {
        if (canBurn) { 
            
        }

        if (canFreeze)
        {

        }
    }

    public void BulletHit()
    {
        if (canFreeze && shotsBetweenFreeze > 0)
        {

        }
    }

}
