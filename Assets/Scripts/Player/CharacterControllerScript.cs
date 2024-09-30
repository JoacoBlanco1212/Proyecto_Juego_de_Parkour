using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerScript : MonoBehaviour
{
    [Header("Basic Movement")]
    public float sprintSpeed;
    public float walkSpeedMutiplier;
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
    public float slideTime;
    public float slideYmultiplier;
    public float slideSpeedRequirementMultiplier;
    public float slideSpeedMultiplier;
    public bool IsSliding;

    [Header("Wallrunning")]
    public bool isWallRun;
    public float wallRunSpeedMultiplier;
    public float wallRunSpeedRequierementMultiplier;
    public float wallJumpSideForce;
    public float wallJumpUpForce;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("Wallrun detection")]
    public float wallCheckDistance;
    public RaycastHit leftWallHit;
    public RaycastHit rightWallHit;
    public bool wallLeft;
    public bool wallRight;

    [Header("Exiting Wallrun")]
    private bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    [Header("WallRun gravity")]
    private float entryVelocityY;
    private float gravity = 9.8f;
    private float halfDuration;
    private float currentTime;

    [Header("Climbing")]
    public float climbSpeedMultiplier;
    public float maxClimbTime;
    private float climbTimer;
    public bool isClimbing;

    [Header("Climb detection")]
    public float climbDetectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;

    private float wallLookAngle;
    private RaycastHit frontWallHit;
    private bool wallFront;
    private Transform lastWall;
    private Vector3 lastWallNormal;

    [Header("Climb Jumping")]
    public float climbJumpUpForce;
    public float climbJumpBackForce;
    public int climbJumps;
    private int climbJumpsLeft;

    [Header("Landing")]
    public float fallThreshold;
    public float lethalFallDistance;
    public float damageMultiplier;
    public float landingDmgMutiplier;
    public float perfectLandingLength;
    private bool isFalling;
    private float startY;
    private bool isPerfectLanding;

    [Header("Ground check")]
    public float rayLength = 0.3f;
    bool groundedGeneral;
    bool groundedTL;
    bool groundedTR;
    bool groundedDL;
    bool groundedDR;

    [Header("Slope handler")]
    public float maxSlopeAngle;
    RaycastHit slopeHit;

    [Header("HP Regen system")]
    private bool canRegen;
    public float startHP;

    [Header("Mantle")]
    public float mantleRayLength;

    [Header("Player info")]
    public float moveSpeed;
    public float walkSpeed;
    public float speedLimit;
    public float crouchingSpeed;
    public float airSpeed;
    public float slideSpeed;
    public float slideSpeedRequirement;
    public float wallRunSpeed;
    public float wallRunSpeedRequierement;
    public float climbSpeed;
    public float counterDrag;
    public float rbStartDrag;

    [Header("Player atributes")]
    public float playerHealth;

    [Header("Keybinds")]
    public KeyCode climbKey;
    public KeyCode climbJumpKey;
    public KeyCode wallJumpKey;
    public KeyCode jumpKey;
    public KeyCode crouchKey;
    public KeyCode slideKey;
    public KeyCode landingKey;
    public KeyCode switchCameraKey;
    public KeyCode sprintKey;

    [Header("References")]
    public Transform orientation;
    public Transform groundRayCastPos;
    public Transform groundTLRayCastPos;
    public Transform groundTRRayCastPos;
    public Transform groundDLRayCastPos;
    public Transform groundDRRayCastPos;
    public Transform wallRayCastPos;
    public GameObject firstPersonCamera;
    public GameObject thirdPersonCamera;
    public PlayerSFXManager pSFX;
    public PlayerCam fpCamSp;
    public Transform topMantleRayPos;
    public Transform middleMantleRayPos;

    Vector3 moveDirection;
    float verticalInput;
    float horizontalInput;
    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        idle,
        walking,
        sprinting,
        crouching,
        air,
        sliding,
        wallRunning,
        climbing,

    }

    public CameraState cameraState;
    public enum CameraState
    {
        first,
        third,
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.drag = groundDrag;
        readyToJump = true;
        isFalling = false;
        groundedGeneral = false;
        playerHealth = startHP;
        canRegen = true;

        startYscale = transform.localScale.y;

        moveSpeed = sprintSpeed;
        walkSpeed = speedLimit * walkSpeedMutiplier;
        crouchingSpeed = speedLimit * crouchSpeedMultiplier;
        airSpeed = moveSpeed * airMultiplier;
        slideSpeed = moveSpeed * slideSpeedMultiplier;
        slideSpeedRequirement = speedLimit * slideSpeedRequirementMultiplier;
        wallRunSpeed = moveSpeed * wallRunSpeedMultiplier;
        wallRunSpeedRequierement = speedLimit * wallRunSpeedRequierementMultiplier;
        climbSpeed = speedLimit * climbSpeedMultiplier;

        fallThreshold = startYscale * 2.5f;
        lethalFallDistance = startYscale * 20f;

        cameraState = CameraState.first;

        counterDrag = groundDrag * 3.5f;
        rbStartDrag = rb.drag;
    }


    // Update is called once per frame
    void Update()
    {
        CheckForCameraInput();
        MyInput();

        IsGrounded();

        CheckForWall();
        whenToWallRun();

        WallClimbCheck();
        whenToClimb();

        checkForFallDmg();
        CheckForHPRegen();

        // CheckForMantle(); Necesito implementacion del personaje, osea scagno la concha de tu madre

        StateHandler();
        AdjustGravityAndDrag();
    }

    private void FixedUpdate()
    {
        if (isWallRun)
        {
            WallRunningMovement();
        }
        if (isClimbing)
        {
            WallClimb();
        }
        MovePlayer();
        SpeedControl();
    }

    private void IsGrounded()
    {
        //Ground check

        groundedTL = Physics.Raycast(groundTLRayCastPos.position, Vector3.down, rayLength);
        // Debug.DrawRay(groundTLRayCastPos.position, Vector3.down * rayLength, groundedTL ? Color.green : Color.red);
        groundedTR = Physics.Raycast(groundTRRayCastPos.position, Vector3.down, rayLength);
        // Debug.DrawRay(groundTRRayCastPos.position, Vector3.down * rayLength, groundedTR ? Color.green : Color.red);
        groundedDL = Physics.Raycast(groundDLRayCastPos.position, Vector3.down, rayLength);
        // Debug.DrawRay(groundDLRayCastPos.position, Vector3.down * rayLength, groundedDL ? Color.green : Color.red);
        groundedDR = Physics.Raycast(groundDRRayCastPos.position, Vector3.down, rayLength);
        // Debug.DrawRay(groundDRRayCastPos.position, Vector3.down * rayLength, groundedDR ? Color.green : Color.red);

        if (groundedTL || groundedTR || groundedDL || groundedDR)
        {
            groundedGeneral = true;
        } else
        {
            groundedGeneral = false;
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //On slope
        if (OnSlope())
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * CalculateSpeedReductionFromHP(), ForceMode.Acceleration);
        }
        //Crouching
        else if (state == MovementState.crouching)
        {
            rb.velocity = new Vector3(moveDirection.x * crouchingSpeed * CalculateSpeedReductionFromHP(), 0f, moveDirection.z * crouchingSpeed * CalculateSpeedReductionFromHP());
        }
        //Sliding
        else if (state == MovementState.sliding)
        {
            rb.AddForce(moveDirection.normalized * slideSpeed * CalculateSpeedReductionFromHP(), ForceMode.Acceleration);
        }
        //Sprinting
        else if (groundedGeneral && Input.GetKey(sprintKey))
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * CalculateSpeedReductionFromHP(), ForceMode.Acceleration);
            AddCounterDragToRB();
        }
        //Walking
        else if (groundedGeneral)
        {
            rb.velocity = new Vector3(moveDirection.x * walkSpeed * CalculateSpeedReductionFromHP(), rb.velocity.y, moveDirection.z * walkSpeed * CalculateSpeedReductionFromHP());
        }
        //On air
        else if (state == MovementState.air)
        {
            rb.AddForce(moveDirection.normalized * airSpeed * CalculateSpeedReductionFromHP(), ForceMode.Acceleration);
        }
    }

    private void AddCounterDragToRB()
    {
        if (state != MovementState.walking)
        {
            if (Mathf.Abs(horizontalInput) < 0.8f && Mathf.Abs(verticalInput) < 0.8f && rb.velocity.magnitude > 0.01f)
            {
                rb.drag = counterDrag;
            }
            else if (horizontalInput != 0 || verticalInput != 0)
            {
                rb.drag = rbStartDrag;
            }
        }
    }

    private void MyInput()
    {
        //Get keyboard inputs
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        //When to jump
        if (Input.GetKeyDown(jumpKey) && readyToJump && groundedGeneral && (state != MovementState.crouching && state != MovementState.sliding))
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //Start crouching
        if (Input.GetKeyDown(crouchKey) && state == MovementState.walking)
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

        rb.AddForce(transform.up * jumpForce * CalculateSpeedReductionFromHP(), ForceMode.Impulse);

        // Call jumpSFX function
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void StateHandler()
    {
        if (rb.velocity == Vector3.zero && !IsCrouching)
        {
            //State: Idle
            state = MovementState.idle;
        }
        else if (isWallRun)
        {
            //State: Wallrunning
            state = MovementState.wallRunning;
            moveSpeed = wallRunSpeed;

            // Call wallRunSFX function
        }
        else if (isClimbing)
        {
            //State: Climbing
            state = MovementState.climbing;

            // Call climbSFX function
        }
        else if (IsCrouching && groundedGeneral)
        {
            // State: Crouching
            state = MovementState.crouching;
            moveSpeed = crouchingSpeed;
        }
        else if (IsSliding && groundedGeneral)
        {
            //State: Sliding
            state = MovementState.sliding;
            moveSpeed = slideSpeed;

            // Call slideSFX function
        }
        else if (groundedGeneral && Input.GetKey(sprintKey))
        {
            // State: Sprinting
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;

            // Call sprintSFX function
            pSFX.PlaySprintSFX();
        }
        else if (groundedGeneral)
        {
            //State: Walking
            state = MovementState.walking;
            moveSpeed = walkSpeed;

            pSFX.PlayWalkSFX();
        }
        else if (!groundedGeneral)
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
        if (state != MovementState.wallRunning)
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

    private bool CanSlide()
    {
        Vector2 flatVel = new Vector2(rb.velocity.x, rb.velocity.z);
        if (flatVel.magnitude > slideSpeedRequirement && state == MovementState.sprinting)
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

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(wallRayCastPos.position, orientation.right, out rightWallHit, wallCheckDistance);
        wallLeft = Physics.Raycast(wallRayCastPos.position, -orientation.right, out leftWallHit, wallCheckDistance);

        // Debug.DrawRay(wallRayCastPos.position, orientation.right * wallCheckDistance, wallRight ? Color.green : Color.red);
        // Debug.DrawRay(wallRayCastPos.position, -orientation.right * wallCheckDistance, wallLeft ? Color.green : Color.red);
    }

    private bool AboveGround()
    {

        return !groundedGeneral;
    }

    private void whenToWallRun()
    {
        //State 1: WallRunning
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall && Input.GetKey(wallJumpKey) && (rb.velocity.magnitude > wallRunSpeedRequierement || isClimbing))
        {
            if (!isWallRun)
            {
                StartWallRunning();
            }
            //Wallrun timer
            if (wallRunTimer > 0)
            {
                wallRunTimer -= Time.deltaTime;
            }
            if (wallRunTimer <= 0 && isWallRun)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }
        }
        // Wall jump
        else if (Input.GetKeyUp(wallJumpKey) && isWallRun)
        {
            wallJump();
        }
        //State 2: Exiting wallRun
        else if (exitingWall)
        {
            if (isWallRun)
            {
                StopWallRunning();
            }

            if (exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }
            if (exitWallTimer <= 0)
            {
                exitingWall = false;
            }
        }

        //State 3: None
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
        wallRunTimer = maxWallRunTime;
        StartParabolicWallRun();
    }

    private void WallRunningMovement()
    {
        rb.useGravity = false;
        // rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.zero;

        if (wallRight)
        {
            wallForward = Vector3.Cross(transform.up, wallNormal);
        }
        if (wallLeft)
        {
            wallForward = Vector3.Cross(wallNormal, transform.up);
        }

        //Wallrun force
        rb.AddForce(wallForward * wallRunSpeed * CalculateSpeedReductionFromHP(), ForceMode.Acceleration);
        UpdateParabolicYTrajectory();

        //Push player to wall for curve walls
        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0) && isWallRun)
        {
            rb.AddForce(-wallNormal * 5f, ForceMode.Acceleration);
        }

        // Camera effects
        // fpCamSp.StartWallRunEffects();
    }

    private void StartParabolicWallRun()
    {
        currentTime = 0f;
        halfDuration = maxWallRunTime / 2f;
        entryVelocityY = rb.velocity.y;
    }

    private void UpdateParabolicYTrajectory()
    {
        currentTime += Time.deltaTime;
        float pendient = 0.1f;
        if (currentTime <= halfDuration)
        {
            // Ascending phase
            float ascendY = pendient * currentTime + entryVelocityY;
            rb.velocity = new Vector3(rb.velocity.x, ascendY, rb.velocity.z);

        }
        else if (currentTime <= maxWallRunTime)
        {
            // Descending phase
            float t = currentTime - halfDuration; // Adjust time
            float descendY = -pendient * t + entryVelocityY;
            rb.AddForce(-transform.up * descendY, ForceMode.Force);

        }

        // End wall run
        if (currentTime >= maxWallRunTime)
        {
            StopWallRunning();
        }
    }

    private void StopWallRunning()
    {
        isWallRun = false;
        rb.useGravity = true;
        currentTime = 0f;
        //End camera effects
        // fpCamSp.EndWallRunEffects();
    }

    private void wallJump()
    {
        //Enter exiting wall state
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        
        //Push player away from the wall
        rb.position += wallNormal * 0.1f;
        
        Vector3 forceToApply = transform.up  * wallJumpUpForce + wallNormal * wallJumpSideForce;


        //Reset Y vel and AddForce
        StopWallRunning();
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply * CalculateSpeedReductionFromHP(), ForceMode.Impulse);

        //Bug con el wallJump en paralelo no funcionando (arreglado?, no se)
        //Solucion: ?
    }

    private void WallClimbCheck()
    {
        wallFront = Physics.SphereCast(wallRayCastPos.position, sphereCastRadius, orientation.forward, out frontWallHit, climbDetectionLength);
        // Debug.DrawRay(wallRayCastPos.position, orientation.forward * climbDetectionLength, wallFront ? Color.green : Color.red);

        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);
        
        if (groundedGeneral)
        {
            climbTimer = maxClimbTime;
            climbJumpsLeft = climbJumps;
        }
    }

    private void whenToClimb()
    {
        // State 1: Climbing
        if (wallFront && Input.GetKey(climbKey) && wallLookAngle < maxWallLookAngle)
        {
            if (!isClimbing && climbTimer > 0) 
            {
                StartClimbing();
            }

            //Timer:
            if (climbTimer > 0)
            {
                climbTimer -= Time.deltaTime;
            }
            if (climbTimer < 0)
            {
                StopClimbing();
            }
        }

        //State: ClimbJump
        else if (wallFront && Input.GetKeyUp(climbJumpKey) && climbJumpsLeft > 0 && !groundedGeneral && isClimbing)
        {
            ClimbJump();
        }

        //State 3: None
        else
        {
            if (isClimbing)
            {
                StopClimbing();
            }
        }

        
    }

    private void StartClimbing()
    {
        isClimbing = true;
    }

    private void WallClimb()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed * CalculateSpeedReductionFromHP(), rb.velocity.z);
    }

    private void StopClimbing()
    {
        isClimbing = false;
    }

    private void ClimbJump()
    {
        Vector3 forceToApply = transform.up * climbJumpUpForce + frontWallHit.normal * climbJumpBackForce;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

        climbJumpsLeft--;
    }

    private void checkForFallDmg()
    {
        //Detect when player starts falling
        if (!isFalling && !groundedGeneral && rb.velocity.y < 0 && state == MovementState.air)
        {
            startY = transform.position.y;
            isFalling = true;
            isPerfectLanding = false;
        }
        // Detect perfect landing
        if (isFalling)
        {
            CheckPerfectLanding();
        }
        // Check if player is climbing or wallrunning to reset fallDmg
        if (state == MovementState.wallRunning || state == MovementState.climbing)
        {
            isFalling = false;
        }
        //Detect when player lands
        if (isFalling && groundedGeneral)
        {
            float fallDistance = startY - transform.position.y;
            if (fallDistance < lethalFallDistance)
            {
                if (fallDistance > fallThreshold)
                {
                    float damage = (fallDistance - fallThreshold) * damageMultiplier;
                    ApplyDamage(damage);
                }
            }
            else
            {
                Debug.Log("Player has died");
                // Death();
            }

            isFalling = false;
        }
    }

    private void ApplyDamage(float damage)
    {
        if (isPerfectLanding)
        {
            Debug.Log("Perfect landing!");
            damage = 0f;
            //Perfect landing system

            // Call perfectLandingSFX function
        }
        else if (Input.GetKey(landingKey))
        {
            Debug.Log("Dmg reduced by landing");
            damage *= landingDmgMutiplier;
            
            // Call landingSFX function
        }
        else
        {
            Debug.Log("Didnt reduce dmg");

            // Call brokenLegsSFX function
            pSFX.PlayDamageSFX();
        }
        playerHealth -= damage;
        Debug.Log("Dmg taken:" + damage);
    }

    private void CheckPerfectLanding()
    {
        bool perfectLandingRaycast = Physics.Raycast(groundRayCastPos.position, Vector3.down, perfectLandingLength);
        // Debug.DrawRay(groundRayCastPos.position, Vector3.down * perfectLandingLength, perfectLandingRaycast ? Color.green : Color.red);

        if (Input.GetKeyDown(landingKey) && perfectLandingRaycast)
        {
            isPerfectLanding = true;
        }
    }

    private float CalculateSpeedReductionFromHP()
    {
        return playerHealth / startHP;
    }

    private void CheckForHPRegen()
    {
        if(playerHealth < startHP && canRegen)
        {
            RegenerateHP();
        }

        if (playerHealth > startHP)
        {
            playerHealth = startHP;
        }
    }

    private void RegenerateHP()
    {
        if (playerHealth < startHP)
        {
            playerHealth += Time.deltaTime;
        }
    }

    private void CheckForCameraInput()
    {
        if (Input.GetKeyDown(switchCameraKey))
        {
            switch (cameraState)
            {
                case CameraState.first:
                    cameraState = CameraState.third;
                    break;

                case CameraState.third:
                    cameraState = CameraState.first;
                    break;
            }
            SwitchCameraMode();
        }
    }

    private void SwitchCameraMode()
    {
        switch (cameraState)
        {
            case CameraState.first:
                thirdPersonCamera.SetActive(false);
                firstPersonCamera.SetActive(true);
                break;

            case CameraState.third:
                firstPersonCamera.SetActive(false);
                thirdPersonCamera.SetActive(true);
                break;
        }
    }

    private void CheckForMantle() // Necesito implementacion del personaje, osea scagno la concha de tu madre
    {
        bool topMantleRay = Physics.Raycast(topMantleRayPos.position, orientation.forward, mantleRayLength);
        Debug.DrawRay(topMantleRayPos.position, orientation.forward * mantleRayLength, topMantleRay ? Color.green : Color.red);

        bool middleMantleRay = Physics.Raycast(middleMantleRayPos.position, orientation.forward, mantleRayLength);
        Debug.DrawLine(middleMantleRayPos.position, orientation.forward * mantleRayLength, middleMantleRay ? Color.green : Color.red);
    }
}
