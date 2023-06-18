using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterMovement characterMovement;

    void Update()
    {
        characterMovement.SetMovementInput(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            characterMovement.IsRunning = !characterMovement.IsRunning;
        }
        characterMovement.IsSprinting = Input.GetKey(KeyCode.LeftShift);
    }
}