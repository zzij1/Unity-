
using System;
using UnityEngine;

public class PlayerState: CharacterState
{
    public Animator animator;
    public bool isTakingDamage;
    public int currentLevel = 1;
    public int currentExp;
    public int expToNextLevel;
    private ThirdPersonMove movement; // 移动脚本
    private PlayerAttack attack; // 攻击脚本
    private void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<ThirdPersonMove>();
        attack = GetComponent<PlayerAttack>();

    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (currentHealth <= 0)
        {
            Die();
        }

    }

    public override void Die()
    {
        base.Die();
        animator.SetTrigger("Die");
        movement.enabled = false;
        attack.enabled = false;
    }
    public int CalculateRequiredExp(int level) 
    {
        return Mathf.FloorToInt(100 * Mathf.Pow(level, 1.2f));
    }
}
