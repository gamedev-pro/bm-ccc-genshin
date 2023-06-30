using UnityEngine;

public class HardLandAnimationState : StateMachineBehaviour
{
    private CharacterMovement characterMovement;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.TryGetComponent<AnimatorOwner>(out var owner) &&
            owner.Owner.TryGetComponent(out characterMovement))
        {
            characterMovement.ForceStop();
            characterMovement.DisableMovementFromInput = true;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (characterMovement != null)
        {
            characterMovement.DisableMovementFromInput = false;
        }
    }
}