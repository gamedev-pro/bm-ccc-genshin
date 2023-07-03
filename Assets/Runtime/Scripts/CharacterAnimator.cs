using System;
using UnityEngine;

public static class CharacterAnimatorParameters
{
    public static int iMovementMode = Animator.StringToHash("MovementState");
    public static int bIsJumping = Animator.StringToHash("IsJumping");
    public static int bIsGrounded = Animator.StringToHash("IsAnimationGrounded");
    public static int bIsDashing = Animator.StringToHash("IsDashing");
    public static int tHardLand = Animator.StringToHash("HardLand");
}

public class CharacterAnimator : MonoBehaviour
{
    //current movement state given a context (i.e defaul movement mode, swimming, gliding, etc)
    public enum MovementState
    {
        Idle,
        Walk,
        Run,
        Sprint
    }

    //Maybe?
    // public enum MovementMode
    // {
    //     Default,
    //     Gliding,
    //     Swimming
    // }

    public Animator Animator;
    public CharacterMovement CharacterMovement;

    public float GroundCheckDistance = 0.5f;
    public float HardLandYSpeed = 2;

    private void LateUpdate()
    {
        Animator.SetInteger(CharacterAnimatorParameters.iMovementMode, (int)SelectMovementState());
        Animator.SetBool(CharacterAnimatorParameters.bIsJumping, CharacterMovement.IsJumping);
        Animator.SetBool(CharacterAnimatorParameters.bIsDashing, CharacterMovement.IsDashing);

        var isAnimatorGrounded = CharacterMovement.CheckGround(GroundCheckDistance) && !CharacterMovement.IsJumping;
        Animator.SetBool(CharacterAnimatorParameters.bIsGrounded, isAnimatorGrounded);
        

        if (!isAnimatorGrounded && CharacterMovement.Velocity.y < -HardLandYSpeed)
        {
            Animator.SetTrigger(CharacterAnimatorParameters.tHardLand);
        }
    }

    private MovementState SelectMovementState()
    {
        if (CharacterMovement.MoveInput.sqrMagnitude < 0.0001)
        {
            return MovementState.Idle;
        }

        if (CharacterMovement.IsSprinting || CharacterMovement.IsDashing)
        {
            return MovementState.Sprint;
        }

        if (CharacterMovement.IsRunning)
        {
            return MovementState.Run;
        }

        return MovementState.Walk;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(CharacterMovement.transform.position,
            CharacterMovement.transform.position + Vector3.down * GroundCheckDistance);
    }
}