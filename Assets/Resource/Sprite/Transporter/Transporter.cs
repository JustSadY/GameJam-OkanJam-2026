using UnityEngine;

public class Transporter : MonoBehaviour
{
    [SerializeField] private float speed = 0.3f;

    private void OnCollisionStay2D(Collision2D other)
    {
        if (!other.collider.CompareTag("Player")) return;
        Rigidbody2D playerRb = other.collider.GetComponent<Rigidbody2D>();
        if (!playerRb) return;
        playerRb.AddForce(Vector2.left * speed, ForceMode2D.Impulse);
    }
}