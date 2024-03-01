using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Move Parameters")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float walkSmoothness = 10f;
    [SerializeField] private float runSpeedModifier = 1.3f;
    [SerializeField] private float crouchSpeedModifier = 0.5f;

    [Header("Air Parameters")]
    [SerializeField] private float airDragTime = 0.7f;
    [SerializeField] private float airControl = 0.7f;

    [Header("Jump And Gravity")]
    [SerializeField] private float jumpForce = 1.5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Crouching Settings")]
    [SerializeField] private float crouchHeight = 1.5f;
    [SerializeField] private float crouchHeightOffset = -0.25f;
    [SerializeField] private float timeToChangeHeight = 0.2f;
    [SerializeField] private Transform crouchRayPoint;

    private float currentSpeed;
    private float playerHeight;
    private float vel1, vel2;
    private bool canStandUp;
    private CharacterController controller;
    private Vector3 velocity;
    private WalkState walkState;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = walkSpeed;
        playerHeight = controller.height;
    }
    private void Update()
    {
        Movement();
        PlayerInput();
        CanStandupCheker();
    }
    void Movement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 move = (transform.right * x + transform.forward * z).normalized;

        currentSpeed = walkState switch //Change your speed by current State
        {
            WalkState.Walking => walkSpeed,
            WalkState.Running => walkSpeed * runSpeedModifier,
            WalkState.Crouching => walkSpeed * crouchSpeedModifier,
        };

        if(controller.isGrounded)
        {
            Vector3 moveVector = move * currentSpeed;
            velocity.x = Mathf.Lerp(velocity.x, moveVector.x, walkSmoothness * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, moveVector.z, walkSmoothness * Time.deltaTime);
        }
        else
        {
            Vector3 moveVector = move * currentSpeed * airControl;
            velocity.x = Mathf.Lerp(velocity.x, moveVector.x, airDragTime * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, moveVector.z, airDragTime * Time.deltaTime);
        }
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }
    void PlayerInput()
    {
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
        if (Input.GetKey(KeyCode.LeftShift) && controller.isGrounded 
            && walkState != WalkState.Crouching && canStandUp)
        {
            //You are can running only on the ground!
            walkState = WalkState.Running;
        }
        else if (Input.GetKey(KeyCode.C) && walkState != WalkState.Running)
        {
            walkState = WalkState.Crouching; 
        }
        else if (canStandUp)
        {
            walkState = WalkState.Walking;
        }
        switch (walkState)
        {
            case WalkState.Walking:
                controller.height = Mathf.SmoothDamp(controller.height, playerHeight, ref vel1, timeToChangeHeight);
                controller.center = new Vector3(0, Mathf.SmoothDamp(controller.center.y, 0, ref vel2, timeToChangeHeight), 0);
                //Disable your crouching Animation here
                break;
            case WalkState.Running:
                controller.height = Mathf.SmoothDamp(controller.height, playerHeight, ref vel1, timeToChangeHeight);
                controller.center = new Vector3(0, Mathf.SmoothDamp(controller.center.y, 0, ref vel2, timeToChangeHeight), 0);
                //Disable your crouching Animation here
                break;
            case WalkState.Crouching:
                controller.height = Mathf.SmoothDamp(controller.height, crouchHeight, ref vel1, timeToChangeHeight);
                controller.center = new Vector3(0, Mathf.SmoothDamp(controller.center.y, crouchHeightOffset, ref vel2, timeToChangeHeight), 0);
                //Enable your crouching Animation here
                break;
        }
    }
    void CanStandupCheker()
    {
        float rayDistance = playerHeight * 0.5f + controller.radius * 0.1f;
        if (Physics.SphereCast(crouchRayPoint.position, controller.radius * 0.9f, transform.up, out RaycastHit hit, rayDistance))
            canStandUp = false;
        else
            canStandUp = true;
    }
    enum WalkState //Player States
    {
        Walking, Running, Crouching
    }
}
