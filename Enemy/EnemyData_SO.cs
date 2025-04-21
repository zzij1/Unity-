using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyData")]
public class EnemyData_SO : ScriptableObject
{
    public string enemyType; // 敌人类型
    public int maxHealth;    // 最大生命值
    public int damage;       // 伤害
    public int expReward;//经验奖励
}