using System.Collections;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] 
    private bool combatEnabled;
    [SerializeField]
    private float inputTimer, attack1Radius, attack1Damage;
    [SerializeField]
    private Transform attack1HitBoxPos;
    [SerializeField]
    private LayerMask whatIsDamageable;
    [SerializeField]
    private GameObject sword;

    // New variables for dash
    [SerializeField]
    private float dashSpeed = 10f; // Speed of the dash
    [SerializeField]
    private float dashDistance = 1.5f; // Distance ninja moves forward during attack

    [SerializeField] private EnemyRangeDetector enemyRangeDetector;

    private bool gotInput, isAttacking, isFirstAttack;

    private float lastInputTime = Mathf.NegativeInfinity;

    private float[] attackDetails = new float[2];

    private Animator anim;
    private PlayerMovements PC;
    private PlayerStats PS;

    public GameObject shurikenPrefab;
    public Transform shurikenSpawnPoint; // Must be placed at ninja's right hand
    public float throwCooldown = 0.5f;

    private bool isThrowing;
    private float lastThrowTime;
    private float gameStartDelay = 0.1f; // small delay to let input settle
    private float startTime;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.Rebind();
        anim.SetBool("canAttack", combatEnabled);
        PC = GetComponent<PlayerMovements>();
        PS = GetComponent<PlayerStats>();
        startTime = Time.time;
    }

    private void Update()
    {
        CheckCombatInput();
        CheckAttack();
        if (Time.time >= startTime + gameStartDelay)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                OnAttackButtonPressed();
            }
        }
    }

    void StartThrow()
    {
        isThrowing = true;
        lastThrowTime = Time.time;
        anim.ResetTrigger("isThrowingShuriken");
        anim.SetTrigger("isThrowingShuriken"); // Use "throw" trigger in Animator
    }

    public void ThrowShuriken()
    {
        if (!isThrowing) return;
        GameObject shuriken = Instantiate(shurikenPrefab, shurikenSpawnPoint.position, shurikenSpawnPoint.rotation);
    }

    public void EndThrow()
    {
        isThrowing = false;
    }

    private void CheckCombatInput()
    {
        // We are now using F key for unified attack logic, so this is disabled
        // if (Input.GetMouseButtonDown(0))
        // {
        //     if (combatEnabled)
        //     {
        //         gotInput = true;
        //         lastInputTime = Time.time;
        //     }
        // }

    }

    private void CheckAttack()
    {
        if (gotInput)
        {
            //Perform Attack
            if (!isAttacking)
            {
                gotInput = false;
                isAttacking = true;
                isFirstAttack = !isFirstAttack;
                anim.SetBool("attack1", true);
                anim.SetBool("FirstAttack", isFirstAttack);
                anim.SetBool("isAttacking", isAttacking);
                sword.SetActive(true);
                // Trigger Dash Forward during Sword Attack
                StartCoroutine(PerformDash()); // Coroutine to move ninja forward during attack
            }
        }
        if (Time.time >= lastInputTime + inputTimer)
        {
            gotInput = false;
        }
    }

    // New Coroutine to apply the dash movement during sword attack
    private IEnumerator PerformDash()
    {
        float startPosX = transform.position.x;
        int facingDirection = PC.GetFacingDirection(); // +1 for right, -1 for left
        float dashEndPosX = startPosX + dashDistance * facingDirection;
        float elapsedTime = 0f;

        while (elapsedTime < 0.1f) // Dash duration (adjust as needed)
        {
            transform.position = Vector2.Lerp(new Vector2(startPosX, transform.position.y), new Vector2(dashEndPosX, transform.position.y), elapsedTime / 0.1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure we reach the exact dash end position
        transform.position = new Vector2(dashEndPosX, transform.position.y);
        FinishAttack1();
    }

    private void FinishAttack1()
    {
        sword.SetActive(false);
        isAttacking = false;
        anim.SetBool("isAttacking", isAttacking);
        anim.SetBool("attack1", false);
        CheckAttackHitBox();
    }

    private void CheckAttackHitBox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack1HitBoxPos.position, attack1Radius, whatIsDamageable);

        attackDetails[0] = attack1Damage;
        attackDetails[1] = transform.position.x;
        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.parent.SendMessage("Damage", attackDetails);
            //Instantiate hit particle
        }
    }


    public void Damage(float[] attackDetails)
    {
        if (!PC.GetDashStatus())
        {
            int direction = (attackDetails[1] < transform.position.x) ? 1 : -1;

            // Kill player instead of reducing health
            PS.KillPlayer();
        }
    }

    public void OnAttackButtonPressed()
    {
        if (PC != null && PC.IsVanished())
        {
            return;
        }
        if (Time.time >= lastThrowTime + throwCooldown && !isThrowing && !isAttacking)
        {
            if (enemyRangeDetector != null && enemyRangeDetector.isEnemyInRange)
            {
                // Sword Attack (same logic as key F press)
                if (combatEnabled)
                {
                    gotInput = true;
                    lastInputTime = Time.time;
                }
            }
            else
            {
                // Throw Shuriken
                StartThrow();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack1HitBoxPos.position, attack1Radius);
    }

}