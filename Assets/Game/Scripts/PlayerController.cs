using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterMovement characterMovement;
    private void Update()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");
        
        characterMovement.SetInput(new CharacterMovementInput()
        {
            MoveInput = new Vector2(h, v)
        });
    }
}
