using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterMovement characterMovement;
    [SerializeField] private CameraController cameraController;

    void Update()
    {
        var moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        characterMovement.SetMovementInput(new CharacterMovementInputs
        {
            MoveInput = moveInput,
            LookRotation = cameraController.LookRotation,
            WantsToJump = Input.GetKeyDown(KeyCode.Space)
        });

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            characterMovement.IsRunning = !characterMovement.IsRunning;
        }

        characterMovement.IsSprinting = Input.GetKey(KeyCode.LeftShift);

        cameraController.IncrementLookRotation(new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")));
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        Cursor.visible = !hasFocus;
        Cursor.lockState = hasFocus ? CursorLockMode.Locked : CursorLockMode.None;
    }
}