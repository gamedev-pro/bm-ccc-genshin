using UnityEngine;

public static class CharacterAnimatorParameters
{
    public static int iMovementMode = Animator.StringToHash("MovementState");
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

    private void LateUpdate()
    {
        Animator.SetInteger(CharacterAnimatorParameters.iMovementMode, (int)SelectMovementState());
    }

    private MovementState SelectMovementState()
    {
        if (CharacterMovement.MoveInput.sqrMagnitude < 0.0001)
        {
            return MovementState.Idle;
        }
        if (CharacterMovement.IsSprinting)
        {
            return MovementState.Sprint;
        }
        if (CharacterMovement.IsRunning)
        {
            return MovementState.Run;
        }
        return MovementState.Walk;
    }
    
}