using UnityEngine;

[CreateAssetMenu(fileName = "NewMovementProfile", menuName = "Player/Movement Profile")]
public class MovementProfile : ScriptableObject
{
    #region Input

    [Header("Input Settings")]
    [Tooltip("Input deadzone threshold: the minimum absolute input value required to start movement.")]
    [Range(0f, 1f)]
    public float inputThreshold = 0.1f;

    #endregion

    #region Movement

    [Header("Movement Settings")]
    [Tooltip("The maximum speed the character can reach while walking (units per second).")]
    public float maxWalkSpeed = 12.5f;

    [Tooltip("How quickly the character accelerates to the target speed.")]
    public float acceleration = 5f;

    [Tooltip("How quickly the character slows down when input is released.")]
    public float deceleration = 20f;

    #endregion
}