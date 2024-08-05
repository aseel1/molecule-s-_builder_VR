using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;  // Import the new Input System namespace

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;

    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    public bool canMove = true;

    CharacterController characterController;
    PlayerInput playerInput;  // Reference to the PlayerInput component

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Assuming you have a PlayerInput component attached to the same GameObject
        playerInput = GetComponent<PlayerInput>();

        // Check if playerInput is null
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not found on the GameObject.");
        }
    }

    void Update()
    {
        if (!canMove || playerInput == null) return;

        #region Handles Movement
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Get movement input
        float vertical = playerInput.actions["Move"].ReadValue<Vector2>().y;
        float horizontal = playerInput.actions["Move"].ReadValue<Vector2>().x;

        // Press Left Shift to run
        bool isRunning = Keyboard.current.leftShiftKey.isPressed;
        float curSpeedX = (isRunning ? runSpeed : walkSpeed) * vertical;
        float curSpeedY = (isRunning ? runSpeed : walkSpeed) * horizontal;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        #endregion

        #region Handles Jumping
        if (Keyboard.current.spaceKey.wasPressedThisFrame && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        #endregion

        #region Handles Rotation
        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            rotationX -= mouseDelta.y * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, mouseDelta.x * lookSpeed, 0);
        }
        #endregion
    }
}