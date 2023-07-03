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

[Serializable]
public struct AirborneParameters
{
    public float Gravity;
    public float MaxFallSpeed;
    public float MaxSpeedXZ;
    public float Acceleration;
    [Range(0, 1)]
    public float Drag;
}

[RequireComponent(typeof(KinematicCharacterMotor))]
public class CharacterMovement : MonoBehaviour, ICharacterController
{
    public bool IsRunning;
    public bool IsSprinting;
    public bool IsDashing;

    [Header("Ground Movement")] public MovementParameters WalkParameters;
    public MovementParameters RunParameters;
    public MovementParameters SprintParameters;
    public JumpParameters JumpParameters;

    public float DashDuration = 0.2f;
    public MovementParameters DashParameters;

    [Header("Airborne")] public AirborneParameters DefaultAirborneParameters;
    public AirborneParameters GlideParameters;

    private Vector3 moveInput;
    private float wantsToJumpExpireTime;
    private float jumpEndTime;

    public bool DisableMovementFromInput;

    public bool WantsToGlide { get; private set; }
    public bool IsGliding => WantsToGlide && !IsGrounded;

    public AirborneParameters CurrentAirborneParameters
    {
        get
        {
            if (IsGliding)
            {
                return GlideParameters;
            }

            return DefaultAirborneParameters;
        }
    }

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
        var airborneParameters = CurrentAirborneParameters;
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
                currentVelocity.y = JumpParameters.CalculateJumpSpeed(airborneParameters.Gravity);
                jumpEndTime = Time.time + JumpParameters.TimeToApex;
                WantsToJump = false;
                //required so KinematicMotor doesn't snap us to the ground
                motor.ForceUnground();
            }

            WantsToGlide = false;
        }
        else
        {
            var targetVelocityXZ = new Vector2(moveInput.x, moveInput.z) * airborneParameters.MaxSpeedXZ;
            var currentVelocityXZ = new Vector2(currentVelocity.x, currentVelocity.z);

            currentVelocityXZ = Vector2.Lerp(
                currentVelocityXZ,
                targetVelocityXZ,
                1 - Mathf.Exp(-airborneParameters.Acceleration * deltaTime));

            var vy = Mathf.MoveTowards(currentVelocity.y, -airborneParameters.MaxFallSpeed,
                airborneParameters.Gravity * deltaTime);

            currentVelocity.x = currentVelocityXZ.x;
            currentVelocity.y = vy;
            currentVelocity.z = currentVelocityXZ.y;
            
            ApplyDrag(ref currentVelocity.x, airborneParameters.Drag, deltaTime);
            ApplyDrag(ref currentVelocity.y, airborneParameters.Drag, deltaTime);
            ApplyDrag(ref currentVelocity.z, airborneParameters.Drag, deltaTime);
        }
    }

    private void ApplyDrag(ref float n, float drag, float deltaTime)
    {
        n *= 1f / (1f + drag * deltaTime);
    }

    public void ToggleWantsToGlide()
    {
        WantsToGlide = !WantsToGlide;
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