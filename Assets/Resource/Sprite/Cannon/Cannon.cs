using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Cannon : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private float fireSpeed = 5;
    [SerializeField] private Transform spawnPos;
    [SerializeField] private GameObject ball;
    [SerializeField] private float ballSpeed = 10f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        InvokeRepeating(nameof(Fire), 1, fireSpeed);
    }

    private void Fire()
    {
        _animator.SetTrigger("Fire");
    }

    private void SpawnBall()
    {
        GameObject o = Instantiate(ball, spawnPos.position, spawnPos.rotation);

        float directionX = Mathf.Sign(transform.localScale.x);

        Vector3 ballScale = o.transform.localScale;
        ballScale.x = Mathf.Abs(ballScale.x) * directionX;
        o.transform.localScale = ballScale;

        CannonBall moveScript = o.AddComponent<CannonBall>();
        moveScript.Setup(directionX * ballSpeed);
    }
}