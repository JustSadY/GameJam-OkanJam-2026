using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class Jump2D : MonoBehaviour
{
    public delegate void JumpEvent();

    public event JumpEvent OnJumpTriggered;
    private Rigidbody2D _rigidbody;

    [Header("Jump Settings")]
    [Tooltip("Force applied when jumping (higher = higher jumps)")]
    [FormerlySerializedAs("JumpForce")]
    [SerializeField]
    private float jumpForce = 5f;

    [Tooltip("Maximum number of jumps allowed (1 = single jump, 2 = double jump, etc.)")]
    [FormerlySerializedAs("MaxJumpCount")]
    [SerializeField]
    private int maxJumpCount = 1;

    [Header("Coyote Time")]
    [Tooltip("Grace period after leaving ground where player can still jump (in seconds)")]
    [FormerlySerializedAs("MaxCoyoteTime")]
    [SerializeField]
    private float maxCoyoteTime = 0.1f;

    private int _jumpCount;
    private float _coyoteTimeRemaining;
    private bool _wasGrounded;
    private bool _isGrounded;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _coyoteTimeRemaining = maxCoyoteTime;
    }

    public void UpdateGroundedStatus(bool isGrounded)
    {
        _isGrounded = isGrounded;

        if (_isGrounded)
        {
            _jumpCount = 0;
            _coyoteTimeRemaining = maxCoyoteTime;
        }
        else
        {
            if (_wasGrounded)
            {
                _coyoteTimeRemaining = maxCoyoteTime;
            }
            else
            {
                _coyoteTimeRemaining -= Time.deltaTime;
            }
        }

        _wasGrounded = _isGrounded;
    }

    public void TryExecuteJump()
    {
        if (!CanJump()) return;

        PerformJump();
    }

    private bool CanJump()
    {
        if (_jumpCount >= maxJumpCount) return false;

        if (_jumpCount == 0)
        {
            return _isGrounded || _coyoteTimeRemaining > 0f;
        }

        return true;
    }

    private void PerformJump()
    {
        _jumpCount++;
        _coyoteTimeRemaining = 0f;

        Vector2 velocity = _rigidbody.linearVelocity;
        velocity.y = 0f;
        _rigidbody.linearVelocity = velocity;
        OnJumpTriggered?.Invoke();
        _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void ResetJumpCount() => _jumpCount = 0;
    public int GetJumpCount() => _jumpCount;
    public void SetJumpCount(int count) => _jumpCount = count;
}