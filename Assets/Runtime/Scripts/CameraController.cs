using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    public CharacterMovement CharacterMovement;
    public Transform Target;
    public float TargetHeight = 1.1f;
    public Vector2 lookRotationSpeed = Vector2.one;
    public Vector2 xRotationRange = new(-70, 70);
    private Vector2 targetLook;

    public Quaternion LookRotation => Target.rotation;

    private void LateUpdate()
    {
        Target.transform.position = CharacterMovement.transform.position + Vector3.up*TargetHeight;
        Target.transform.rotation = Quaternion.Euler(targetLook.x, targetLook.y, 0);
    }

    public void IncrementLookRotation(Vector2 lookDelta)
    {
        const float threshold = 0.01f;
        if (lookDelta.sqrMagnitude >= threshold)
        {
            targetLook += lookDelta * lookRotationSpeed;
            targetLook.x = Mathf.Clamp(targetLook.x, xRotationRange.x, xRotationRange.y);
        }
    }
}