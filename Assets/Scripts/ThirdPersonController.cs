using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class ThirdPersonController : MonoBehaviour
{
    private const string speedParamName = "Speed"; // const prevents values being changed from default values
    private const string jumpParamName = "Jump";
    private const string groundedParamName = "Grounded";
    private const string fallingParamName = "Falling";
    private const float lookThreshold = 0.01f;


    [Header("Cinemachine")]
    [SerializeField]
    private Transform cameraTarget;

    [SerializeField]
    private float topClamp = 70.0f;

    [SerializeField]
    private float bottomClamp = -30.0f;

    [Header("Speed")]
    [SerializeField]
    private float lookSpeed = 10f;
    private float movementSpeed = 3f;

    [SerializeField]
    private float turnSpeed = 180f; // sets speed for tank rotation

    [Header("Quickturn")]
    [SerializeField]
    private float quickturnSpeed = 720f;
    private bool isQuickturning = false;
    private bool canQuickturn = true;


    [Header("Aim")]
    [SerializeField]
    private float aimTurnSpeed = 0f;
    private bool isAiming;

    [SerializeField] private float aimBodyAlignSpeed = 20f; // tweak to taste

    [SerializeField] private Transform aimPivot; // assign in Inspector
    public bool IsAiming => isAiming; // with aim script

    private Vector2 look;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 60f;


    [Header("Jump")]
    [SerializeField]
    private float jumpStrength = 7f;

    [SerializeField]
    private float jumpDowntime = 1f;

    [Header("Grounded")]
    [SerializeField]
    private Transform groundCheckPoint;

    [SerializeField]
    private float groundCheckRadius = 0.08f;

    [SerializeField]
    private LayerMask groundLayer;

    private Rigidbody body;
    private Animator animator;
    private Vector2 move;

    private float currentSpeed;
    private float yaw;
    private float pitch;

    private bool isGrounded = true;



    private bool isRunning;
    private bool canJump = true;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        GroundedCheck(); // check immediately

        if (isAiming)
        {
            Look(); // updates yaw/pitch and rotates cameraTarget
            RotateBodyToCameraYaw(); // keep body aligned to the camera yaw
            RotateBodyToCameraPitch();
        }
        else
        {
            // when exiting aiming returns rotation to movement
            yaw = transform.eulerAngles.y;
        }

    }


    private void FixedUpdate()
    {
        Move(); // before rotation
    }

    private void Move()
    {
        float vertical = move.y;     // W/S
        float horizontal = move.x;   // A/D

        // ROTATION (Tank)

        if (!isAiming)
        {
            // tank rotation (movement)
            float currentTurnSpeed = isAiming ? aimTurnSpeed : turnSpeed;
            transform.Rotate(0f, horizontal * currentTurnSpeed * Time.fixedDeltaTime, 0f);
        }


        // RUNNING
        float targetSpeed = (isRunning ? movementSpeed * 2f : movementSpeed) * move.magnitude; // twice fast for running without increase diagonally
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.fixedDeltaTime * 8f);


        // MOVEMENT (Forward/back)
        if (!isAiming)
        {
            Vector3 forward = transform.forward;
            Vector3 velocity = forward * vertical * currentSpeed;
            body.linearVelocity = new Vector3(velocity.x, body.linearVelocity.y, velocity.z);
        }
        else
        {
            // Stop movement while aiming
            body.linearVelocity = new Vector3(0, body.linearVelocity.y, 0);
        }

        // Animator
        float normalizedAnimSpeed = currentSpeed / (movementSpeed * 2f); // update animator
        animator.SetFloat(speedParamName, normalizedAnimSpeed);
        animator.SetBool(fallingParamName, !isGrounded && body.linearVelocity.y < -0.1f);
    }


    private void Jump()
    {
        if (!isGrounded || !canJump)
        {
            return;
        }

        body.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
        canJump = false;
        StartCoroutine(JumpDowntimeCoroutine());

        animator.SetTrigger(jumpParamName);
    }

    private IEnumerator JumpDowntimeCoroutine()
    {
        yield return new WaitForSeconds(0.25f);

        var waitforGrounded = new WaitUntil(() => isGrounded);
        yield return waitforGrounded;

        yield return new WaitForSeconds(jumpDowntime);
        canJump = true;
    }

    private void Look()
    {
        //this moves the camera with player input
        if (look.sqrMagnitude >= lookThreshold)
        {
            float deltaTimeMultiplier = Time.deltaTime * lookSpeed;
            yaw += look.x * deltaTimeMultiplier;
            pitch -= look.y * deltaTimeMultiplier;
        }
        yaw = ClampAngle(yaw, float.MinValue, float.MaxValue);
        pitch = ClampAngle(pitch, bottomClamp, topClamp);


            cameraTarget.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void RotateBodyToCameraYaw()
    {
        // Build a flat (Y-only) rotation from the current yaw we computed for the camera
        Quaternion targetRot = Quaternion.Euler(0f, yaw, 0f);

        // Smoothly rotate the player to match the camera yaw
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            Time.deltaTime * aimBodyAlignSpeed
        );
    }

    private void RotateBodyToCameraPitch()
    {
        if (!aimPivot) return;
        aimPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }


    private float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f)
        {
            lfAngle += 360f;
        }

        if (lfAngle > 360f)
        {
            lfAngle -= 360f;
        }

        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void GroundedCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);
        animator.SetBool(groundedParamName, isGrounded);
    }

    private void OnDrawGizmosSelected() // debug to check if groundcheckpoint is correct
    {
        if (groundCheckPoint == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundCheckPoint.position, groundCheckRadius);
    }

    private void OnMove(InputValue inputValue) // get inputs
    {
        move = inputValue.Get<Vector2>();
    }

    private void OnJump()
    {
        Jump();
    }

    private void OnRun(InputValue inputValue)
    {
        isRunning = inputValue.isPressed;
    }

    private void OnAim(InputValue inputValue)
    {
        if (inputValue.isPressed)
            isAiming = !isAiming;

    }


    private void OnQuickturn(InputValue value)
    {
        if (value.isPressed)
        
            TryQuickturn();
        
    }

    private void TryQuickturn()
    {
        if (canQuickturn && !isQuickturning)
            TryQuickTurn();
    }

    private void TryQuickTurn()
    {

        transform.rotation = Quaternion.Euler(
        0f,
        transform.eulerAngles.y + 180f,
        0f
        );
    }

    private void OnLook(InputValue value)
    {
        look = value.Get<Vector2>();
    }

}
