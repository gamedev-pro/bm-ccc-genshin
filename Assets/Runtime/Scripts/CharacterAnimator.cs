using UnityEngine;

public static class CharacterAnimatorParameters
{
    public static int bIsRunning = Animator.StringToHash("IsRunning");
}

public class CharacterAnimator : MonoBehaviour
{
    public Animator Animator;
    public CharacterMovement CharacterMovement;

    private void LateUpdate()
    {
        Animator.SetBool(CharacterAnimatorParameters.bIsRunning, CharacterMovement.MoveInput.sqrMagnitude > 0);
    }
}