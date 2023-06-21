using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterMovement characterMovement;

    void Update()
    {
        characterMovement.SetMovementInput(new CharacterMovementInputs()
        {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
            WantsToJump = Input.GetKeyDown(KeyCode.Space)
        });
        
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            characterMovement.IsRunning = !characterMovement.IsRunning;
        }
        characterMovement.IsSprinting = Input.GetKey(KeyCode.LeftShift);
    }
}