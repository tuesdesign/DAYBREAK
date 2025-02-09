using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletApplicationHandling : MonoBehaviour
{
    public float bulletDamageMod;

    public bool canBurn = false;
    public int shotsBetweenBurn;
    int burnshots;
    public float burnChance;
    public float burnTick;
    public float burnVFX;

    public bool canExplode = false;
    public int shotsBetweenExplosive;
    int explosiveshots; //counter for how many shots it has been since last explosive
    public float explosionRange;
    public float explosionDamage;
    public GameObject explosionVFX;

    public bool canFreeze = false;
    public int shotsBetweenFreeze;
    public int freezeshots; //counter for how many shots it has been since last freeze
    public float freezeChance; // if it is a percentage chance to freeze, this variable starts at 0 and is  applied to the ones not automatic
    public float freezeTime;
    public GameObject freezeVFX;

    public bool canPoison = false;
    public int shotsBetweenPoison;
    public int poisonshots; //counter for how many shots it has been since last poison
    public float poisonChance; // if it is a percentage chance to poison, this variable starts at 0 and is  applied to the ones not automatic
    public float poisonTime;
    public GameObject poisonVFX;

    public bool canBounce = false;
    public float bounceAmount;

    public bool canPierce = false;
    public int pierceAmount;

    public bool castWind = false;
    public int shotsBetweenWind;

    [Header("Particles")]
    public GameObject burnParticles;


    private void OnEnable()
    {
        PlayerShooting.OnBulletShot += OnBulletCreation;
    }

    private void OnDisable()
    {
        PlayerShooting.OnBulletShot -= OnBulletCreation;
    }

    void OnBulletCreation(GameObject bullet)
    {
        BulletShotEffects(bullet.GetComponent<Bullet>());

    }



    public void BulletShotEffects(Bullet bullet)
    {
        bullet.Damage += bulletDamageMod;

        if (canBurn) {
            
            if (burnshots >= shotsBetweenBurn)
            {
                bullet.CanBurn = true;
                burnshots--;
                //apply vfx to bullet
            }
            else
            {
                if (Random.Range(0, 100) >= burnChance)
                {
                    bullet.CanBurn = true;
                    //apply vfx to bullet
                }
            }
            burnshots++;
        }

        if (canFreeze)
        {
            //applies the stat to the bullet 
        }
        if (pierceAmount > 0) {
            bullet.PeirceAmount = pierceAmount;
        }

        if (canExplode)
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
