using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{   
    private PlayerInputActions inputActions;
    private void Awake() {
        inputActions = new PlayerInputActions();
        inputActions.Enable();
    }
    private void OnDisable() {
        if(inputActions!=null)
        {
            inputActions.Disable();
        }
    }
    internal Vector2 GetMoveAmount()
    {
        Vector2 move = Vector2.zero;
        move = inputActions.Player.Move.ReadValue<Vector2>();
        return move;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal bool GetJumpButton()
    {
       return inputActions.Player.Jump.WasPressedThisFrame();
    }
}
