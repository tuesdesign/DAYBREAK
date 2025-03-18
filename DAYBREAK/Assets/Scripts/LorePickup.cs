using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LorePickup : MonoBehaviour
{
    public LorePieceSO lore;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        lore.unlocked = true;
        Destroy(gameObject);
    }
}
