using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera aimVirtualCamera;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    private Animator animator;

    private const string fireTriggerName = "Firing"; // trigger anim

    public float weaponRange = 100f;

    private void Awake()
    {
        ThirdPersonController controller = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();

    }

    private bool isAiming;
    private bool isFiring;
    private void OnAim(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            isAiming = !isAiming; // toggle
            Aim(isAiming);
        }
    }

    private void OnFire(InputValue value)
    {
        if (value.isPressed)
        {
            TryFire();
        }
    }

    private void Aim(bool aiming)
    {
        aimVirtualCamera.gameObject.SetActive(aiming);

        animator.SetLayerWeight(1, aiming ? 1f : 0f);  // sets animation layer from movement to aiming


    }
    void TryFire()
    {
        RaycastHit hit;
        if(Physics.Raycast(aimVirtualCamera.transform.position, aimVirtualCamera.transform.forward, out hit, weaponRange))
        {
            Debug.Log("Hit: " + hit.transform.name);
        }
        else
        {
            Debug.Log("Miss");
        }
        animator.SetBool(fireTriggerName, !isFiring);
    }
    // TESTING to check mouse cursor movements
    public Vector3 screenPosition; 
    public Vector3 worldPosition;

    private void Update()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        Transform hitTransform = null;

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            hitTransform = raycastHit.transform;
        }

    }

}
