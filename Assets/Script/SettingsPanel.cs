using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponentInChildren<Slider>();
    }

    private void OnEnable()
    {
        if (_slider != null)
        {
            _slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    private void OnDisable()
    {
        if (_slider != null)
        {
            _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }

    private void Start()
    {
        if (Settings.Instance != null && _slider != null)
        {
            _slider.value = Settings.Instance.GetVolume();
        }
    }

    private void OnSliderValueChanged(float value)
    {
        if (Settings.Instance != null)
        {
            Settings.Instance.SetVolume(value);
        }
    }
}