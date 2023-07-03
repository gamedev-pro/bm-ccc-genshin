using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterMovement characterMovement;
    [SerializeField] private CameraController cameraController;

    private InputActions inputActions;

    private void Awake()
    {
        inputActions = new InputActions();
        inputActions.Enable();
    }

    void Update()
    {
        characterMovement.SetMovementInput(new CharacterMovementInputs
        {
            MoveInput = inputActions.Game.Move.ReadValue<Vector2>(),
            LookRotation = cameraController.LookRotation,
            WantsToJump = inputActions.Game.Jump.WasPressedThisFrame()
        });

        if (inputActions.Game.ToggleRun.WasPressedThisFrame())
        {
            characterMovement.IsRunning = !characterMovement.IsRunning;
        }

        characterMovement.IsSprinting = inputActions.Game.Sprint.IsPressed();

        if (inputActions.Game.Sprint.WasPressedThisFrame())
        {
            characterMovement.TryPerformDash();
        }

        var look = inputActions.Game.Look.ReadValue<Vector2>();
        cameraController.IncrementLookRotation(new Vector2(look.y, look.x));
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        Cursor.visible = !hasFocus;
        Cursor.lockState = hasFocus ? CursorLockMode.Locked : CursorLockMode.None;
    }
}