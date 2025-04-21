using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    private Animator animator; // 动画控制器
    private Enemy enemy; // 引用 Enemy 脚本

    private void Awake()
    {
        animator = GetComponent<Animator>(); // 获取动画控制器
        enemy = GetComponent<Enemy>(); // 获取 Enemy 脚本
    }

    // 设置追击状态
    public void SetChasing(bool isChasing)
    {
        animator.SetBool("isChasing", isChasing);
    }

    // 设置攻击状态
    public void SetAttacking(bool isAttacking)
    {
        animator.SetBool("isAttacking", isAttacking);
    }

    // 设置返回状态
    public void SetReturning(bool isReturning)
    {
        animator.SetBool("isReturning", isReturning);
    }

    // 设置巡逻状态
    public void SetPatrolling(bool isPatrolling)
    {
        animator.SetBool("isPatrolling", isPatrolling);
    }

    // 设置 Idle 状态
    public void SetIdle(bool isIdle)
    {
        animator.SetBool("isIdle", isIdle);
    }

    // 设置死亡状态
    public void SetDead(bool isDead)
    {
        animator.SetBool("isDead", isDead);
    }

    // 触发攻击动画
    public void TriggerAttack()
    {
        animator.SetTrigger("Attack");
    }

    // 触发死亡动画
    public void TriggerDeath()
    {
        animator.SetTrigger("Die");
    }

    // 在攻击动画的关键帧调用此方法
    public void OnAttackAnimationHit()
    {
        enemy.OnAttackHit(); // 调用 Enemy 脚本的 OnAttackHit 方法
    }

    // 在死亡动画的最后一帧调用此方法
    public void OnDeathAnimationEndEvent()
    {
        Destroy(gameObject, 1f); // 销毁敌人对象
    }
}