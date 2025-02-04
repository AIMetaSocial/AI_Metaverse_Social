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
    [SerializeField] bool isSpawned = false;
    [SerializeField] Animator animator;
    [SerializeField] PlayerState playerState = PlayerState.IDLE;
    

    private void ApplyNewModel()
    {
        for (int i = 0; i < playerModels.Length; i++)
        {
            playerModels[i].SetActive(false);
        }
        playerModels[selectedGender].SetActive(true);
        animator = playerModels[selectedGender].GetComponent<Animator>();
    }
    public override void Spawned()
    {
        base.Spawned();
        isSpawned = true;

        // Initialize components
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;


        if (HasStateAuthority)
        {
            playerInputManager = FindObjectOfType<PlayerInputManager>();
            //selectedGender = gET FROM DATABASE

            CommonRefs.Instance.SetPlayer(this);

            AIChatManager.OnChatToggleEvent += HandleChatToggle;
        }
        ApplyNewModel();

    }
    private void OnDestroy() {
        if(HasStateAuthority){
            AIChatManager.OnChatToggleEvent -= HandleChatToggle;
        }
    }

    private void HandleChatToggle(bool _chatOn)
    {
        if(_chatOn){
            playerState = PlayerState.CHATING;
        }
        else{
            playerState = PlayerState.IDLE;
        }
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
    [SerializeField]private bool isGrounded;
    private Camera mainCamera;
    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;
        
        HandleMovement();
        HandleRotation();
        HandleJump();
    }   

    [SerializeField] Vector2 moveAmount;
    [SerializeField] bool jumpPressed;
    [SerializeField] bool lastJumpPressed;
    private void Update()
    {
        if (playerInputManager == null) { return; }

        moveAmount = playerInputManager.GetMoveAmount();
        jumpPressed = playerInputManager.GetJumpButton();
        if(!lastJumpPressed && jumpPressed && isGrounded){
           lastJumpPressed =true;
        }

    }

  

    private void HandleMovement()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -1f; // Small force to keep the player grounded
        }


        float horizontal = 0;
        float vertical = 0;

        // Get input
        if(playerState == PlayerState.IDLE){
            horizontal = moveAmount.x;
            vertical = moveAmount.y;
        }

          // Calculate movement direction relative to the camera
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        // Ignore vertical component of the camera's forward and right vectors
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate movement direction based on input and camera orientation
        Vector3 moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;


        animator.SetFloat("Velocity",moveDirection.magnitude);

        // Move the player
        if (moveDirection.magnitude >= 0.1f)
        {
            controller.Move(moveDirection * moveSpeed * Runner.DeltaTime);
        }

        // Apply gravity
        velocity.y += gravity * Runner.DeltaTime;
        if(velocity.y<-10f){
            velocity.y = -10f;
        }

        controller.Move(velocity * Runner.DeltaTime);

        


    }

    private void HandleRotation()
    {   
        if(playerState != PlayerState.IDLE) return;
        
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
        if (isGrounded && lastJumpPressed)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            animator.SetTrigger("Jump");
            lastJumpPressed = false;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.transform.TryGetComponent<AI_GeneratorPerson>(out AI_GeneratorPerson aI_GeneratorPerson)){
            GameUI.Instance?.PlayerInteractionWithAIPlayer(aI_GeneratorPerson,true);
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.transform.TryGetComponent<AI_GeneratorPerson>(out AI_GeneratorPerson aI_GeneratorPerson)){
            GameUI.Instance?.PlayerInteractionWithAIPlayer(aI_GeneratorPerson,false);
        }
    }
}

public enum PlayerState{
    IDLE,CHATING
}