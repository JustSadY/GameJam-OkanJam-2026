using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Gravity2D : MonoBehaviour
{
    private GravityProfile _gravityProfile;

    private Rigidbody2D _rigidbody;
    private float _currentGravityScale;
    private float _defaultGravityScale;

    public void SetGravityProfile(GravityProfile gravityProfile)
    {
        if (!_rigidbody || !_gravityProfile || this._gravityProfile == gravityProfile) return;
        this._gravityProfile = gravityProfile;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _defaultGravityScale = _rigidbody.gravityScale;
        _currentGravityScale = _defaultGravityScale;
        if (!_gravityProfile)
        {
            _gravityProfile = ScriptableObject.CreateInstance<GravityProfile>();
        }
    }

    private void Update()
    {
        if (!_gravityProfile) return;
        UpdateGravity();
    }

    private void UpdateGravity()
    {
        float verticalVelocity = _rigidbody.linearVelocity.y;

        if (verticalVelocity > _gravityProfile.resetVelocityThreshold)
        {
            ResetGravity();
            return;
        }

        if (verticalVelocity <= _gravityProfile.accelerationStartVelocity)
        {
            float excessVelocity = Mathf.Abs(verticalVelocity - _gravityProfile.accelerationStartVelocity);
            float velocityBasedGravity =
                _defaultGravityScale + (excessVelocity * _gravityProfile.gravityPerVelocityUnit);
            _currentGravityScale = Mathf.Min(velocityBasedGravity, _gravityProfile.maxGravityScale);
        }
        else
        {
            _currentGravityScale = _defaultGravityScale;
        }

        _rigidbody.gravityScale = _currentGravityScale;

        if (verticalVelocity < _gravityProfile.maxFallVelocity)
        {
            Vector2 velocity = _rigidbody.linearVelocity;
            velocity.y = _gravityProfile.maxFallVelocity;
            _rigidbody.linearVelocity = velocity;
        }
    }

    private void ResetGravity()
    {
        _currentGravityScale = _defaultGravityScale;
        _rigidbody.gravityScale = _currentGravityScale;
    }
}