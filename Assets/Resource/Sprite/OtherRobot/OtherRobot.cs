using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class OtherRobot : MonoBehaviour
{
    [Header("Movement")] public float speed = 3f;
    public float moveDistance = 5f;

    [Header("Detection")] public float sightRange = 4f;

    [Header("Animation")] public string animationTriggerName = "Attack";
    public string boolName = "IsMoving";
    public float attackCooldown = 0.5f;

    private Animator animator;
    private Rigidbody2D _rb;
    private Vector3 _startPosition;
    private bool _movingRight = true;
    private bool _isAttacking = false;
    private float _attackTimer = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _startPosition = transform.position;
    }

    void Update()
    {
        CheckForPlayer();

        if (!_isAttacking)
        {
            Patrol();
        }
        else
        {
            HandleAttackTiming();
        }
    }

    void Patrol()
    {
        if (animator != null)
        {
            animator.SetBool(boolName, true);
        }

        float rightLimit = _startPosition.x + moveDistance;
        float leftLimit = _startPosition.x - moveDistance;

        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (_movingRight && transform.position.x >= rightLimit)
        {
            _movingRight = false;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (!_movingRight && transform.position.x <= leftLimit)
        {
            _movingRight = true;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void CheckForPlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, sightRange);
        Debug.DrawRay(transform.position, transform.right * sightRange, Color.red);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            if (!_isAttacking)
            {
                _isAttacking = true;
                _attackTimer = attackCooldown;

                if (animator != null)
                {
                    animator.SetBool(boolName, false);
                }
            }
        }
        else
        {
            _isAttacking = false;
        }
    }

    void HandleAttackTiming()
    {
        _attackTimer += Time.deltaTime;

        if (_attackTimer >= attackCooldown)
        {
            _attackTimer = 0f;

            if (animator != null)
            {
                animator.SetTrigger(animationTriggerName);
            }
        }
    }

    public void OnAttackHitNotify()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, sightRange);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndGame();
            }
        }
    }
}