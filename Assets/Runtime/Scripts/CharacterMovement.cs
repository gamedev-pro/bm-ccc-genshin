using System;
using KinematicCharacterController;
using UnityEngine;

public struct CharacterMovementInputs
{
    public Vector2 moveInput;
    public bool WantsToJump;
}

[Serializable]
public struct MovementParameters
{
    public float Speed;
    public float Acceleration;
    public float RotationSpeed;
}

[Serializable]
public struct JumpParameters
{
    public float JumpHeight;
    public float TimeToApex;

    [Range(0.01f, 0.5f)] public float JumpRequestExpireDuration;

    public float CalculateJumpSpeed(float gravity)
    {
        return (float)(JumpHeight - 0.5 * -gravity * TimeToApex * TimeToApex) / TimeToApex;
    }
}

[RequireComponent(typeof(KinematicCharacterMotor))]
public class CharacterMovement : MonoBehaviour, ICharacterController
{
    public bool IsRunning;
    public bool IsSprinting;

    public float Gravity = 30;
    public MovementParameters WalkParameters;
    public MovementParameters RunParameters;
    public MovementParameters SprintParameters;
    public JumpParameters JumpParameters;

    private Vector3 moveInput;
    private float wantsToJumpExpireTime;

    public bool WantsToJump
    {
        get => Time.time < wantsToJumpExpireTime;
        set
        {
            if (value)
            {
                wantsToJumpExpireTime = Time.time + JumpParameters.JumpRequestExpireDuration;
            }
            else
            {
                wantsToJumpExpireTime = -1;
            }
        }
    }

    public bool IsJumping => !IsGrounded && Velocity.y > 0;
    public bool IsGrounded => motor.GroundingStatus.IsStableOnGround && !motor.MustUnground();

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

    public void SetMovementInput(in CharacterMovementInputs inputs)
    {
        moveInput = Vector3.zero;
        if (inputs.moveInput != Vector2.zero)
        {
            moveInput = new Vector3(inputs.moveInput.x, 0, inputs.moveInput.y).normalized;
        }

        if (inputs.WantsToJump)
        {
            WantsToJump = true;
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
        if (motor.GroundingStatus.IsStableOnGround)
        {
            var targetVelocity = moveInput * movementParameters.Speed;

            currentVelocity = Vector3.Lerp(
                currentVelocity,
                targetVelocity,
                1 - Mathf.Exp(-movementParameters.Acceleration * deltaTime));

            //instant change if jumping
            if (WantsToJump)
            {
                currentVelocity.y = JumpParameters.CalculateJumpSpeed(Gravity);
                WantsToJump = false;
                //required so KinematicMotor doesn't snap us to the ground
                motor.ForceUnground();
            }
        }
        else
        {
            currentVelocity += Vector3.down * Gravity * deltaTime;
        }
    }

    public bool CheckGround(float checkDistance)
    {
        var pos = motor.TransientPosition;
        var groundingReport = default(CharacterGroundingReport);
        motor.ProbeGround(ref pos, motor.TransientRotation, checkDistance, ref groundingReport);
        return groundingReport.FoundAnyGround;
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