using System;
using UnityEngine;

public class ExperienceManager : MonoBehaviour {
    public static ExperienceManager Instance;
    // 经验增加事件（用于UI更新）
    public event Action<int> OnExpChanged;
    public event Action<int> OnLevelUp;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    

    public void AddExperience(int amount) {
        CharacterManager.instance.playerState.currentExp += amount;
        
        // 循环处理可能的多级提升
        while (CharacterManager.instance.playerState.currentExp >= 
               CharacterManager.instance.playerState.expToNextLevel) {
            LevelUp();
        }
        
        OnExpChanged?.Invoke(CharacterManager.instance.playerState.currentExp);
    }

    private void LevelUp() {
        var state = CharacterManager.instance.playerState;
        state.currentExp -= state.expToNextLevel;
        state.currentLevel++;
        
        // 计算下一级所需经验
        state.expToNextLevel = state.CalculateRequiredExp(state.currentLevel);
        
        // 提升角色属性（示例）
        state.maxHealth += 10;
        state.currentHealth = state.maxHealth;
        
        OnLevelUp?.Invoke(state.currentLevel);
        
        // // 触发升级特效/音效
        // AudioManager.Instance.Play("LevelUp");
        // UIManager.Instance.ShowLevelUpEffect();
    }
}