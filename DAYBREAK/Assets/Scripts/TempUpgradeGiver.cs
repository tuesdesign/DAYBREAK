using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempUpgradeGiver : MonoBehaviour
{
    public UpgradeBaseSO upgrade;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            FindObjectOfType<UpgradeHandling>().IncreaseUpgrade(upgrade);
            Destroy(this.gameObject);
            Debug.Log("Upgrade be Upon ye");
        }
    }

}
