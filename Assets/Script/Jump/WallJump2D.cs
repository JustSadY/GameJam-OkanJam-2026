using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WallJump2D : MonoBehaviour
{
    public delegate void WallJumpEvent();

    public event WallJumpEvent OnWallJumpEvent;

    [Header("Wall Jump Directional Force")]
    [Tooltip("Combined directional force for wall jump (X = horizontal, Y = vertical).")]
    public Vector2 wallJumpForce = new Vector2(15f, 1f);

    [Header("Wall Jump Cooldown")] [Tooltip("Cooldown time before another wall jump can be performed.")]
    public float wallJumpCooldown = 0.2f;

    private Rigidbody2D _rigidbody;
    private float _wallJumpTimer;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        UpdateWallJumpCooldown();
    }

    private void UpdateWallJumpCooldown()
    {
        if (_wallJumpTimer > 0f)
        {
            _wallJumpTimer -= Time.fixedDeltaTime;
        }
    }

    public void WallJump(sbyte wallDirection)
    {
        if (_wallJumpTimer > 0f)
            return;

        OnWallJumpEvent?.Invoke();
        Vector2 jumpForce = new Vector2(
            -wallDirection * wallJumpForce.x,
            wallJumpForce.y
        );

        _rigidbody.linearVelocity = Vector2.zero;

        _rigidbody.AddForce(jumpForce, ForceMode2D.Impulse);

        _wallJumpTimer = wallJumpCooldown;
    }

    public bool CanWallJump => _wallJumpTimer <= 0f;
}