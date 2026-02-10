using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera aimVirtualCamera;


    private Animator animator;

    private void Awake()
    {
        ThirdPersonController controller = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();

    }

    private bool isAiming;

    private void OnAim(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            isAiming = !isAiming; // toggle
            Aim(isAiming);
        }
    }

    private void Aim(bool aiming)
    {
        aimVirtualCamera.gameObject.SetActive(aiming);

        animator.SetLayerWeight(1, aiming ? 1f : 0f);  // sets animation layer from movement to aiming

        Cursor.visible = aiming;
        Cursor.lockState = aiming ? CursorLockMode.None : CursorLockMode.Locked;


    }

    // TESTING to check mouse cursor movements
    public Vector3 screenPosition; 
    public Vector3 worldPosition;

    private void Update()
    {
        screenPosition = Mouse.current.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

    }

}
