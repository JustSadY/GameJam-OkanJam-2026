using System;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings Instance { private set; get; }
    public static Action<float> OnVolumeChanged;

    private const string VolumeKey = "SavedVolume";
    [SerializeField] private float volume = 1.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float GetVolume()
    {
        return volume;
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        SaveSettings();
        OnVolumeChanged?.Invoke(volume);
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        volume = PlayerPrefs.GetFloat(VolumeKey, 1.0f);
    }
}