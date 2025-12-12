using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.UI;

public class PlayerMovements : MonoBehaviour
{
    //UI Input Flags
    private bool uiMoveLeft;
    private bool uiMoveRight;
    private bool uiJump;
    private bool uiDash;

    [Header("MOVEMENT SETTINGS")]
    public float movementSpeed = 10.0f;
    public float jumpForce = 16.0f;
    public float movementForceInAir;
    public float airDragMultiplier = 0.95f;
    public float variableJumpHeightMultiplier = 0.5f;

    private bool canMove;
    private bool canFlip;
    private int facingDirection = 1;


    [Header("GROUND & WALL DETECTION")]
    public Transform groundCheck;
    public Transform groundCheck2;
    public Transform wallCheck;

    public LayerMask whatIsUnclimbableWall;
    public LayerMask whatIsGround;

    public float groundCheckRadius;
    public float groundCheckRadius2;
    public float wallCheckDistance;


    [Header("JUMP / WALL JUMP SYSTEM")]
    public int amountOfJump = 1;
    private int amountOfJumpLeft;
    private int lastWallJumpDirection;

    private float wallJumpTimer;
    private float jumpTimer;
    private float turnTimer;

    private bool canNormalJump;
    private bool canWallJump;
    private bool checkJumpMultiplier;
    private bool isAttemptingToJump;
    private bool isWallSliding;
    private bool hasWallJumped;
    private bool isTouchingWall;
    private bool isTouchingUnclimbableWall;

    public float jumpTimerSet = 0.15f;
    public float wallJumpTimerSet = 0.5f;
    public float wallJumpForce;
    public float wallHopForce;
    public float wallSlideSpeed;
    public float turnTimerSet = 0.1f;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;


    [Header("DASH SYSTEM")]
    public float dashTime;
    public float dashSpeed;
    public float dashCoolDown;
    public float distanceBetweenImages;
    private float movementInputDirection;
    private float verticalInput;
    private float dashTimeLeft;
    private float lastDash = -100f;
    private float lastImageXpos;

    private bool isDashing;


    [Header("DISGUISE / VANISH SYSTEM")]
    public GameObject woodenDummyPrefab;
    public GameObject visuals;  
    public GameObject bones;

    public Transform dummySpawnPoint;

    public CinemachineCamera virtualCam;

    public float dummyMoveSpeed = 2f;

    private bool isVanished = false;


    [Header("CONTROL FLAGS")]
    private bool isControlDisabled = false;
    private bool isAutoRunning = false;
    private static bool levelStartAutoRunTriggered = false;
    [SerializeField] private float runMinSpeed = 0.1f;


    [Header("PROGRESS BAR UI")]
    private bool isFacingRight = true;
    private bool isRunning;
    private bool isGrounded;


    [Header("UI BUTTON EFFECT")]
    [SerializeField] private Image disguiseProgressBar;
    [SerializeField] private Image dashProgressBar;


    [Header("AUDIO SYSTEM")]
    [SerializeField] private PlayerAudioManager audioManager;

    
    [Header("VISUAL EFFECTS")]
    [SerializeField] private GameObject jumpParticlesPrefab;
    [SerializeField] private Transform jumpParticleSpawnPoint;

    [SerializeField] private GameObject vanishParticlesPrefab;
    [SerializeField] private Transform vanishParticleSpawnPoint;

    [Header("COMPONENT REFERENCES")]
    private Rigidbody2D rb;
    private Animator anim;

    public static event System.Action<PlayerMovements> OnPlayerSpawned;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountOfJumpLeft = amountOfJump;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();

        if (!levelStartAutoRunTriggered)
        {
            levelStartAutoRunTriggered = true;
            StartCoroutine(StartAutoRunForDuration(2f));
        }
        audioManager = GetComponent<PlayerAudioManager>();
        if (audioManager == null)
        {
            audioManager = FindAnyObjectByType<PlayerAudioManager>();
        }
    }

    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimation();
        CheckIfCanJump();
        CheckIfWallSliding();
        CheckJump();
        CheckDash();

        if (Input.GetKeyDown(KeyCode.C) && !isVanished)
        {
            StartCoroutine(VanishRoutine());
        }

    }

    //If player respawn this hold reference to new ninja clone
    private void Awake()
    {
        OnPlayerSpawned?.Invoke(this);
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurrondings();
    }

    private IEnumerator VanishRoutine()
    {
        isVanished = true;

        visuals.SetActive(false);
        bones.SetActive(false);


        if (audioManager != null)
        {
            audioManager.PlayDisguiseSound();
        }
        
        if (vanishParticlesPrefab != null && vanishParticleSpawnPoint != null)
        {
            Instantiate(vanishParticlesPrefab, vanishParticleSpawnPoint.position, Quaternion.identity);
        }

        // Spawn dummy
        GameObject dummy = Instantiate(woodenDummyPrefab, dummySpawnPoint.position, Quaternion.identity);

        var dummyController = dummy.GetComponent<PlayerDisguiseController>();
        dummyController.moveSpeed = dummyMoveSpeed;
        dummyController.AssignOwner(this);

        // Set up spike detector if it exists
        var detector = dummy.transform.Find("WoodDisguiseSpikeDetector")?.GetComponent<WoodDisguiseSpikeDetector>();
        if (detector != null)
            detector.Init(dummyController);

        // Make Cinemachine follow dummy
        virtualCam.Follow = dummy.transform;

        // Disable player movement
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        rb.simulated = false;

        if (disguiseProgressBar != null)
        {
            StartCoroutine(DisguiseProgressRoutine(10f));
        }
        // Wait for 7 seconds
        yield return new WaitForSeconds(10f);

        // Destroy dummy
        transform.position = dummy.transform.position;

        // Re-enable ninja
        visuals.SetActive(true);
        bones.SetActive(true);

        if (audioManager != null)
        {
            audioManager.PlayRevertSound();
        }
 
        GetComponent<Collider2D>().enabled = true;
        rb.simulated = true;

        if (vanishParticlesPrefab != null && vanishParticleSpawnPoint != null)
        {
            Vector3 offset = new Vector3(0.1f, 0f, 0f); // Just to trigger proper instantiate
            Instantiate(vanishParticlesPrefab, vanishParticleSpawnPoint.position + offset, Quaternion.identity);
        }

        // Cinemachine follows ninja again
        virtualCam.Follow = transform;
        Destroy(dummy);
        isVanished = false;
    }

    public bool IsVanished()
    {
        return isVanished;
    }
    //Radial Progress Bar on Disgusie UI button
    private IEnumerator DisguiseProgressRoutine(float duration)
    {
        disguiseProgressBar.fillAmount = 1f;
        disguiseProgressBar.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            disguiseProgressBar.fillAmount = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        disguiseProgressBar.fillAmount = 0f;
        disguiseProgressBar.gameObject.SetActive(false); // bar ko hide kar do

    }

    public void KillDueToDummyDeath()
    {
        if (!isVanished) return;

        // Instant respawn logic
        GetComponent<PlayerStats>().KillPlayer();
    }

    public void DeactivateDisguise()
    {
        // agar disguise chal raha hai to turant wapas revert kar do
        StopAllCoroutines();
        visuals.SetActive(true);
        bones.SetActive(true);
        GetComponent<Collider2D>().enabled = true;
        rb.simulated = true;
        virtualCam.Follow = transform;
        isVanished = false;
    }


    private void CheckIfWallSliding()
    {
        // NEW: Allow wall slide as long as player is touching wall and not grounded
        if (isTouchingWall && !isGrounded && rb.linearVelocity.y < 0 && !isTouchingUnclimbableWall)
        {
            // Stop sliding if opposite key is pressed
            if (movementInputDirection != -facingDirection)
            {
                isWallSliding = true;
            }
            else
            {
                isWallSliding = false;
            }
        }
        else
        {
            isWallSliding = false;
        }
    }

    public bool GetDashStatus()
    {
        return isDashing;
    }

    private void CheckSurrondings()
    {
        RaycastHit2D groundHit = Physics2D.CircleCast(groundCheck.position, groundCheckRadius, Vector2.down, 0.1f, whatIsGround);
        RaycastHit2D groundHit2 = Physics2D.CircleCast(groundCheck2.position, groundCheckRadius2, Vector2.down, 0.1f, whatIsGround);

        isGrounded = groundHit.collider != null;
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
        isTouchingUnclimbableWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsUnclimbableWall);

    }

    private void CheckIfCanJump()
    {
        if (isGrounded && rb.linearVelocity.y <= 0.01f)
        {
            amountOfJumpLeft = amountOfJump;
        }

        if (isTouchingWall && !isGrounded)
        {
            canWallJump = true;
        }
        else
        {
            canWallJump = false;
        }

        if (amountOfJumpLeft <= 0)
        {
            canNormalJump = false;
        }
        else
        {
            canNormalJump = true;
        }
    }

    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }

        if (Mathf.Abs(rb.linearVelocity.x) >= 0.01f)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
    }

    //When Disgusie obejct hit level-end trigger it forcefully convert into ninja
    public void ForceRevertDisguise()
    {
        if (!isVanished) return; 

        StopAllCoroutines(); 

        // Enable ninja visuals & collider
        visuals.SetActive(true);
        bones.SetActive(true);
        GetComponent<Collider2D>().enabled = true;
        rb.simulated = true;

        if (virtualCam != null)
        {
            virtualCam.Follow = transform;
        }
            
        if (disguiseProgressBar != null)
        {
            disguiseProgressBar.fillAmount = 0f;
            disguiseProgressBar.gameObject.SetActive(false);
        }
        isVanished = false;
    }

    private void UpdateAnimation()
    {
        anim.SetBool("isRunning", isRunning);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
    }

    private void CheckInput()
    {
        // Combine keyboard and UI input
        float keyboardInput = Input.GetAxisRaw("Horizontal");
        movementInputDirection = keyboardInput;

        if (uiMoveLeft) movementInputDirection = -1f;
        else if (uiMoveRight) movementInputDirection = 1f;
        else if (keyboardInput != 0) movementInputDirection = keyboardInput;

        verticalInput = Input.GetAxisRaw("Vertical");

        bool jumpPressed = Input.GetButtonDown("Jump") || uiJump;
        bool dashPressed = Input.GetButtonDown("Dash") || uiDash;

        if (jumpPressed)
        {
            if (isGrounded || (amountOfJumpLeft > 0 && isTouchingWall))
            {
                NormalJump();
            }
            else
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
                NormalJump();
            }

            uiJump = false; // Reset so it triggers only once
        }

        if (Input.GetButtonDown("Horizontal") && isTouchingWall)
        {
            if (!isGrounded && movementInputDirection != facingDirection)
            {
                canMove = false;
                canFlip = false;
                turnTimer = turnTimerSet;
            }
        }

        if (!canMove)
        {
            turnTimer -= Time.deltaTime;
            if (turnTimer <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }

        if (checkJumpMultiplier && Input.GetButton("Jump"))
        {
            checkJumpMultiplier = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * variableJumpHeightMultiplier);
        }

        if (isWallSliding && verticalInput > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, movementSpeed);
        }

        if (dashPressed)
        {
            if (Time.time >= (lastDash + dashCoolDown))
            {
                AttempToDash();
            }

            uiDash = false; // Reset dash flag
        }
    }

    private void AttempToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;

        if (dashProgressBar != null)
        {
            StartCoroutine(DashProgressRoutine(dashCoolDown));
        }
        if (audioManager != null)
        {
            audioManager.PlayDashSound();
        }
    }

    //Radial Progress Bar on Dash UI button
    private IEnumerator DashProgressRoutine(float duration)
    {
        dashProgressBar.fillAmount = 1f;
        dashProgressBar.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            dashProgressBar.fillAmount = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        dashProgressBar.fillAmount = 0f;
        dashProgressBar.gameObject.SetActive(false);
    }

    private void CheckDash()
    {
        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                canMove = false;
                canFlip = false;
                rb.linearVelocity = new Vector2(dashSpeed * facingDirection, rb.linearVelocity.y);
                dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXpos = transform.position.x;
                }
            }

            if (dashTimeLeft <= 0 || isTouchingWall)
            {
                isDashing = false;
                canMove = true;
                canFlip = true;
            }
        }

    }

    private void CheckJump()
    {
        if (jumpTimer > 0)
        {
            if (!isGrounded && isTouchingWall && movementInputDirection == facingDirection)
            {
                WallJump();
            }
            else if (isGrounded)
            {
                NormalJump();
            }
        }

        if (isAttemptingToJump)
        {
            jumpTimer -= Time.deltaTime;
        }

        if (wallJumpTimer >= 0)
        {
            if (hasWallJumped && movementInputDirection == -lastWallJumpDirection)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0.0f);
                hasWallJumped = false;
            }
            else if (wallJumpTimer <= 0)
            {
                hasWallJumped = false;
            }
            else
            {
                wallJumpTimer -= Time.deltaTime;
            }
        }
    }

    private void NormalJump()
    {
        if (!canNormalJump && !canWallJump) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        if (audioManager != null)
        {
            audioManager.PlayJumpSound();
        }
        amountOfJumpLeft--;

        checkJumpMultiplier = true;
        PlayJumpParticles();

    }

    private void WallJump()
    {
        if (isTouchingUnclimbableWall) return;

        if (canWallJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0.0f);
            isWallSliding = false;
            amountOfJumpLeft = amountOfJump - 1;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
            if (audioManager != null)
            {
                audioManager.PlayJumpSound();
            }
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
            turnTimer = 0;
            canMove = true;
            canFlip = true;
            hasWallJumped = true;
            wallJumpTimer = wallJumpTimerSet;
            lastWallJumpDirection = -facingDirection;
            PlayJumpParticles();
        }
    }

    private void ApplyMovement()
    {
        if (!isGrounded && !isWallSliding && movementInputDirection == 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * airDragMultiplier, rb.linearVelocity.y);
        }
        else if (canMove)
        {
            if (isControlDisabled && isAutoRunning)
            {
                rb.linearVelocity = new Vector2(movementSpeed, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(movementSpeed * movementInputDirection, rb.linearVelocity.y);
            }
        }

        if (isWallSliding && rb.linearVelocity.y < -wallSlideSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
        }
    }

    public void DisabledFlip()
    {
        canFlip = false;
    }

    public void EnabledFlip()
    {
        canFlip = true;
    }
    private void Flip()
    {
        if (!isWallSliding && canFlip)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    public void OnLeftButtonDown() => uiMoveLeft = true;
    public void OnLeftButtonUp() => uiMoveLeft = false;

    public void OnRightButtonDown() => uiMoveRight = true;
    public void OnRightButtonUp() => uiMoveRight = false;

    public void OnJumpButtonDown() => uiJump = true;
    public void OnJumpButtonUp() => uiJump = false;

    public void OnDashButtonDown() => uiDash = true;
    public void OnDashButtonUp() => uiDash = false;
    //Jostu buttons to control left/right movment for mobile 
    public bool IsUIMoveLeftPressed() => uiMoveLeft;
    public bool IsUIMoveRightPressed() => uiMoveRight;
    public void OnDisguiseButtonPressed()
    {
        if (!isVanished)
        {
            StartCoroutine(VanishRoutine());
        }
    }

    // TOUCH INPUT METHODS
    public void SetUIMoveLeft(bool state) => uiMoveLeft = state;
    public void SetUIMoveRight(bool state) => uiMoveRight = state;
    public void SetUIJump(bool state) => uiJump = state;
    public void SetUIDash(bool state) => uiDash = state;
    public bool IsGrounded() => isGrounded;
    public void TriggerVanish()
    {
        if (!isVanished)
        {
            StartCoroutine(VanishRoutine());
        }
    }

    public void DisableControlAndAutoRun()
    {
        isControlDisabled = true;
        isAutoRunning = true;
        rb.linearVelocity = new Vector2(movementSpeed, rb.linearVelocity.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawWireSphere(groundCheck2.position, groundCheckRadius2);
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
    public int GetFacingDirection()
    {
        return facingDirection;
    }

    private void PlayJumpParticles()
    {
        if (jumpParticlesPrefab != null && jumpParticleSpawnPoint != null)
        {
            GameObject particle = Instantiate(jumpParticlesPrefab, jumpParticleSpawnPoint.position, Quaternion.identity);
            Destroy(particle, 0.5f); // Auto-destroy after short time
        }
    }
    private IEnumerator StartAutoRunForDuration(float duration)
    {
        DisableControlAndAutoRun(); // sets isControlDisabled + isAutoRunning + movement

        yield return new WaitForSeconds(duration);

        isControlDisabled = false;
        isAutoRunning = false;
    }

    public void StopRunningSound()
    {
        if (audioManager != null)
        {
            audioManager.StopRunningSound();
        }
       
    }

}