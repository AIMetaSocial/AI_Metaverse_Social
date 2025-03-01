using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using UnityEngine.UI;
using TMPro;
using Thirdweb;

public class PlayerController : NetworkBehaviour
{
    [Networked][OnChangedRender(nameof(ApplyNewModel))]public int selectedGender { get; set; }
    [Networked][OnChangedRender(nameof(ChangedName))]public string playerName { get; set; }
    [Networked] public string playerAddress { get; set; }
    [SerializeField] GameObject[] playerModels;
    [SerializeField] PlayerInputManager playerInputManager;
    [SerializeField] bool isSpawned = false;
    [SerializeField] Animator animator;
    [SerializeField] PlayerState playerState = PlayerState.IDLE;
    [Networked] public bool inFight {get; set;}

    [SerializeField] TMP_Text nameText;
    [SerializeField] GameObject challengeUI;
    [SerializeField] GameObject healthbarUI;
    [SerializeField] int maxHealth; 
    [SerializeField] GameObject[] weaponObjects;
    [Networked][OnChangedRender(nameof(HandleHealthChange))]public int currentHealth{get; private set;}
    [SerializeField] Image playerHealthBar;
    [SerializeField] TMP_Text playerHealth;
    [SerializeField] GameObject hitArea;
    void OnEnable()
    {
        EditProfilePanel.OnProfileEdited += HandleProfileChange;
    }

    private void HandleProfileChange()
    {   
        if(HasStateAuthority){
            LocalData data = DatabaseManager.Instance.GetLocalData();
            selectedGender = data.selectedGender;            
            playerName =  data.playername;
        }
    }

    void OnDisable()
    {
        EditProfilePanel.OnProfileEdited -= HandleProfileChange;
    }

    private void ChangedName()
    {
        nameText.text = playerName.ToString();
    }
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
            nameText.gameObject.SetActive(false);
            LocalData data = DatabaseManager.Instance.GetLocalData();
            
            selectedGender = data.selectedGender;            
            playerName =  data.playername;
            playerAddress = LoginManager.address;

            CommonRefs.Instance.SetPlayer(this);

            AIChatManager.OnChatToggleEvent += HandleChatToggle;
            Destroy(hitArea);
        }

        nameText.text = playerName;
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

        if(isSpawned && playerState == PlayerState.IDLE){
            //GeT INPUT FOR MOUSE CLICK
            bool pressedMouseClick = playerInputManager.GetFireClick();
            if(pressedMouseClick){
                if(inFight){
                    Attack(); 
                }
                else{                
                    CheckForChallengeRaycast();
                }
            }
        }


        moveAmount = playerInputManager.GetMoveAmount();
        jumpPressed = playerInputManager.GetJumpButton();
        if(!lastJumpPressed && jumpPressed && isGrounded && playerState == PlayerState.IDLE){
           lastJumpPressed =true;
        }

    }

    [SerializeField] bool isAttacking = false;
    [SerializeField] float attackStartDelay=1f;
    [SerializeField] float attackEndDelay=1f;
    
    private void Attack()
    {
        if(isAttacking) return;

        isAttacking = true;
        Debug.Log("Attack");
        StartCoroutine(attack());
    }
    IEnumerator attack(){
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(attackStartDelay);
        weaponObjects[selectedGender].GetComponent<Collider>().enabled =true;
        yield return new WaitForSeconds(attackEndDelay);
        weaponObjects[selectedGender].GetComponent<Collider>().enabled =false;
        isAttacking = false;
    }

    [SerializeField] LayerMask challengeLayer;
    private void CheckForChallengeRaycast()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray,out RaycastHit hit,Mathf.Infinity,challengeLayer)){
            if(hit.transform.parent.TryGetComponent<PlayerController>(out PlayerController pc)){
                if(!inFight && !pc.inFight)
                pc.SendBattleRequest();
            }
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
            if(!inFight){
                GameUI.Instance?.PlayerInteractionWithAIPlayer(aI_GeneratorPerson,true);
            }
        }
        else if(other.CompareTag("TriggerArea") && other.transform.parent.TryGetComponent<PlayerController>(out PlayerController pc)){
            if(!HasStateAuthority && !pc.inFight && !inFight && pc!= this){
                Debug.Log("Entering player Trigger");
                challengeUI.SetActive(true);
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.transform.TryGetComponent<AI_GeneratorPerson>(out AI_GeneratorPerson aI_GeneratorPerson)){
            GameUI.Instance?.PlayerInteractionWithAIPlayer(aI_GeneratorPerson,false);
        }
        else if(other.CompareTag("TriggerArea")){ 
                Debug.Log("Exiting player Trigger");
                challengeUI.SetActive(false);            
        }
    }


    #region Battle
    public void SendBattleRequest(){
        challengeUI.SetActive(false);
        if(!inFight){
            Rpc_SendBattleRequest(Runner.LocalPlayer,playerName);
        }
        GameUI.Instance?.PlayerInteractionWithAIPlayer(null,false);
    }

    [Rpc(RpcSources.All,RpcTargets.StateAuthority)]
    public void Rpc_SendBattleRequest(PlayerRef localPlayer, string playerName)
    {   
        if(inFight) return;

        Debug.Log("Got Battle Request From player Id : "  + localPlayer.PlayerId + " playername : " + playerName);
        GameUI.Instance?.ShowIncomingChallengeUI(localPlayer,playerName);
        GameUI.Instance?.PlayerInteractionWithAIPlayer(null,false);
    }


    [SerializeField] NetworkObject hitParticle;
     public void GotDamage(int damage, Vector3 hitposition)
    {
        if(HasStateAuthority){
            if(currentHealth>0){
                Runner.Spawn(hitParticle,hitposition,Quaternion.identity);
                currentHealth -= damage;
                if (currentHealth <= 0)
                {
                    Died();
                }
            }
        }
        else{
            Rpc_GotDamage(damage,hitposition);
        }
    }
    [Rpc(RpcSources.All,RpcTargets.StateAuthority)]
    public void Rpc_GotDamage(int _damage,Vector3 hitPosition){
        if(currentHealth>0){
            Runner.Spawn(hitParticle,hitPosition,Quaternion.identity);
            currentHealth -= _damage;
            if(currentHealth<=0){
                Died();
            }
        }
    }

    private void Died()
    {
        BattleManager.Instance?.EndBattle(GameEndReason.LOSE,true);
    }

    #endregion

    private void LateUpdate() {
        if(mainCamera==null) return;

        Vector3 camDirection = mainCamera.transform.forward;
        camDirection.y =0;
        camDirection.Normalize();

        healthbarUI.transform.rotation = 
        challengeUI.transform.rotation = 
        nameText.transform.rotation = Quaternion.LookRotation(camDirection);
    }

    internal void StartBattle()
    {       
        if(HasStateAuthority){
           currentHealth = maxHealth;
           StartCoroutine(enableFightBool());

        }
        challengeUI.SetActive(false);
        healthbarUI.SetActive(true);
        UpdateHealthBar();
        weaponObjects[selectedGender].SetActive(true);       

    }
    IEnumerator enableFightBool(){
        yield return new WaitForEndOfFrame();
        inFight = true;
    }

     internal void EndBattle()
    {
        if(HasStateAuthority){           
            inFight = false;
        }

        healthbarUI.SetActive(false);        
        weaponObjects[selectedGender].SetActive(false);     
    }
     private void HandleHealthChange()
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
       playerHealthBar.fillAmount = (float)currentHealth/(float)maxHealth;
       playerHealth.text = currentHealth.ToString();
    }

   
}

public enum PlayerState{
    IDLE,CHATING
}