using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterMovement characterMovement;
    [SerializeField] private CameraController cameraController;

    [Header("Mobile")]
    [SerializeField] private bool useMobile;
    [SerializeField] private GameObject mobileCanvas;

    private InputActions inputActions;

    private void Awake()
    {
        inputActions = new InputActions();
        inputActions.Enable();
        RefreshOnScreenControls();
    }

    private void OnValidate()
    {
        RefreshOnScreenControls();
    }

    private void RefreshOnScreenControls()
    {
        mobileCanvas.SetActive(useMobile);
        Cursor.visible = useMobile;
    }

    private void Update()
    {
        var moveInput = inputActions.Game.Move.ReadValue<Vector2>();
        var wantsToJump = inputActions.Game.Jump.WasPressedThisFrame();
        
        characterMovement.SetInput(new CharacterMovementInput()
        {
            MoveInput = moveInput,
            LookRotation = cameraController.LookRotation,
            WantsToJump = wantsToJump
        });

        var look = inputActions.Game.Look.ReadValue<Vector2>();
        cameraController.IncrementLookRotation(new Vector2(look.y, look.x));
    }
}
