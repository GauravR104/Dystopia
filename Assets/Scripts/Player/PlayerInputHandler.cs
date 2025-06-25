using UnityEngine;
using UnityEngine.Events;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Unity Events")]
    public UnityEvent<Vector2> OnMove;
    public UnityEvent OnCrouch;
    public UnityEvent OnJump;
    public UnityEvent<bool> OnAim;
    public UnityEvent<bool> OnShoot;
    public UnityEvent OnReload;

    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();

        // Subscribe to Move
        controls.Gameplay.Move.performed += ctx =>
        {
            Vector2 moveInput = ctx.ReadValue<Vector2>();
            OnMove?.Invoke(moveInput); // Safely invoke OnMove
        };

        controls.Gameplay.Move.canceled += ctx =>
        {
            OnMove?.Invoke(Vector2.zero); // Reset input to zero when movement is canceled
        };

        // Subscribe to Crouch
        controls.Gameplay.Crouch.performed += ctx => OnCrouch.Invoke();

        // Subscribe to Jump
        controls.Gameplay.Jump.performed += ctx => OnJump.Invoke();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
