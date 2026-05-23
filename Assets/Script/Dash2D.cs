using UnityEngine;
using UnityEngine.Serialization;

public class Dash2D : MonoBehaviour
{
    public delegate void DashEvent();
    public event DashEvent OnDashTriggered;
    public event DashEvent OnDashEnded;

    public bool IsDashing => _isDashing;

    [Header("Dash Settings")]
    [FormerlySerializedAs("DashForce")]
    [SerializeField]
    private float dashForce = 15f;

    [SerializeField]
    private float dashDuration = 0.2f;

    [FormerlySerializedAs("MaxDashCount")]
    [SerializeField]
    private int maxDashCount = 1;

    [Header("Cooldown Settings")]
    [FormerlySerializedAs("DashCooldown")]
    [SerializeField]
    private float dashCooldown = 1f;

    [Header("Physics Settings")]
    [FormerlySerializedAs("ResetVerticalVelocity")]
    [SerializeField]
    private bool resetVerticalVelocity = true;

    private Rigidbody2D _rigidbody;

    private int _dashCount;
    private float _defaultGravityScale;
    private float _dashTimeRemaining;
    private float _cooldownTimeRemaining;
    private bool _isDashing;
    private Vector2 _dashDirection;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _defaultGravityScale = _rigidbody.gravityScale;
    }

    private void Update()
    {
        UpdateDash();
        UpdateCooldown();
    }

    private void FixedUpdate()
    {
        if (_isDashing)
        {
            Vector2 dashVelocity = _dashDirection * dashForce;
            _rigidbody.linearVelocity = dashVelocity;
        }
    }

    private void UpdateDash()
    {
        if (!_isDashing) return;

        _dashTimeRemaining -= Time.deltaTime;

        if (_dashTimeRemaining <= 0f)
        {
            EndDash();
        }
    }

    private void UpdateCooldown()
    {
        if (_cooldownTimeRemaining > 0f)
        {
            _cooldownTimeRemaining -= Time.deltaTime;
        }
    }

    public void Dash(Vector2 direction, bool isGrounded)
    {
        if (isGrounded)
        {
            _dashCount = 0;
        }

        if (!CanDash(direction)) return;

        PerformDash(direction.normalized);
    }

    public bool CanDash(Vector2 direction)
    {
        if (_isDashing) return false;
        if (_cooldownTimeRemaining > 0f) return false;
        if (_dashCount >= maxDashCount) return false;
        if (direction.magnitude < 0.1f) return false;

        return true;
    }

    private void PerformDash(Vector2 direction)
    {
        _dashCount++;
        _isDashing = true;
        _dashDirection = direction;
        _dashTimeRemaining = dashDuration;
        _cooldownTimeRemaining = dashCooldown;

        _rigidbody.gravityScale = 0f;

        OnDashTriggered?.Invoke();

        if (resetVerticalVelocity)
        {
            _rigidbody.linearVelocity = Vector2.zero;
        }
    }

    private void EndDash()
    {
        _isDashing = false;
        _rigidbody.gravityScale = _defaultGravityScale;

        OnDashEnded?.Invoke();
    }
}