using System;
using UnityEngine;
using UnityEngine.UI;


public class EnergyPanel : MonoBehaviour
{
    private Slider _energySlider;
    private PlayerController _playerController;

    private void Start()
    {
        _energySlider = GetComponentInChildren<Slider>();
        _playerController = FindAnyObjectByType<PlayerController>();
    }

    private void Update()
    {
        if (_energySlider != null && _playerController != null)
            _energySlider.value = _playerController.GetEnergySystem().GetCurrentEnergy();
    }
}