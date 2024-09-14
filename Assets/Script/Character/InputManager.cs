using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
public class InputManager : NetworkBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;
    private PlayerMotor motor;

    private void Awake()
    {
        motor = GetComponent<PlayerMotor>();
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        onFoot.Run.performed += ctx => motor.Running();
        onFoot.Jump.performed += ctx => motor.Jump();
        onFoot.Crouch.performed += ctx => motor.Crouch();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }

    private void LateUpdate()
    {
        if (!IsOwner) return;
        motor.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
    }
}
