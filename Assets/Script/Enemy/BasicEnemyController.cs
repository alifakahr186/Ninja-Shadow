using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BasicEnemyController : MonoBehaviour
{
    private enum State
    {
        Moving,
        Knockback,
        Dead
    }

    private State currentState;

    [SerializeField]
    private float
        groundCheckDistance,
        wallCheckDistance,
        movementSpeed,
        maxHealth,
        knockbackDuration,
        lastTouchDamageTime,
        touchDamageCoolDown,
        touchDamage,
        touchDamageWidth,
        touchDamageHeight,
        attackRange,
        attackCooldown;

    [SerializeField]
    private Transform
        groundCheck,
        wallCheck,
        touchDamageCheck,
        playerCheck;

    [SerializeField]
    private LayerMask whatIsGround, whatIsPlayer;

    [SerializeField]
    private Vector2 knockbackSpeed;

    [SerializeField]
    private GameObject
        hitParticle,
        deathChunkParticle,
        deathBloodParticle;


    [SerializeField] private float chaseSpeed;
    [SerializeField] private float playerCheckDistance;

    private float
        currentHealth,
        knockbackStartTime,
        lastAttackTime;

    private float[] attackDetails = new float[2];

    private int
        facingDirection,
        damageDirection;

    private Vector2 movement, touchDamageBothLeft, touchDamageTopRight;

    private bool
        groundDetected,
        wallDetected,
        playerDetected,
        isAttacking,
        isPlayerDead = false;

    private GameObject alive;
    private Rigidbody2D aliveRb;
    private Animator aliveAnim;
    [SerializeField] private AudioClip ninjaDeathSound;
    [SerializeField] private float deathSoundVolume = 1f;
    private void Start()
    {
        alive = transform.Find("Alive").gameObject;
        aliveRb = alive.GetComponent<Rigidbody2D>();
        aliveAnim = alive.GetComponent<Animator>();

        currentHealth = maxHealth;
        facingDirection = 1;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Moving:
                UpdateMovingState();
                break;
            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }
    }

    private void EnterMovingState() { }

    private void UpdateMovingState()
    {
        if (isPlayerDead)
        {
            // Player is dead, just patrol normally
            groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
            wallDetected = Physics2D.Raycast(wallCheck.position, wallCheck.right, wallCheckDistance, whatIsGround);

            if (!groundDetected || wallDetected)
            {
                Flip();
            }
            else
            {
                movement.Set(movementSpeed * facingDirection, aliveRb.linearVelocity.y);
                aliveRb.linearVelocity = movement;
            }

            return; // Exit early to avoid chasing/attacking
        }

        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(wallCheck.position, wallCheck.right, wallCheckDistance, whatIsGround);
        Vector2 direction = Vector2.right * facingDirection;
        playerDetected = Physics2D.Raycast(playerCheck.position, direction, playerCheckDistance, whatIsPlayer);

        if (!groundDetected || wallDetected)
        {
            Flip();
        }
        else
        {
            float currentSpeed = playerDetected ? chaseSpeed : movementSpeed;
            movement.Set(currentSpeed * facingDirection, aliveRb.linearVelocity.y);
            aliveRb.linearVelocity = movement;

        }

        if (playerDetected && !isAttacking && !isPlayerDead)
        {
            float distanceToPlayer = Vector2.Distance(alive.transform.position, playerCheck.position);
            if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
            {
                isAttacking = true;
                aliveAnim.SetTrigger("Attack");
                lastAttackTime = Time.time;
            }

        }

    }

    private void ExitMovingState() { }

    private void EnterKnockbackState()
    {
        knockbackStartTime = Time.time;
        movement.Set(knockbackSpeed.x * damageDirection, knockbackSpeed.y);
        aliveRb.linearVelocity = movement;
        aliveAnim.SetBool("Knockback", true);
    }

    private void UpdateKnockbackState()
    {
        if (Time.time >= knockbackStartTime + knockbackDuration)
        {
            SwitchState(State.Moving);
        }
    }

    private void ExitKnockbackState()
    {
        aliveAnim.SetBool("Knockback", false);
    }

    private void EnterDeadState()
    {

        // Play death particles
        Instantiate(deathChunkParticle, alive.transform.position, deathChunkParticle.transform.rotation);
        Instantiate(deathBloodParticle, alive.transform.position, deathBloodParticle.transform.rotation);
        Destroy(gameObject);
    }

    private void UpdateDeadState() { }

    private void ExitDeadState() { }

    private void Damage(float[] attackDetails)
    {
        currentHealth -= attackDetails[0];

        Instantiate(hitParticle, alive.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));

        damageDirection = attackDetails[1] > alive.transform.position.x ? -1 : 1;

        if (currentHealth > 0.0f)
        {
            SwitchState(State.Knockback);
        }
        else
        {
            SwitchState(State.Dead);
        }
    }


    public void AttackTrigger() // Call from Animation Event
    {
        touchDamageBothLeft.Set(touchDamageCheck.position.x - (touchDamageWidth / 2), touchDamageCheck.position.y - (touchDamageHeight / 2));
        touchDamageTopRight.Set(touchDamageCheck.position.x + (touchDamageWidth / 2), touchDamageCheck.position.y + (touchDamageHeight / 2));

        Collider2D hit = Physics2D.OverlapArea(touchDamageBothLeft, touchDamageTopRight, whatIsPlayer);

        if (hit != null)
        {
            PlayerStats playerStats = hit.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                PlayNinjaDeathSound();
                playerStats.KillPlayer();
                isPlayerDead = true;
            }
        }

        isAttacking = false;
    }

    public void SetIsAttacking(bool value)
    {
        isAttacking = value;
    }

    private void CheckTouchDamage() { } // no longer used directly

    private void Flip()
    {
        facingDirection *= -1;
        alive.transform.Rotate(0.0f, 180.0f, 0.0f);

    }

    private void SwitchState(State state)
    {
        switch (currentState)
        {
            case State.Moving:
                ExitMovingState();
                break;
            case State.Knockback:
                ExitKnockbackState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }

        switch (state)
        {
            case State.Moving:
                EnterMovingState();
                break;
            case State.Knockback:
                EnterKnockbackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }

        currentState = state;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        Vector2 gizmoDirection = Vector2.right * facingDirection;
        Gizmos.DrawLine(playerCheck.position, playerCheck.position + (Vector3)(gizmoDirection * playerCheckDistance));

        Vector2 bothLeft = new Vector2(touchDamageCheck.position.x - (touchDamageWidth / 2), touchDamageCheck.position.y - (touchDamageHeight / 2));
        Vector2 bothRight = new Vector2(touchDamageCheck.position.x + (touchDamageWidth / 2), touchDamageCheck.position.y - (touchDamageHeight / 2));
        Vector2 topRight = new Vector2(touchDamageCheck.position.x + (touchDamageWidth / 2), touchDamageCheck.position.y + (touchDamageHeight / 2));
        Vector2 topLeft = new Vector2(touchDamageCheck.position.x - (touchDamageWidth / 2), touchDamageCheck.position.y + (touchDamageHeight / 2));

        Gizmos.DrawLine(bothLeft, bothRight);
        Gizmos.DrawLine(bothRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bothLeft);
    }

    public void OnPlayerRespawned()
    {
        isPlayerDead = false;
    }
    private void PlayNinjaDeathSound()
    {
        if (ninjaDeathSound != null)
        {
            GameObject tempSound = new GameObject("TempNinjaDeathSound");
            AudioSource source = tempSound.AddComponent<AudioSource>();
            source.clip = ninjaDeathSound;
            source.volume = deathSoundVolume;
            source.Play();
            Destroy(tempSound, ninjaDeathSound.length);
        }
    }
}
