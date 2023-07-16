using KinematicCharacterController;
using UnityEngine;

public struct CharacterMovementInput
{
    public Vector2 MoveInput;
    public bool WantsToJump;
}

[RequireComponent(typeof(KinematicCharacterMotor))]
public class CharacterMovement : MonoBehaviour, ICharacterController
{
    public KinematicCharacterMotor Motor;

    public float MaxSpeed = 5;
    public float Acceleration = 50;
    public float RotationSpeed = 15;
    public float Gravity = 30;
    public float JumpSpeed = 20;
    [Range(0.01f, 0.3f)]
    public float JumpRequestDuration = 0.1f;

    private Vector3 moveInput;
    private float jumpRequestExpireTime;

    private void Awake()
    {
        Motor.CharacterController = this;
    }

    public void SetInput(in CharacterMovementInput input)
    {
        moveInput = Vector3.zero;
        if (input.MoveInput != Vector2.zero)
        {
            moveInput = new Vector3(input.MoveInput.x, 0, input.MoveInput.y).normalized;
        }

        if (input.WantsToJump)
        {
            jumpRequestExpireTime = Time.time + JumpRequestDuration;
        }
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        if (moveInput != Vector3.zero)
        {
            var targetRotation = Quaternion.LookRotation(moveInput);
            currentRotation = Quaternion.Slerp(currentRotation, targetRotation, RotationSpeed * deltaTime);
        }
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (Motor.GroundingStatus.IsStableOnGround)
        {
            var targetVelocity = moveInput * MaxSpeed;
            // currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Acceleration * deltaTime);
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, Acceleration * deltaTime);

            if (Time.time < jumpRequestExpireTime)
            {
                currentVelocity.y = JumpSpeed;
                jumpRequestExpireTime = 0;
                Motor.ForceUnground();
            }
        }
        else
        {
            currentVelocity.y -= Gravity * deltaTime;
        }
    }

    #region not implemented

    public void BeforeCharacterUpdate(float deltaTime)
    {
    }

    public void PostGroundingUpdate(float deltaTime)
    {
    }

    public void AfterCharacterUpdate(float deltaTime)
    {
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
        ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
        ref HitStabilityReport hitStabilityReport)
    {
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
        Vector3 atCharacterPosition,
        Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
    }

    #endregion
}