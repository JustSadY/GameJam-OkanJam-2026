using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { private set; get; }
    [SerializeField] private GameObject EndPanel;

    public delegate void GameEndEvent();

    public event GameEndEvent OnGameEnded;
    private bool _isEnd = false;
    public bool IsEnd() => this._isEnd;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        var system = EnergySystem.Instance;
        if (system != null) system.OnEnergyEndEvent += EndGame;
    }

    public void EndGame()
    {
        if (_isEnd) return;
        _isEnd = true;
        Time.timeScale = 0;
        EndPanel.SetActive(true);
        OnGameEnded?.Invoke();
    }
}