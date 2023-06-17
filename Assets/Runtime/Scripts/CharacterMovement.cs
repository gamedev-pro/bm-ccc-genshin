using System;
using KinematicCharacterController;
using UnityEngine;

public struct CharacterMovementInputs
{
    public Vector2 moveInput;
}

[RequireComponent(typeof(KinematicCharacterMotor))]
public class CharacterMovement : MonoBehaviour, ICharacterController
{
    [SerializeField]
    private float speed = 10f;

    [SerializeField]
    private float acceleration = 25f;

    [SerializeField]
    private float rotationSpeed = 20f;

    private Vector3 moveInput;

    [NonSerialized]
    private KinematicCharacterMotor motor;

    public Vector3 MoveInput => moveInput;
    
    public Vector3 Velocity => motor.Velocity;
    
    private void Awake()
    {
        motor = GetComponent<KinematicCharacterMotor>();
        motor.CharacterController = this;
    }

    public void SetInputs(in CharacterMovementInputs input)
    {
        moveInput = Vector3.zero;
        if (input.moveInput != Vector2.zero)
        {
            moveInput = new Vector3(input.moveInput.x, 0, input.moveInput.y).normalized;
        }
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        if (moveInput != Vector3.zero)
        {
            var targetRot = Quaternion.LookRotation(new Vector3(moveInput.x, 0, moveInput.z), Vector3.up);
            currentRotation = Quaternion.Slerp(
                currentRotation,
                targetRot,
                1 - Mathf.Exp(-rotationSpeed * deltaTime));
        }
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        var targetVelocity = moveInput * speed;
        currentVelocity = Vector3.Lerp(
            currentVelocity,
            targetVelocity,
            1 - Mathf.Exp(-acceleration * deltaTime));
    }


    #region not implemented
    public void AfterCharacterUpdate(float deltaTime)
    {
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void PostGroundingUpdate(float deltaTime)
    {
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
    }
    #endregion
}
