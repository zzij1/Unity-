using UnityEngine;

public class Enemy : CharacterState
{
    public string enemyID; // 敌人的唯一标识符
    public EnemyData_SO enemyData; // 引用敌人的数据
    public float detectionRange = 10f; // 检测玩家的范围
    public float attackRange = 2f; // 攻击范围
    public float moveSpeed = 6f; // 移动速度
    public float attackCooldown = 2f; // 攻击冷却时间
    public float returnDelay = 2f; // 玩家脱离检测范围后，返回原位的延迟时间
    public float patrolRange = 5f; // 巡逻范围
    public float patrolSpeed = 2f; // 巡逻速度
    public float attackAngle = 60f; // 攻击角度范围
    public float idleWaitTime = 1f; // Idle 等待时间
    public float gravity = 9.8f; // 重力强度

    private Transform player; // 玩家对象
    private EnemyAnimatorController animatorController; // 动画控制器
    private CharacterController characterController; // 角色控制器
    private float lastAttackTime = 0f; // 上次攻击时间
    private Vector3 initialPosition; // 初始位置
    private Vector3 patrolTarget; // 巡逻目标位置
    private float outOfRangeTimer = 0f; // 玩家脱离检测范围的计时器
    private float idleTimer = 0f; // Idle 状态计时器
    private float attackToIdleTimer = 0f; // 攻击后停留在 Idle 状态的计时器
    private bool isWaitingAfterAttack = false; // 是否正在等待攻击后的 Idle 状态
    private Vector3 verticalVelocity; // 垂直速度（用于重力）

    private enum EnemyState { Idle, Patrolling, Chasing, Attacking, Returning, Dead }
    private EnemyState currentState = EnemyState.Idle; // 初始状态为 Idle

    private void Start()
    {
        initialPosition = transform.position; // 保存初始位置
        SetNewPatrolTarget(); // 设置初始巡逻目标
    }

    private void Awake()
    {
        // 初始化CharacterController
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
        }

        if (enemyData == null)
        {
            Debug.LogError("EnemyData is not assigned!");
            return;
        }

        maxHealth = enemyData.maxHealth;
        currentHealth = maxHealth;
        damage = enemyData.damage;

        // 确保CharacterManager已初始化并且player对象有效
        if (CharacterManager.instance != null && CharacterManager.instance.player != null)
        {
            player = CharacterManager.instance.player.transform;
        }
        else
        {
            Debug.LogWarning("Player reference not found, enemy will not function properly.");
        }

        animatorController = GetComponent<EnemyAnimatorController>();
        if (animatorController == null)
        {
            Debug.LogWarning("EnemyAnimatorController not found on enemy.");
        }
    }

    private void Update()
    {
        if (currentState == EnemyState.Dead) return;

        // 应用重力
        ApplyGravity();

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // 如果正在等待攻击后的Idle状态
        if (isWaitingAfterAttack)
        {
            attackToIdleTimer += Time.deltaTime;
            if (attackToIdleTimer >= 1f)
            {
                isWaitingAfterAttack = false;
                attackToIdleTimer = 0f;
                currentState = EnemyState.Idle;
            }
            return;
        }

        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdleState();
                if (distanceToPlayer <= attackRange)
                {
                    currentState = EnemyState.Attacking;
                }
                else if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange)
                {
                    currentState = EnemyState.Chasing;
                }
                break;

            case EnemyState.Patrolling:
                HandlePatrolState();
                if (distanceToPlayer <= attackRange && IsPlayerInFront())
                {
                    currentState = EnemyState.Attacking;
                }
                else if (distanceToPlayer <= detectionRange)
                {
                    currentState = EnemyState.Chasing;
                }
                break;

            case EnemyState.Chasing:
                ChasePlayer();
                if (distanceToPlayer > detectionRange)
                {
                    outOfRangeTimer += Time.deltaTime;
                    if (outOfRangeTimer >= returnDelay)
                    {
                        currentState = EnemyState.Returning;
                    }
                }
                else if (distanceToPlayer <= attackRange && Time.time - lastAttackTime >= attackCooldown)
                {
                    currentState = EnemyState.Attacking;
                }
                break;

            case EnemyState.Attacking:
                PrepareAttack();
                break;

            case EnemyState.Returning:
                if (distanceToPlayer <= detectionRange)
                {
                    currentState = EnemyState.Chasing;
                    outOfRangeTimer = 0f;
                }
                else
                {
                    ReturnToInitialPosition();
                    if (Vector3.Distance(transform.position, initialPosition) < 0.1f)
                    {
                        currentState = EnemyState.Idle;
                        SetNewPatrolTarget();
                    }
                }
                break;
        }

        // 更新动画状态
        if (animatorController != null)
        {
            animatorController.SetChasing(currentState == EnemyState.Chasing);
            animatorController.SetAttacking(currentState == EnemyState.Attacking);
            animatorController.SetReturning(currentState == EnemyState.Returning);
            animatorController.SetPatrolling(currentState == EnemyState.Patrolling);
            animatorController.SetIdle(currentState == EnemyState.Idle);
        }
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity.y = -0.5f; // 轻微向下的力确保贴地
        }
        else
        {
            verticalVelocity.y -= gravity * Time.deltaTime;
        }

        characterController.Move(verticalVelocity * Time.deltaTime);
    }

    private void HandleIdleState()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleWaitTime)
        {
            currentState = EnemyState.Patrolling;
            idleTimer = 0f;
        }
    }

    private void HandlePatrolState()
    {
        Vector3 direction = (patrolTarget - transform.position).normalized;
        direction.y = 0;

        // 使用CharacterController移动
        characterController.Move(direction * patrolSpeed * Time.deltaTime);

        // 面向移动方向
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        if (Vector3.Distance(transform.position, patrolTarget) < 0.5f)
        {
            currentState = EnemyState.Idle;
            SetNewPatrolTarget();
        }
    }

    private void SetNewPatrolTarget()
    {
        patrolTarget = initialPosition + new Vector3(
            Random.Range(-patrolRange, patrolRange),
            0,
            Random.Range(-patrolRange, patrolRange)
        );
    }

    private void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        float currentSpeed = (Vector3.Distance(transform.position, player.position) <= attackRange) ? 0f : moveSpeed;

        characterController.Move(direction * currentSpeed * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void ReturnToInitialPosition()
    {
        Vector3 direction = (initialPosition - transform.position).normalized;
        direction.y = 0;

        characterController.Move(direction * moveSpeed * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void PrepareAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        moveSpeed = 0f;
        if (animatorController != null)
        {
            animatorController.SetAttacking(true);
            animatorController.TriggerAttack();
        }

        lastAttackTime = Time.time;
    }

    public void OnAttackHit()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange && IsPlayerInFront())
        {
            CharacterState playerState = CharacterManager.instance?.playerState;
            if (playerState != null)
            {
                playerState.TakeDamage(damage);
            }
        }

        if (animatorController != null)
        {
            animatorController.SetAttacking(false);
        }

        moveSpeed = 6f;
        isWaitingAfterAttack = true;
        attackToIdleTimer = 0f;
    }

    private bool IsPlayerInFront()
    {
        if (player == null) return false;
        
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle <= attackAngle / 2;
    }

    public override void Die()
    {
        if (currentState == EnemyState.Dead) return;
        
        GameEvents.Instance?.TriggerEnemyKilled(enemyData.enemyType);
        currentState = EnemyState.Dead;
        
        if (animatorController != null)
        {
            animatorController.SetDead(true);
            animatorController.TriggerDeath();
        }
        ExperienceManager.Instance.AddExperience(enemyData.expReward);
        // 禁用碰撞和移动
        characterController.enabled = false;
        enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, patrolRange);

        Gizmos.color = Color.green;
        Vector3 leftBoundary = Quaternion.Euler(0, -attackAngle / 2, 0) * transform.forward * attackRange;
        Vector3 rightBoundary = Quaternion.Euler(0, attackAngle / 2, 0) * transform.forward * attackRange;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}