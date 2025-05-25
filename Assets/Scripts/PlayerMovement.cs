using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float runSpeed = 6f;
    public float crouchSpeed = 1f;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    public float turnThresholdAngle = 90f;
    public float rotationSpeed = 5f;

    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchTransitionTime = 0.2f;

    [Header("Components")]
    public Animator animator;

    private CharacterController controller;
    private CapsuleCollider capsuleCollider;

    private Transform cameraTransform;
    public float distanceFromCamera = 10f;

    private Vector3 velocity;
    private bool isCrouching = false;

    private Vector2 inputDirection;
    private float crouchStartTime;
    float crouchBlend = 0f;
    private void OnEnable()
    {
        PlayerInputHandler input = FindAnyObjectByType<PlayerInputHandler>();
        input.OnMove.AddListener(HandleMove);
        input.OnCrouch.AddListener(ToggleCrouch);
    }

    private void OnDisable()
    {
        PlayerInputHandler input = FindAnyObjectByType<PlayerInputHandler>();
        input.OnMove.RemoveListener(HandleMove);
        input.OnCrouch.RemoveListener(ToggleCrouch);
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        float targetCrouch = isCrouching ? 0.75f : 0f;
        crouchBlend = Mathf.MoveTowards(crouchBlend, targetCrouch, Time.deltaTime * 5f); // adjust speed as needed
        //animator.SetFloat("Crouch", crouchBlend);

        if (isCrouching)
        {
            float crouchDuration = Time.time - crouchStartTime;
            crouchStartTime = Time.time; // Reset crouch start time
        }
    }


    private void HandleMove(Vector2 input)
    {
        inputDirection = new Vector2(input.x, input.y);
    }

    private void Move()
    {
        Vector3 direction = new Vector3(inputDirection.x, 0f, inputDirection.y).normalized;

        if (direction.magnitude >= 0.1f)
        {
            Vector3 moveDir = HandleNormalMovement(direction);
            float speed = inputDirection.magnitude * (isCrouching ? crouchSpeed : runSpeed);
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }


        //animator.SetFloat("Speed", inputDirection.magnitude, 0.05f, Time.deltaTime);

        // Gravity
        if (!controller.isGrounded)
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
        else
        {
            velocity.y = 0;
        }
    }


    private Vector3 HandleNormalMovement(Vector3 direction)
    {
        Vector3 moveDir;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        return moveDir;
    }

    private void ToggleCrouch()
    {
        isCrouching = !isCrouching;


        //  animator.SetFloat("Crouch", smoothCrouch);


        // Adjust Character Controller Height
        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        StartCoroutine(AdjustHeight(targetHeight));
    }

    private IEnumerator AdjustHeight(float targetHeight)
    {
        float currentHeight = controller.height;
        float currentCenterY = controller.center.y;

        float elapsed = 0f;

        float targetCenterY = currentCenterY - (currentHeight - targetHeight) / 2f;


        while (elapsed < crouchTransitionTime)
        {
            elapsed += Time.deltaTime;
            controller.height = Mathf.Lerp(currentHeight, targetHeight, elapsed / crouchTransitionTime);
            controller.center = new Vector3(controller.center.x, Mathf.Lerp(currentCenterY, targetCenterY, elapsed / crouchTransitionTime), controller.center.z);

            capsuleCollider.height = controller.height;
            capsuleCollider.center = controller.center;

            yield return null;
        }

        controller.height = targetHeight;
        controller.center = new Vector3(controller.center.x, targetCenterY, controller.center.z);

        capsuleCollider.height = controller.height;
        capsuleCollider.center = controller.center;
    }

    public void Die()
    {
       // animator.SetTrigger("Dead");
        GetComponent<Collider>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
    }
}
