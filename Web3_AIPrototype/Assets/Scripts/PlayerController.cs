using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using UnityEngine.UIElements;

public class PlayerController : NetworkBehaviour
{
    [Networked][OnChangedRender(nameof(ApplyNewModel))] int selectedGender { get; set; }
    [SerializeField] GameObject[] playerModels;
    [SerializeField] PlayerInputManager playerInputManager;

    private void ApplyNewModel()
    {
        for (int i = 0; i < playerModels.Length; i++)
        {
            playerModels[i].SetActive(false);
        }
        playerModels[selectedGender].SetActive(true);
    }
    public override void Spawned()
    {
        base.Spawned();

        // Initialize components
        controller = GetComponent<CharacterController>();


        if (HasStateAuthority)
        {
            playerInputManager = FindObjectOfType<PlayerInputManager>();
            //selectedGender = gET FROM DATABASE

            CommonRefs.Instance.SetPlayer(this);
        }
        ApplyNewModel();

    }





    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float rotationSpeed = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private Camera mainCamera;
    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;
        
        HandleMovement();
        HandleRotation();
        HandleJump();

    }   

    [SerializeField] Vector2 moveAmount;
    private void Update()
    {
        if (playerInputManager == null) { return; }

        moveAmount = playerInputManager.GetMoveAmount();

    }

  

    private void HandleMovement()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small force to keep the player grounded
        }

        // Get input
        float horizontal = moveAmount.x;
        float vertical = moveAmount.y;

        // Calculate movement direction
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        // Move the player
        if (moveDirection.magnitude >= 0.1f)
        {
            controller.Move(moveDirection * moveSpeed * Runner.DeltaTime);
        }

        // Apply gravity
        velocity.y += gravity * Runner.DeltaTime;
        controller.Move(velocity * Runner.DeltaTime);
    }

    private void HandleRotation()
    {
        // Get the camera's forward direction (ignoring vertical rotation)
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        // Calculate the target rotation based on input
        float horizontal = moveAmount.x;
        float vertical = moveAmount.y;

        Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            Vector3 targetDirection = cameraForward * inputDirection.z + mainCamera.transform.right * inputDirection.x;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            // Smoothly rotate the player
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Runner.DeltaTime);
        }
    }

    private void HandleJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }
}
