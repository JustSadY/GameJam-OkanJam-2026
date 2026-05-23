using UnityEngine;

[CreateAssetMenu(fileName = "NewGravityProfile", menuName = "Physics/Gravity Profile")]
public class GravityProfile : ScriptableObject
{
    #region Gravity Settings

    [Header("Gravity Scaling")]
    [Tooltip("The maximum gravity scale that can be applied as the character accelerates downward.")]
    [Range(1f, 20f)]
    public float maxGravityScale = 5f;

    [Tooltip("The vertical velocity threshold at which gravity scaling begins to increase.")]
    public float accelerationStartVelocity = -2f;

    [Tooltip("Additional gravity scale applied per unit of downward velocity.")]
    public float gravityPerVelocityUnit = 0.2f;

    #endregion

    #region Fall Control

    [Header("Fall Speed Limits")] [Tooltip("The maximum downward velocity the character can reach while falling.")]
    public float maxFallVelocity = -25f;

    [Tooltip("If vertical velocity is above this threshold, gravity scale may be reset.")]
    public float resetVelocityThreshold = -0.1f;

    #endregion
}