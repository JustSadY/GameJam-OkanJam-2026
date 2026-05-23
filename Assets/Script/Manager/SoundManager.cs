using System;
using UnityEngine;

public enum SoundType
{
    None,
    Dash,
    Walk,
    Jump,
}

[System.Serializable]
public struct SoundData
{
    public AudioClip clip;
    public SoundType soundType;
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { private set; get; }

    [SerializeField] private SoundData[] soundData;
    private AudioSource audioSource;
    private AudioSource walkAudioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            
            walkAudioSource = gameObject.AddComponent<AudioSource>();
            walkAudioSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        Settings.OnVolumeChanged += UpdateVolume;
    }

    private void OnDisable()
    {
        Settings.OnVolumeChanged -= UpdateVolume;
    }

    private void Start()
    {
        if (Settings.Instance != null)
        {
            UpdateVolume(Settings.Instance.GetVolume());
        }
    }

    public void PlaySound(SoundType soundType)
    {
        AudioClip clip = GetSound(soundType);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private AudioClip GetSound(SoundType soundType)
    {
        for (int i = 0; i < soundData.Length; i++)
        {
            if (soundData[i].soundType == soundType)
            {
                return soundData[i].clip;
            }
        }

        return null;
    }

    private void UpdateVolume(float newVolume)
    {
        audioSource.volume = newVolume;
        if (walkAudioSource != null)
        {
            walkAudioSource.volume = newVolume;
        }
    }

    public void PlayWalkSound()
    {
        AudioClip clip = GetSound(SoundType.Walk);
        if (clip != null && walkAudioSource != null)
        {
            walkAudioSource.clip = clip;
            walkAudioSource.Play();
        }
    }

    public void StopWalkSound()
    {
        if (walkAudioSource != null && walkAudioSource.isPlaying)
        {
            walkAudioSource.Stop();
        }
    }
}