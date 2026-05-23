using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CannonBall : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private GameObject hitVFX;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(float speed)
    {
        _rb.linearVelocity = new Vector2(speed, _rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (hitVFX != null)
        {
            GameObject vfx = Instantiate(hitVFX, transform.position, Quaternion.identity);
            Destroy(vfx, 2f);
        }

        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.EndGame();
        }

        Destroy(gameObject);
    }
}