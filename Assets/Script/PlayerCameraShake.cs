using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(CinemachineImpulseSource))]
public class PlayerCameraShake : MonoBehaviour
{
    [Header("Shake Profiles")] [SerializeField]
    private float jumpForce = 0.2f;

    [SerializeField] private float dashForce = 0.2f;

    private CinemachineImpulseSource _impulseSource;
    private Jump2D _jump;
    private Dash2D _dash;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _jump = GetComponent<Jump2D>();
        _dash = GetComponent<Dash2D>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        if (_jump != null) _jump.OnJumpTriggered += ShakeOnJump;
        if (_dash != null) _dash.OnDashTriggered += ShakeOnDash;
    }

    private void OnDisable()
    {
        if (_jump != null) _jump.OnJumpTriggered -= ShakeOnJump;
        if (_dash != null) _dash.OnDashTriggered -= ShakeOnDash;
    }

    private void ShakeOnJump()
    {
        TriggerShake(Vector2.up, jumpForce);
    }

    private void ShakeOnDash()
    {
        Vector2 dashDirection = Vector2.right;

        if (_rb != null && _rb.linearVelocity.magnitude > 0.1f)
        {
            dashDirection = _rb.linearVelocity.normalized;
        }

        TriggerShake(dashDirection, dashForce);
    }

    private void TriggerShake(Vector2 direction, float force)
    {
        if (_impulseSource != null)
        {
            Vector3 finalVelocity = new Vector3(direction.x, direction.y, 0f) * force;
            _impulseSource.GenerateImpulseWithVelocity(finalVelocity);
        }
    }
}