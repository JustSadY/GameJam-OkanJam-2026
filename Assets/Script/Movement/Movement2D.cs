using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement2D : MonoBehaviour
{
    // Components
    private Rigidbody2D _rigidbody;

    private MovementProfile _movementProfile;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        if (!_movementProfile)
        {
            _movementProfile = ScriptableObject.CreateInstance<MovementProfile>();
        }
    }

    private void MoveWithParams(float acceleration, float deceleration, float input)
    {
        Turn(input);
        Vector2 currentVelocity = _rigidbody.linearVelocity;
        if (Mathf.Abs(input) > _movementProfile.inputThreshold)
        {
            float newSpeed = Mathf.Lerp(currentVelocity.x, _movementProfile.maxWalkSpeed * input,
                acceleration * Time.fixedDeltaTime);
            _rigidbody.linearVelocity = new Vector2(newSpeed, currentVelocity.y);
        }
        else
        {
            float newSpeed = Mathf.Lerp(currentVelocity.x, 0f, deceleration * Time.fixedDeltaTime);
            _rigidbody.linearVelocity = new Vector2(newSpeed, currentVelocity.y);
        }
    }

    private void Turn(float x)
    {
        if (x < -_movementProfile.inputThreshold)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else if (x > _movementProfile.inputThreshold)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    public void Move(float input)
    {
        if (!_rigidbody || !_movementProfile) return;
        MoveWithParams(_movementProfile.acceleration, _movementProfile.deceleration, input);
    }

    public void SetMovementProfile(MovementProfile movementProfile)
    {
        if (!_rigidbody || !_movementProfile || this._movementProfile == movementProfile) return;
        this._movementProfile = movementProfile;
    }
}