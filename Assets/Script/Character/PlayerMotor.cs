using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;



public enum PlayerState {normal, walk, run , crouch}


public class PlayerMotor : MonoBehaviour
{
    public PlayerState playerState;
    [SerializeField] PlayerStatsSO _stats;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private float walkSpeed;
    private float RunSpeed;
    private float jumpPower;
    private float gravity;
    private float lookSpeed;
    private float lookXLimit;
    private float defaultHeight;
    private float crouchHeight;
    private float crouchSpeed;

    //camera
    //[SerializeField] private CinemachineVirtualCamera[] _cam;
    [SerializeField] private Camera _cam;
    private float xRotation = 0f;
    private int CameraIndex = 0;

    private void Awake()
    {
        walkSpeed = _stats.walkSpeed;
        RunSpeed = _stats.RunSpeed;
        jumpPower = _stats.jumpPower;
        gravity = _stats.gravity;
        lookSpeed = _stats.lookSpeed;
        lookXLimit = _stats.lookXLimit;
        defaultHeight = _stats.defaultHeight;
        crouchHeight = _stats.crouchHeight;
        crouchSpeed = _stats.crouchSpeed;
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;

        //calculate camera rotation for up and down
        xRotation -= (mouseY * Time.deltaTime) * lookSpeed;
        xRotation = Mathf.Clamp(xRotation, -lookXLimit, lookXLimit);

        _cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * lookSpeed);
    }

    public void ProcessMove(Vector2 input)
    {
        //Movement Input
        Vector3 movedirection = Vector3.zero;

        movedirection.x = input.x;
        movedirection.z = input.y;
        float move = Mathf.Max(Mathf.Abs(movedirection.x), Mathf.Abs(movedirection.z));

        controller.Move(transform.TransformDirection(movedirection) * walkSpeed * Time.deltaTime);

        //apply Gravity
        playerVelocity.y -= gravity * Time.deltaTime;

        //Set Fall Speed Limit
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }
        //Player Move
        controller.Move(playerVelocity * Time.deltaTime);


    }

    //PlayerRun
    public void Running()
    {

        //kalo true pake RunSpeed, kalo false pake Walkspeed
        if (playerState != PlayerState.run)
        {
            playerState = PlayerState.run;
            walkSpeed = RunSpeed;
        }
        else
        {
            walkSpeed = _stats.walkSpeed;
        }

    }

    //Player Jump
    public void Jump()
    {
        //Kalo Grounded baru bisa lompat
        if (controller.isGrounded)
        {
            playerVelocity.y = jumpPower;
        }

    }

    //Player Crouch
    public void Crouch()
    {
        //Toggle Crouch

        if (playerState != PlayerState.crouch)
        {
            //controller.height = crouchHeight;
            playerState = PlayerState.crouch;
            controller.center = new Vector3(0, -0.25f, 0);
            controller.height = 1.5f;
            walkSpeed = crouchSpeed;
        }
        else
        {
            controller.center = new Vector3(0, 0, 0);
            controller.height = 2f;
            //controller.height = defaultHeight;
            walkSpeed = _stats.walkSpeed;
        }


    }


}
