using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletApplicationHandling : MonoBehaviour
{

    [HideInInspector] public float bulletDamageMod;

    [HideInInspector] public bool canBurn = false;
    [HideInInspector] public int shotsBetweenBurn;
    int burnshots;
    [HideInInspector] public float burnChance;
    [HideInInspector] public float burnTick;
    public GameObject burnVFX;

    [HideInInspector] public bool canExplode = false;
    [HideInInspector] public int shotsBetweenExplosive;
    int explosiveshots; //counter for how many shots it has been since last explosive
    [HideInInspector] public float explosionRange;
    [HideInInspector] public float explosionDamage;
    public GameObject explosionVFX;

    [HideInInspector] public bool canFreeze = false;
    [HideInInspector] public int shotsBetweenFreeze;
    int freezeshots; //counter for how many shots it has been since last freeze
    [HideInInspector] public float freezeChance; // if it is a percentage chance to freeze, this variable starts at 0 and is  applied to the ones not automatic
    [HideInInspector] public float freezeTime;
    public GameObject freezeVFX;

    [HideInInspector] public bool canSlow = false;
    [HideInInspector] public int shotsBetweenSlow;
    int slowshots; //counter for how many shots it has been since last slow
    [HideInInspector] public float slowChance; // if it is a percentage chance to slow, this variable starts at 0 and is  applied to the ones not automatic
    [HideInInspector] public float slowTime;
    public GameObject slowVFX;

    [HideInInspector] public bool canPoison = false;
    [HideInInspector] public int shotsBetweenPoison;
    int poisonshots; //counter for how many shots it has been since last poison
    [HideInInspector] public float poisonChance; // if it is a percentage chance to poison, this variable starts at 0 and is  applied to the ones not automatic
    [HideInInspector] public float poisonTime;
    public GameObject poisonVFX;

    [HideInInspector] public bool canBounce = false;
    [HideInInspector] public float bounceAmount;

    [HideInInspector] public bool canPierce = false;
    [HideInInspector] public int pierceAmount;

    [HideInInspector] public bool castWind = false;
    [HideInInspector] public int shotsBetweenWind;

    [Header("Particles")]
    public GameObject burnParticles;
    public GameObject freezeParticles;
    public GameObject slowParticles;
    public GameObject poisonParticles;
    public GameObject explosionParticles;


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
                burnshots -= shotsBetweenBurn;
                //apply vfx to bullet
                GameObject v = Instantiate(burnParticles,bullet.transform);
            }
            else if (Random.Range(0, 100) <= burnChance)
            {
                    bullet.CanBurn = true;
                //apply vfx to bullet
                GameObject v = Instantiate(burnParticles, bullet.transform);
            }
            burnshots++;
        }

        if (canPoison)
        {
            if (poisonshots >= shotsBetweenPoison)
            {
                bullet.CanPoision = true;
                                poisonshots -= shotsBetweenPoison;
                //apply vfx to bullet
            }
            else if(Random.Range(0, 100) <= poisonChance)
            {
                bullet.CanBurn = true;
            }
            poisonshots++;
        }

        if (canFreeze)
        {
            //applies the stat to the bullet 
            if (freezeshots >= shotsBetweenFreeze)
            {
                bullet.CanFreeze = true;
                freezeshots -= shotsBetweenFreeze;
                //GameObject v = Instantiate(freezeParticles, bullet.transform);
                //apply vfx to bullet

            }
            else if (Random.Range(0, 100) <= freezeChance)
            {
                bullet.CanFreeze= true;
                //GameObject v = Instantiate(freezeParticles, bullet.transform);
            }
            freezeshots++;
        }

        if (canSlow)
        {
            //applies the stat to the bullet 
            if (slowshots >= shotsBetweenSlow)
            {
                bullet.CanSlow = true;
                slowshots -= shotsBetweenSlow;
                //apply vfx to bullet

            }
            else if (Random.Range(0, 100) <= slowChance)
            {
                bullet.CanSlow = true;
            }
            slowshots++;
        }
        if (pierceAmount > 0) {
            bullet.PeirceAmount = pierceAmount;
        }

        if (canExplode)
        {

        }
    }

}
