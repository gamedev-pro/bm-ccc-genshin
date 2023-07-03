using System;
using System.Collections;
using KinematicCharacterController;
using UnityEngine;

public struct CharacterMovementInputs
{
    public Vector2 MoveInput;
    public Quaternion LookRotation;
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

    [Min(0.03f)] public float JumpRequestExpire;

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
    public bool IsDashing;

    public float Gravity = 30;
    public MovementParameters WalkParameters;
    public MovementParameters RunParameters;
    public MovementParameters SprintParameters;
    public JumpParameters JumpParameters;
    public float DashDuration = 0.2f;
    public MovementParameters DashParameters;

    private Vector3 moveInput;
    private float wantsToJumpExpireTime;
    private float jumpEndTime;

    public bool DisableMovementFromInput;

    public bool WantsToJump
    {
        get => Time.time < wantsToJumpExpireTime;
        set
        {
            if (value)
            {
                wantsToJumpExpireTime = Time.time + JumpParameters.JumpRequestExpire;
            }
            else
            {
                wantsToJumpExpireTime = -1;
            }
        }
    }

    public bool IsJumping => !IsGrounded && Time.time < jumpEndTime;
    public bool IsGrounded => motor.GroundingStatus.IsStableOnGround && !motor.MustUnground();

    [NonSerialized] private KinematicCharacterMotor motor;

    public Vector3 MoveInput => moveInput;
    public Vector3 Velocity => motor.Velocity;

    public MovementParameters CurrentMovementParameters
    {
        get
        {
            if (IsDashing)
            {
                return DashParameters;
            }
            
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
        if (DisableMovementFromInput)
        {
            return;
        }
        
        moveInput = Vector3.zero;

        if (inputs.MoveInput != Vector2.zero)
        {
            moveInput = new Vector3(inputs.MoveInput.x, 0, inputs.MoveInput.y).normalized;
            moveInput = motor.GetDirectionTangentToSurface(inputs.LookRotation * moveInput, Vector3.up);
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
            var projectedInput = motor.GetDirectionTangentToSurface(moveInput, motor.GroundingStatus.GroundNormal);
            var targetVelocity = projectedInput * movementParameters.Speed;

            currentVelocity = Vector3.Lerp(
                currentVelocity,
                targetVelocity,
                1 - Mathf.Exp(-movementParameters.Acceleration * deltaTime));

            //instant change if jumping
            if (WantsToJump)
            {
                currentVelocity.y = JumpParameters.CalculateJumpSpeed(Gravity);
                jumpEndTime = Time.time + JumpParameters.TimeToApex;
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

    public void ForceStop()
    {
        moveInput = Vector3.zero;
        motor.BaseVelocity.x = 0;
        motor.BaseVelocity.z = 0;
    }

    public bool CheckGround(float checkDistance)
    {
        var pos = motor.TransientPosition;
        var groundingReport = default(CharacterGroundingReport);
        motor.ProbeGround(ref pos, motor.TransientRotation, checkDistance, ref groundingReport);
        return groundingReport.FoundAnyGround;
    }

    public void TryPerformDash()
    {
        if (!IsDashing)
        {
            IsDashing = true;
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        var dashEndTime = Time.time + DashDuration;
        DisableMovementFromInput = true;

        var input = transform.forward;

        while (Time.time < dashEndTime)
        {
            moveInput = input;
            yield return null;
        }
        
        DisableMovementFromInput = false;
        IsDashing = false;
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