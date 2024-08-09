﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerScript : MonoBehaviour
{
    [Header("Movement")]
    public float sprintSpeed;
    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeedMultiplier;
    public float crouchYmultiplier;
    public bool IsCrouching;
    float startYscale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.C;

    [Header("Ground check")]
    public LayerMask whatIsGround;
    float playerHeight;
    bool grounded;

    [Header("Slope handler")]
    public float maxSlopeAngle;
    RaycastHit slopeHit;

    [Header("Player info")]

    public float moveSpeed;
    public float speedLimit = 3f;
    public float crouchingSpeed;
    public float airSpeed;
    public Transform orientation;
    public Transform RayCastPos;

    Vector3 moveDirection;
    float verticalInput;
    float horizontalInput;
    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        idle,
        sprinting,
        crouching,
        air,
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.drag = groundDrag;
        readyToJump = true;


        playerHeight = transform.localScale.y;
        startYscale = transform.localScale.y;

        moveSpeed = sprintSpeed;
        crouchingSpeed = moveSpeed * crouchSpeedMultiplier;
        airSpeed = moveSpeed * airMultiplier;

    }


    // Update is called once per frame
    void Update()
    {
        IsGrounded();
        MyInput();
        SpeedControl();
        StateHandler();
        AdjustGravityAndDrag();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void IsGrounded()
    {
        //Ground check
        float rayLength = 0.3f;
        Vector3 rayPos = new Vector3(RayCastPos.transform.position.x, RayCastPos.transform.position.y, RayCastPos.transform.position.z);
        grounded = Physics.Raycast(rayPos, Vector3.down, rayLength, whatIsGround);
        // Debug.DrawRay(rayPos, Vector3.down * rayLength, grounded ? Color.green : Color.red);
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        //On slope
        if (OnSlope())
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed, ForceMode.Acceleration);
        }
        //Crouching
        if(state == MovementState.crouching)
        {
            rb.AddForce(moveDirection.normalized * crouchingSpeed * 3f, ForceMode.Acceleration);
        }
        //On ground
        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Acceleration);
        } 
        //On air
        else if (state == MovementState.air)
        {
            rb.AddForce(moveDirection.normalized * airSpeed, ForceMode.Acceleration);
        }
    }

    private void MyInput()
    {
        //Get keyboard inputs
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        //When to jump
        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded && state != MovementState.crouching)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //Start crouching
        if (Input.GetKeyDown(crouchKey) && state != MovementState.air)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * crouchYmultiplier, transform.localScale.z);
            // rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
            IsCrouching = true;
        }

        //Leave crouching
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYscale, transform.localScale.z);
            IsCrouching = false;
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        //Limit speed if needed
        if (flatVel.magnitude > speedLimit) //if current vel is higher than limit vel
        {
            Vector3 limitedVel = flatVel.normalized * speedLimit; // calculated limitedVel
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z); //applies limitedVel
        }
    }

    private void Jump()
    {
        //Reset y vel
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void StateHandler()
    {
        if (rb.velocity == new Vector3(0, 0, 0) && !IsCrouching)
        {

            state = MovementState.idle;
        }
        else if (IsCrouching && grounded)
        {
            // State: Crouching
            state = MovementState.crouching;
            moveSpeed = crouchingSpeed;
            
            
        }
        else if (grounded)
        {
            // State: Sprinting
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (!grounded)
        {
            // State: Air
            state = MovementState.air;
            moveSpeed = airSpeed;
        }
        
    }

    private bool OnSlope()
    {
        //Checks for a slope
        float rayLength = 0.3f;
        Vector3 rayPos = new Vector3(RayCastPos.transform.position.x, RayCastPos.transform.position.y, RayCastPos.transform.position.z);
        bool IsThereASlope = Physics.Raycast(rayPos, Vector3.down, out slopeHit, rayLength);
        // Debug.DrawRay(rayPos, Vector3.down * rayLength, IsThereASlope ? Color.green : Color.red);


        if (IsThereASlope) 
        {
            //Returns if the slope angle is walkable
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
        //Returns angle/direction of slope
    }

    private void AdjustGravityAndDrag()
    {
        if (rb.velocity.y > 0.1f) // Ascending
        {
            // Reduce gravity and increase drag for slower ascent
            Physics.gravity = new Vector3(0, -9.8f * 0.8f, 0); 
            rb.drag = 1.5f; 
        }
        else if (rb.velocity.y < -0.1f) // Descending
        {
            // Increase gravity and reduce drag for faster descent
            Physics.gravity = new Vector3(0, -9.8f * 2.0f, 0);
            rb.drag = 0.5f; 
        }
        else
        {
            // Reset to default when on the ground or not moving vertically
            Physics.gravity = new Vector3(0, -9.8f, 0); 
            rb.drag = groundDrag; 
        }
    }
}
