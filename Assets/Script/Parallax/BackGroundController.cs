using System;
using UnityEngine;

public class BackGroundController : MonoBehaviour
{
    private float _startPos, length;
    [SerializeField] private Camera cam;
    [SerializeField] private float parallaxSpeed;

    private void Start()
    {
        _startPos = this.transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        float distance = cam.transform.position.x * parallaxSpeed;
        float movement = cam.transform.position.x * (1 - parallaxSpeed);
        transform.position = new Vector3(_startPos + distance, transform.position.y, transform.position.z);
        if (movement > _startPos + length)
        {
            _startPos += length;
        }
        else if (movement < _startPos - length)
        {
            _startPos -= length;
        }
    }
}