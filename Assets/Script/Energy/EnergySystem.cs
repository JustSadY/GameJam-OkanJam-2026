using System;
using UnityEngine;

public class EnergySystem : MonoBehaviour
{
    public static EnergySystem Instance { private set; get; }
    private readonly float _maxEnergy = 100f;
    private float _currentEnergy;

    public delegate void EnergyEndEvent();

    public event EnergyEndEvent OnEnergyEndEvent;
    [SerializeField] private float globalMultiplier = 1f;

    public float GetCurrentEnergy() => _currentEnergy;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        _currentEnergy = _maxEnergy;
    }

    private void Update()
    {
        _currentEnergy -= Time.deltaTime;
        _currentEnergy = Math.Clamp(_currentEnergy, 0f, _maxEnergy);
        if (_currentEnergy == 0f) OnEnergyEndEvent?.Invoke();
    }

    public void SetEnergy(float energy)
    {
        _currentEnergy = Mathf.Clamp(energy, 0f, _maxEnergy);
    }

    public void ReductionEnergy(float energy, float customMultiplier = 1f)
    {
        float finalCost = energy * customMultiplier * globalMultiplier;
        SetEnergy(_currentEnergy - finalCost);
    }

    public void AddEnergy(float energy, float customMultiplier = 1f)
    {
        float finalCost = energy * customMultiplier * globalMultiplier;
        SetEnergy(_currentEnergy + finalCost);
    }

    public void SetGlobalMultiplier(float multiplier)
    {
        globalMultiplier = multiplier;
    }
}