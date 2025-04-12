using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;
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
        switch (damagableType)
            {
                case DamagableType.Metal:
                    SoundFXManager.Instance.PlaySoundFXClip(AudioClipManager.Instance.destructionMetalSound, transform, 1f);
                    Debug.Log(damagableType);
                    break;
                case DamagableType.Wood:
                    SoundFXManager.Instance.PlaySoundFXClip(AudioClipManager.Instance.destructionWoodSound, transform, 1f);
                    Debug.Log(damagableType);
                    break;
                case DamagableType.Rock:
                    Debug.Log(damagableType);
                    break;
                default:
                    break;
            }
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
