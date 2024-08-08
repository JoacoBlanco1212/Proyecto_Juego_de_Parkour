﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerScript : MonoBehaviour
{
    [Header("Movement")]
    public float sprintSpeed;
    float moveSpeed;
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

    float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    
    [Header("Player info")]
    public Transform orientation;
    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

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
        readyToJump = true;

        playerHeight = transform.localScale.y;
        startYscale = transform.localScale.y;

        moveSpeed = sprintSpeed;
    }


    // Update is called once per frame
    void Update()
    {
        //Ground check
        float rayLength = playerHeight * 0.5f + 0.3f;
        Vector3 rayCastPos = new Vector3(transform.position.x, transform.position.y + playerHeight * 1.95f, transform.position.z);

        grounded = Physics.Raycast(rayCastPos, Vector3.down, rayLength, whatIsGround);

        Debug.DrawRay(rayCastPos, Vector3.down * rayLength, grounded ? Color.green : Color.red);



        MyInput();
        SpeedControl();
        StateHandler();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if(state == MovementState.crouching)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * crouchSpeedMultiplier, ForceMode.Force);
        }
        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);
        } 
        else if (state == MovementState.air)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * airMultiplier, ForceMode.Force);
        }
    }

    private void MyInput()
    {
        //Get keyboard inputs
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        //When to jump
        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //Start crouching
        if (Input.GetKeyDown(crouchKey))
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
        if (flatVel.magnitude > moveSpeed) //if current vel is higher than limit vel
        {
            Debug.Log("Too much speed");
            Vector3 limitedVel = flatVel.normalized * moveSpeed; // calculated limitedVel
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
        if (rb.velocity == new Vector3(0, 0, 0))
        {

            state = MovementState.idle;
        }
        else if (IsCrouching && grounded)
        {
            // State: Crouching
            state = MovementState.crouching;
            moveSpeed = crouchSpeedMultiplier;
            
            
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
        }
        
    }
}
