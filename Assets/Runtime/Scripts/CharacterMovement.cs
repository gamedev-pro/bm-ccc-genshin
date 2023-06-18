using System;
using KinematicCharacterController;
using UnityEngine;

public struct CharacterMovementInputs
{
    public Vector2 moveInput;
}

[Serializable]
public struct MovementParameters
{
    public float Speed;
    public float Acceleration;
    public float RotationSpeed;
}

[RequireComponent(typeof(KinematicCharacterMotor))]
public class CharacterMovement : MonoBehaviour, ICharacterController
{
    public bool IsRunning;
    public bool IsSprinting;

    public MovementParameters WalkParameters;
    public MovementParameters RunParameters;
    public MovementParameters SprintParameters;

    private Vector3 moveInput;


    [NonSerialized] private KinematicCharacterMotor motor;

    public Vector3 MoveInput => moveInput;
    public Vector3 Velocity => motor.Velocity;

    public MovementParameters CurrentMovementParameters
    {
        get
        {
            if (IsSprinting)
            {
                return SprintParameters;
            }

            if (IsRunning)
            {
                return RunParameters;
            }

            return WalkParameters;
        }
    }

    private void Awake()
    {
        motor = GetComponent<KinematicCharacterMotor>();
        motor.CharacterController = this;
    }

    public void SetMovementInput(float horizontal, float vertical)
    {
        moveInput = Vector3.zero;
        if (horizontal != 0 || vertical != 0)
        {
            moveInput = new Vector3(horizontal, 0, vertical).normalized;
        }
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        if (moveInput != Vector3.zero)
        {
            var movementParameters = CurrentMovementParameters;
            var targetRot = Quaternion.LookRotation(new Vector3(moveInput.x, 0, moveInput.z), Vector3.up);
            currentRotation = Quaternion.Slerp(
                currentRotation,
                targetRot,
                1 - Mathf.Exp(-movementParameters.RotationSpeed * deltaTime));
        }
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        var movementParameters = CurrentMovementParameters;
        var targetVelocity = moveInput * movementParameters.Speed;
        currentVelocity = Vector3.Lerp(
            currentVelocity,
            targetVelocity,
            1 - Mathf.Exp(-movementParameters.RotationSpeed * deltaTime));
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

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
        ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
        ref HitStabilityReport hitStabilityReport)
    {
    }

    public void PostGroundingUpdate(float deltaTime)
    {
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
        Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
    }

    #endregion
}