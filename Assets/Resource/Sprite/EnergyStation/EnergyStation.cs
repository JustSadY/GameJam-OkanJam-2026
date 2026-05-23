using System;
using UnityEngine;

public class EnergyStation : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.TryGetComponent(out EnergySystem energyStation))
            {
                energyStation.AddEnergy(2);
            }
        }
    }
}