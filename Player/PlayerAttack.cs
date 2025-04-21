using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAttack : MonoBehaviour
{
    public Transform weaponTransform; // 武器位置（持刀时使用）
    public Animator animator; // 动画控制器
    public float attackRange; // 攻击范围
    public Vector3 attackBoxSize; // 矩形区域的大小（长、宽、高）
    public LayerMask rayLayer; // 射线检测的层级
    public bool isAttacking; // 是否正在攻击
    public CharacterState characterState; // 角色状态
    public float attackCooldown = 1.0f; // 攻击冷却时间
    public bool isArmed; // 是否装备武器

    private Coroutine attackCoroutine; // 攻击协程
    private HashSet<Collider> hitColliders = new HashSet<Collider>(); // 已命中的碰撞器
    private float cooldownTimer = 0f; // 冷却时间计时器

    private void Start()
    {
        // 初始化组件
        animator = GetComponent<Animator>();
        characterState = GetComponent<CharacterState>();
    }

    private void Update()
    {
        // 更新冷却时间计时器
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    void OnAttack()
    {
        // 检查是否在冷却时间内
        if (cooldownTimer > 0)
        {
            return; // 如果在冷却时间内，忽略攻击输入
        }

        // 检查是否点击了 UI 元素
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // 执行攻击逻辑
        isAttacking = true;
        animator.SetTrigger("Attack");
        animator.SetBool("IsAttacking", isAttacking);

        // 如果有正在进行的攻击协程，则停止它
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        // 开始新的攻击协程
        attackCoroutine = StartCoroutine(PerformAttack());
        // 重置计时器
        cooldownTimer = attackCooldown;
    }

    private IEnumerator PerformAttack()
    {
        // 清空已命中物体的缓存
        hitColliders.Clear();

        while (isAttacking)
        {
            if (isArmed)
            {
                WeaponRay(); // 持刀攻击（使用射线检测）
            }
            else
            {
                FistAttack(); // 拳头攻击（使用矩形区域检测）
            }
            yield return null; // 每帧等待
        }
    }

    void WeaponRay()
    {
        Vector3 rayStart = weaponTransform.position;
        Vector3 rayDirection = weaponTransform.forward;
        Debug.DrawRay(rayStart, rayDirection * attackRange, Color.red, 0.5f);

        // 使用 Physics.RaycastAll 来检测所有命中的物体
        RaycastHit[] hits = Physics.RaycastAll(rayStart, rayDirection, attackRange, rayLayer);

        foreach (RaycastHit hit in hits)
        {
            Collider collider = hit.collider;

            // 只处理未命中的物体，并且立即处理
            if (!hitColliders.Contains(collider))
            {
                hitColliders.Add(collider);

                CharacterState colliderState = collider.GetComponent<CharacterState>();
                if (colliderState != null)
                {
                    colliderState.TakeDamage(characterState.damage);
                    Debug.Log(collider.transform.name+"受到"+characterState.damage+"点伤害"); // 立即处理新命中的物体
                }
            }
        }
    }

    void FistAttack()
    {
        // 获取角色前方的矩形区域
        Vector3 attackOrigin = transform.position + transform.forward * (attackBoxSize.z / 2)+Vector3.up;
        Collider[] hitColliders = Physics.OverlapBox(attackOrigin, attackBoxSize / 2, transform.rotation, rayLayer);

        foreach (Collider collider in hitColliders)
        {
            // 只处理未命中的物体，并且立即处理
            if (!this.hitColliders.Contains(collider))
            {
                this.hitColliders.Add(collider);

                CharacterState colliderState = collider.GetComponent<CharacterState>();
                if (colliderState != null)
                {
                    colliderState.TakeDamage(characterState.damage);
                }
            }
        }

    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", isAttacking);
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    // 设置是否装备武器
    public void SetArmed(bool armed)
    {
        isArmed = armed;
    }

    // 可视化矩形区域
    private void OnDrawGizmosSelected()
    {
        if (!isArmed)
        {
            Gizmos.color = Color.red;
            Vector3 attackOrigin = transform.position + transform.forward * (attackBoxSize.z / 2)+Vector3.up;
            Gizmos.DrawWireCube(attackOrigin, attackBoxSize);
        }
    }
}