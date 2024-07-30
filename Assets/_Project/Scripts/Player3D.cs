using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class Player3D : MonoBehaviour
{
    [Header("Controller")]
    [SerializeField] private Controller controller;
    [SerializeField] private float forwardAcceleration = 40;
    [SerializeField] private float backwardAcceleration = 40;
    [SerializeField] private float sideAcceleration = 40;
    [SerializeField] private float friction = 5;
    [SerializeField] private Vector2 velocityToAdd = Vector2.zero;
    
    private Vector2 _baseVelocity = Vector2.zero;
    private Vector2 _velocity = Vector2.zero;
    private float _speed = 0;
    private float maxSpeedEstimation;
    private float speedProgress;
    
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 90;

    [Header("Arc Cast")]
    [SerializeField, Range(0, 360)] private float arcAngle = 270;
    [SerializeField] private int arcResolution = 6;
    [SerializeField] private LayerMask arcLayer;
    [SerializeField] private Transform arcTransformRotation;
    
    public Controller Controller => controller;
    public Vector2 Velocity => _velocity;
    public Vector3 Velocity3 => new Vector3(_velocity.x, 0, _velocity.y);
    public float Speed => _speed;
    public float SpeedProgress => speedProgress;

    private void OnValidate()
    {
        EstimateMaxSpeed();
    }

    private void Awake()
    {
        EstimateMaxSpeed();
    }

    private void OnDisable()
    {
        _baseVelocity = Vector2.zero;
        UpdateVelocity();
    }

    private void Update()
    {
        ApplyVelocity();
        Rotate();
    }

    private void FixedUpdate()
    {
        ApplyAcceleration();
        ApplyFriction();
        UpdateVelocity();
    }

    private void UpdateVelocity()
    {
        _velocity = _baseVelocity + velocityToAdd;
        UpdateSpeed();
    }
    
    private void UpdateSpeed()
    {
        _speed = _velocity.magnitude;
        speedProgress = Mathf.Clamp01(_speed / maxSpeedEstimation);
    }

    private void EstimateMaxSpeed()
    {
        float velocity = 0, speed;
        
        // Calculate forward speed
        for (float t = 0; t < 10; t += Time.fixedDeltaTime)
        {
            velocity += forwardAcceleration * Time.fixedDeltaTime;
            velocity -= velocity * friction * Time.fixedDeltaTime;
        }
        velocity += velocityToAdd.y;
        speed = Mathf.Abs(velocity);

        maxSpeedEstimation = speed;

        velocity = 0;

        for (float t = 0; t < 10; t += Time.fixedDeltaTime)
        {
            velocity -= backwardAcceleration * Time.fixedDeltaTime;
            velocity -= velocity * friction * Time.fixedDeltaTime;
        }
        velocity += velocityToAdd.y;
        speed = Mathf.Abs(velocity);

        maxSpeedEstimation = Mathf.Max(maxSpeedEstimation, speed);

        velocity = 0;
        
        for (float t = 0; t < 10; t += Time.fixedDeltaTime)
        {
            velocity += Time.fixedDeltaTime * sideAcceleration;
            velocity -= velocity * friction * Time.fixedDeltaTime;
        }

        velocity += velocityToAdd.x * (velocityToAdd.x > 0 == velocity > 0 ? 1 : -1);
        speed = Mathf.Abs(velocity);
        
        maxSpeedEstimation = Mathf.Max(maxSpeedEstimation, speed);
    }

    private void ApplyAcceleration()
    {
        if (!controller)
            return;

        Vector2 stickL = controller.StickL;

        if (stickL != Vector2.zero)
        {
            _baseVelocity += new Vector2(sideAcceleration, stickL.y > 0 ? forwardAcceleration : backwardAcceleration) *
                             stickL * Time.fixedDeltaTime;
        }
    }

    private void ApplyFriction()
    {
        _baseVelocity -= _baseVelocity * (friction * Time.fixedDeltaTime);
    }

    private void ApplyVelocity()
    {
        if (_velocity == Vector2.zero)
            return;

        float arcRadius = _speed * Time.deltaTime;
        Vector3 worldVelocity = arcTransformRotation.TransformVector(Velocity3);

        // Quaternion.LookRotation(worldVelocity, arcTransformRotation.up)
        // makes the object look towards its velocity vector and
        // arcTransformRotation.up is the object's
        // object space up vector
        if (PhysicsExtensions.ArcCast(transform.position,
                Quaternion.LookRotation(worldVelocity, arcTransformRotation.up), arcAngle, arcRadius, arcResolution,
                arcLayer, out RaycastHit hit))
        {
            transform.position = hit.point;
            transform.MatchUp(hit.normal);
        }
    }

    private void Rotate()
    {
        if (!controller)
            return;

        Vector2 stickR = controller.StickR;

        if (stickR.x != 0)
        {
            transform.Rotate(0, stickR.x * rotationSpeed * Time.deltaTime, 0);
        }
    }
}
