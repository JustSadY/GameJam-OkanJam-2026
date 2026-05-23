using System;
using UnityEngine;
using UnityEngine.UI;

public class Trail : MonoBehaviour
{
    private Text _text;
    private GameObject _textObject;
    [SerializeField] private string text;

    private void Awake()
    {
        _textObject = GameObject.Find("Trail");
        if (_textObject != null)
        {
            _text = _textObject.GetComponentInChildren<Text>();
        }
    }

    private void Start()
    {
        _textObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _text.text = text;
            _textObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _text.text = "";
            _textObject.SetActive(false);
        }
    }
}