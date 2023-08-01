using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterMovement characterMovement;
    [SerializeField] private CameraController cameraController;
    private void Update()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");
        var wantsToJump = Input.GetKeyDown(KeyCode.Space);
        
        characterMovement.SetInput(new CharacterMovementInput()
        {
            MoveInput = new Vector2(h, v),
            LookRotation = cameraController.LookRotation,
            WantsToJump = wantsToJump
        });

        var lookX = -Input.GetAxisRaw("Mouse Y");
        var lookY = Input.GetAxisRaw("Mouse X");
        cameraController.IncrementLookRotation(new Vector2(lookX, lookY));

    }
}
