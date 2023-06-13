using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    private bool useExpSmoothing;
    [SerializeField]
    private float expSmoothAcceleration = 25f;
    [SerializeField]
    private float expSmoothRotationSpeed = 20f;


    private void Awake()
    {
        var kinematicMotor = GetComponent<KinematicCharacterMotor>();
        kinematicMotor.CharacterController = this;
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
            if (useExpSmoothing)
            {
                currentRotation = Quaternion.Slerp(currentRotation, targetRot, 1 - Mathf.Exp(-expSmoothRotationSpeed * deltaTime));
            }
            else
            {
                currentRotation = Quaternion.Slerp(currentRotation, targetRot, deltaTime * rotationSpeed);
            }
        }
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        var targetVelocity = moveInput * speed;

        if (useExpSmoothing)
        {

            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, 1 - Mathf.Exp(-expSmoothAcceleration * deltaTime));
        }
        else
        {
            currentVelocity = Vector3.MoveTowards(
                currentVelocity,
                targetVelocity,
                deltaTime * acceleration);
        }
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
