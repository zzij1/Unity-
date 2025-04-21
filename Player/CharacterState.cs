using System;
using UnityEngine;

public class CharacterState: MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public int damage;
    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        GameEvents.Instance.OnPlayerStateChanged?.Invoke();
    }

    public virtual void Die()
    {
        
    }
}
