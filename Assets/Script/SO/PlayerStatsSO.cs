using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerStats", menuName = "Stats")]
public class PlayerStatsSO : ScriptableObject
{
    [Header("Player Movement")]
    [Tooltip("Player Walk Speed")]
    public float walkSpeed = 6f;

    [Tooltip("Player Run Speed")]
    public float RunSpeed = 10f;

    [Tooltip("The immediate velocity applied when jumping")]
    public float jumpPower = 7f;

    [Tooltip("Gravity applied to player")]
    public float gravity = 10f;

    [Tooltip("Player Camera look Speed")]
    public float lookSpeed = 2f;

    [Tooltip("Camera rotation Limit")]
    public float lookXLimit = 45f;

    [Tooltip("Player Default height")]
    public float defaultHeight = 2f;

    [Tooltip("Player Height when crouching")]
    public float crouchHeight = 1f;

    [Tooltip("Speed when player crouch")]
    public float crouchSpeed = 3f;

}
