using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void Update()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");
        
        Debug.Log($"{h}, {v}");
    }
}
