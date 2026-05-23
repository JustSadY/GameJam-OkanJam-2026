using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Movement2D))]
[RequireComponent(typeof(Jump2D))]
[RequireComponent(typeof(WallJump2D))]
[RequireComponent(typeof(Dash2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Gravity2D))]
[RequireComponent(typeof(EnergySystem))]
public class PlayerController : MonoBehaviour
{
    private static readonly int MoveSpeedHash = Animator.StringToHash("moveSpeed");
    private static readonly int VerticalSpeedHash = Animator.StringToHash("verticalSpeed");
    private static readonly int IsGroundedHash = Animator.StringToHash("isGrounded");
    private static readonly int DashHash = Animator.StringToHash("Dash");

    [Header("Movement Profiles")] [SerializeField]
    private MovementProfile walkProfile;

    [SerializeField] private MovementProfile airProfile;

    [Header("Gravity Profiles")] [SerializeField]
    private GravityProfile defaultGravityProfile;

    [SerializeField] private GravityProfile wallGravityProfile;

    [Header("Energy Variable")] [SerializeField]
    private float moveEnergy = 10f;

    [SerializeField] private float jumpEnergy = 2f;
    [SerializeField] private float dashEnergy = 2f;
    [SerializeField] private float wallPhaseExtraEnergy = 5f;

    [Header("Audio Settings")] [SerializeField]
    private float footstepInterval = 0.35f;

    private float _footstepTimer;

    private Movement2D _movement;
    private Jump2D _jump;
    private Dash2D _dash;
    private CapsuleCollider2D _collider;
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private Gravity2D _gravity;
    private WallJump2D _wallJump;
    private EnergySystem _energySystem;

    private InputSystem_Actions _actions;
    private Vector2 _moveInput;

    private bool _isTouchingWall;
    private sbyte _wallDirection;
    private bool _isGameEnded;
    private int _playerLayer;
    private int _phaseWallLayer;

    public EnergySystem GetEnergySystem() => _energySystem;

    private void Awake()
    {
        _movement = GetComponent<Movement2D>();
        _jump = GetComponent<Jump2D>();
        _dash = GetComponent<Dash2D>();
        _collider = GetComponent<CapsuleCollider2D>();
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _gravity = GetComponent<Gravity2D>();
        _wallJump = GetComponent<WallJump2D>();
        _energySystem = GetComponent<EnergySystem>();

        _actions = new InputSystem_Actions();

        _playerLayer = gameObject.layer;
        _phaseWallLayer = LayerMask.NameToLayer("Invisible");
    }

    private void Start()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnGameEnded += OnGameEnd;
    }

    private void OnEnable()
    {
        _actions.Enable();

        _actions.Player.Jump.performed += OnJumpPerformed;
        _actions.Player.Dash.performed += OnDashPerformed;

        if (_jump != null) _jump.OnJumpTriggered += HandleJumpEnergy;
        if (_dash != null)
        {
            _dash.OnDashTriggered += HandleDashStart;
            _dash.OnDashEnded += HandleDashEnd;
        }

        if (_wallJump != null) _wallJump.OnWallJumpEvent += HandleJumpEnergy;
    }

    private void OnGameEnd()
    {
        _isGameEnded = true;
        _moveInput = Vector2.zero;
        if (_movement != null) _movement.Move(0f);
        if (_rigidbody != null) _rigidbody.linearVelocity = Vector2.zero;
        if (_animator != null)
        {
            _animator.SetFloat(MoveSpeedHash, 0f);
            _animator.SetFloat(VerticalSpeedHash, 0f);
        }

        enabled = false;
    }

    private void OnDisable()
    {
        _actions.Player.Jump.performed -= OnJumpPerformed;
        _actions.Player.Dash.performed -= OnDashPerformed;

        if (_jump != null) _jump.OnJumpTriggered -= HandleJumpEnergy;
        if (_dash != null)
        {
            _dash.OnDashTriggered -= HandleDashStart;
            _dash.OnDashEnded -= HandleDashEnd;
        }

        if (_wallJump != null) _wallJump.OnWallJumpEvent -= HandleJumpEnergy;

        if (GameManager.Instance != null) GameManager.Instance.OnGameEnded -= OnGameEnd;

        _actions.Disable();
    }

    private void Update()
    {
        if (_isGameEnded) return;

        _moveInput = _actions.Player.Move.ReadValue<Vector2>();
        _jump.UpdateGroundedStatus(IsGrounded());
        _animator.SetFloat(MoveSpeedHash, Mathf.Abs(_rigidbody.linearVelocity.x));
        _animator.SetFloat(VerticalSpeedHash, _rigidbody.linearVelocity.y);
        _animator.SetBool(IsGroundedHash, IsGrounded());

        HandleFootsteps();
    }

    private void FixedUpdate()
    {
        if (_isGameEnded)
        {
            if (_movement != null) _movement.Move(0f);
            return;
        }

        if (_dash != null && _dash.IsDashing) return;

        float horizontalInput = _moveInput.x;
        var wallHit = CheckWall();

        _isTouchingWall = (wallHit.right && horizontalInput > 0) || (wallHit.left && horizontalInput < 0);
        _wallDirection = wallHit.right ? (sbyte)1 : wallHit.left ? (sbyte)-1 : (sbyte)0;

        if (IsGrounded() || !_isTouchingWall)
            _gravity.SetGravityProfile(defaultGravityProfile);
        else
            _gravity.SetGravityProfile(wallGravityProfile);

        if (horizontalInput > 0 && wallHit.right)
            horizontalInput = 0;
        if (horizontalInput < 0 && wallHit.left)
            horizontalInput = 0;

        _movement.SetMovementProfile(IsGrounded() ? walkProfile : airProfile);
        _movement.Move(horizontalInput);

        if (Mathf.Abs(horizontalInput) > 0.01f && IsGrounded() && _energySystem != null &&
            _energySystem.GetCurrentEnergy() >= (moveEnergy * Time.fixedDeltaTime))
        {
            _energySystem.ReductionEnergy(moveEnergy * Time.fixedDeltaTime);
        }
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (_isGameEnded || !context.performed) return;
        if (_energySystem != null && _energySystem.GetCurrentEnergy() < jumpEnergy) return;

        if (_isTouchingWall && !IsGrounded() && _wallDirection != 0)
        {
            _wallJump.WallJump(_wallDirection);
        }
        else
        {
            _jump.TryExecuteJump();
        }
    }

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        if (_isGameEnded) return;

        Vector2 dashDir = new(Mathf.Round(_moveInput.x), 0);
        if (!(dashDir.magnitude > 0.1f) || !_dash.CanDash(dashDir)) return;

        bool willPhase = CheckPhaseWallAhead(dashDir);
        float requiredEnergy = dashEnergy + (willPhase ? wallPhaseExtraEnergy : 0f);

        if (_energySystem != null && _energySystem.GetCurrentEnergy() < requiredEnergy) return;

        if (willPhase && _energySystem != null)
        {
            _energySystem.ReductionEnergy(wallPhaseExtraEnergy);
        }

        _animator.SetTrigger(DashHash);
        _dash.Dash(dashDir, IsGrounded());
    }

    private void HandleJumpEnergy()
    {
        if (_energySystem != null) _energySystem.ReductionEnergy(jumpEnergy);
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(SoundType.Jump);
        }
    }

    private void HandleDashStart()
    {
        if (_energySystem != null) _energySystem.ReductionEnergy(dashEnergy);
        Physics2D.IgnoreLayerCollision(_playerLayer, _phaseWallLayer, true);
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(SoundType.Dash);
        }
    }

    private void HandleDashEnd()
    {
        Physics2D.IgnoreLayerCollision(_playerLayer, _phaseWallLayer, false);
    }

    private bool CheckPhaseWallAhead(Vector2 direction)
    {
        Vector2 origin = new Vector2(_collider.bounds.center.x, _collider.bounds.center.y);
        const float rayDistance = 1.5f;
        int layerMask = 1 << _phaseWallLayer;
        return Physics2D.Raycast(origin, direction, rayDistance, layerMask);
    }

    private bool IsGrounded()
    {
        Vector2 origin = new(_collider.bounds.center.x, _collider.bounds.min.y);
        const float rayDistance = 0.3f;
        int layerMask = LayerMask.GetMask("Ground") | LayerMask.GetMask("Invisible");
        return Physics2D.Raycast(origin, Vector2.down, rayDistance, layerMask);
    }

    private (bool left, bool right) CheckWall()
    {
        Vector2 origin = new Vector2(_collider.bounds.center.x, _collider.bounds.center.y);
        const float rayDistance = 0.4f;
        int layerMask = LayerMask.GetMask("Ground") | LayerMask.GetMask("Invisible");

        bool hitRight = Physics2D.Raycast(origin, Vector2.right, rayDistance, layerMask);
        bool hitLeft = Physics2D.Raycast(origin, Vector2.left, rayDistance, layerMask);

        return (hitLeft, hitRight);
    }

    private void HandleFootsteps()
    {
        if (_dash != null && _dash.IsDashing)
        {
            _footstepTimer = 0f;
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.StopWalkSound();
            }
            return;
        }

        if (IsGrounded() && Mathf.Abs(_rigidbody.linearVelocity.x) > 0.1f && Mathf.Abs(_moveInput.x) > 0.1f)
        {
            _footstepTimer -= Time.deltaTime;
            if (_footstepTimer <= 0f)
            {
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayWalkSound();
                }

                _footstepTimer = footstepInterval;
            }
        }
        else
        {
            _footstepTimer = 0f;
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.StopWalkSound();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!_collider) return;

        Vector2 bottom = new Vector2(_collider.bounds.center.x, _collider.bounds.min.y);
        const float rayDistance = 0.4f;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(bottom, bottom + Vector2.right * rayDistance);
        Gizmos.DrawLine(bottom, bottom + Vector2.left * rayDistance);
        Gizmos.DrawLine(bottom, bottom + Vector2.down * rayDistance);
    }
}