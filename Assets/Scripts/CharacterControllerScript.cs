using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerScript : MonoBehaviour
{
    [Header("Sprinting")]
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

    [Header("Sliding")]
    public float slideTime = 0.5f;
    public float slideYmultiplier;
    public float slideSpeedRequirementMultiplier;
    public float slideSpeedMultiplier;
    public bool IsSliding;

    [Header("Wallrunning")]
    public bool isWallRun;
    public float wallRunSpeedMultiplier;
    public LayerMask whatIsWall;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("Wall detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    public RaycastHit leftWallHit;
    public RaycastHit rightWallHit;
    public bool wallLeft;
    public bool wallRight;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.C;
    public KeyCode slideKey = KeyCode.LeftControl;

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
    public float slideSpeed;
    public float slideSpeedRequirement;
    public float wallRunSpeed;
    public Transform orientation;
    public Transform groundRayCastPos;
    public Transform wallRayCastPos;

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
        sliding,
        wallRunning,

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
        slideSpeed = moveSpeed * slideSpeedMultiplier;
        slideSpeedRequirement = speedLimit * slideSpeedRequirementMultiplier;

        wallRunSpeed = moveSpeed * wallRunSpeedMultiplier;
    }


    // Update is called once per frame
    void Update()
    {
        IsGrounded();
        MyInput();
        SpeedControl();
        StateHandler();
        AdjustGravityAndDrag();

        CheckForWall();
        whenToWallRun();
    }

    private void FixedUpdate()
    {
        MovePlayer();

        if (isWallRun)
        {
            WallRunningMovement();
        }
    }

    private void IsGrounded()
    {
        //Ground check
        float rayLength = 0.3f;
        Vector3 rayPos = new Vector3(groundRayCastPos.transform.position.x, groundRayCastPos.transform.position.y, groundRayCastPos.transform.position.z);
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
            rb.AddForce(moveDirection.normalized * crouchingSpeed, ForceMode.Acceleration);
        }
        //Sliding
        if (state == MovementState.sliding)
        {
            rb.AddForce(moveDirection.normalized * slideSpeed, ForceMode.Acceleration);
        }
        //On ground
        if (grounded)
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
        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded && (state != MovementState.crouching && state != MovementState.sliding))
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

        //Start sliding
        if (Input.GetKeyDown(slideKey) && CanSlide())
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * slideYmultiplier, transform.localScale.z);
            IsSliding = true;
            StartCoroutine(StartSlideCountdown());
        }
        //Leave sliding
        if (Input.GetKeyUp(slideKey))
        {
            ForceLeaveSliding();
        }
    }

    IEnumerator StartSlideCountdown()
    {
        float remainingTime = slideTime;

        while (remainingTime > 0)
        {
            yield return new WaitForSeconds(0.1f);
            remainingTime -= 0.1f;

            if (Input.GetKeyUp(slideKey))
            {
                remainingTime = 0f;
            }
        }

        ForceLeaveSliding();
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
            //State: Idle
            state = MovementState.idle;
        }
        else if (isWallRun)
        {
            //State: Wallrunning
            state = MovementState.wallRunning;
            moveSpeed = wallRunSpeed;
        }
        else if (IsCrouching && grounded)
        {
            // State: Crouching
            state = MovementState.crouching;
            moveSpeed = crouchingSpeed;
        }
        else if (IsSliding && grounded)
        {
            //State: Sliding
            state = MovementState.sliding;
            moveSpeed = slideSpeed;
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
        Vector3 rayPos = new Vector3(groundRayCastPos.transform.position.x, groundRayCastPos.transform.position.y, groundRayCastPos.transform.position.z);
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

    private bool CanSlide()
    {
        if ((Mathf.Abs(rb.velocity.x) > slideSpeedRequirement || Mathf.Abs(rb.velocity.z) > slideSpeedRequirement) && (state != MovementState.air && state != MovementState.crouching))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ForceLeaveSliding()
    {
        transform.localScale = new Vector3(transform.localScale.x, startYscale, transform.localScale.z);
        state = MovementState.sprinting;
        IsSliding = false;
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(wallRayCastPos.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(wallRayCastPos.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround()
    {
        bool aboveGround = Physics.Raycast(groundRayCastPos.position, Vector3.down, minJumpHeight, whatIsGround);
        Debug.DrawRay(groundRayCastPos.position, Vector3.down * minJumpHeight, aboveGround ? Color.green : Color.red);
        Debug.Log(aboveGround);
        return !aboveGround;
    }

    private void whenToWallRun()
    {
        if((wallLeft || wallRight) && verticalInput > 0 && AboveGround())
        {
            if (!isWallRun)
            {
                StartWallRunning();
            }
        }
        else
        {
            if (isWallRun)
            {
                StopWallRunning();
            }
        }
    }

    private void StartWallRunning()
    {
        isWallRun = true;
    }

    private void WallRunningMovement()
    {
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.5f, rb.velocity.z);

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        //Wallrun force
        rb.AddForce(wallForward * wallRunSpeed, ForceMode.Acceleration);
    }

    private void StopWallRunning()
    {
        isWallRun = false;
    }

    // Video minuto 6:09
    // La re puta madre, tantos errores. El player se queda levitando (problema del rb.UseGravity)
    // Al iniciar wallRun el raycast da false y el player no recibe addForce (probablemente problema del raycast)
}
