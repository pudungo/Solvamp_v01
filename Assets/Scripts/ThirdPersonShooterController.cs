using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera aimVirtualCamera;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    private Animator animator;
    [SerializeField] private Animator platformAnimator;


    private const string fireTriggerName = "Firing"; // trigger anim

    public float weaponRange = 100f;

    private float aimLayerWeight = 0f;

    private Health health;

    private void Awake()
    {
        ThirdPersonController controller = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();

    }

    private bool isAiming;
    private bool isFiring;
    private void OnAim(InputValue inputValue)
    {
        if (health.IsDead)
            return;

        if (inputValue.isPressed)
        {
            isAiming = !isAiming; // toggle
            Aim(isAiming);

        }
    }

    private void OnFire(InputValue value)
    {
        if (health.IsDead)
            return;


        if (value.isPressed)
        {
            TryFire();
        }
    }

    private void Aim(bool aiming)
    {
        aimVirtualCamera.gameObject.SetActive(aiming);


    }
    void TryFire()
    {
        RaycastHit hit;
        if(Physics.Raycast(aimVirtualCamera.transform.position, aimVirtualCamera.transform.forward, out hit, weaponRange))
        {
            Debug.Log("Hit: " + hit.transform.name);

            // Check if the hit object is a freezable platform
            FreezablePlatform platform = hit.transform.GetComponent<FreezablePlatform>();
            if (platform != null)
            {
                platform.ToggleFreeze();
            }

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
        // Death no aim input
        if (health.IsDead)
        {
            // Force aiming off
            isAiming = false;
            aimVirtualCamera.gameObject.SetActive(false);

            // Force aim layer to zero
            animator.SetLayerWeight(1, 0f);
            return;
        }

        // Aim animation
        float targetWeight = isAiming ? 1f : 0f;

         // Smooth fade (tweak 8f for faster/slower blending)
        aimLayerWeight = Mathf.Lerp(aimLayerWeight, targetWeight, Time.deltaTime * 15f);

        animator.SetLayerWeight(1, aimLayerWeight);

        // raycast
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
