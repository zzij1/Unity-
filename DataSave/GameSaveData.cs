using System;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;

[Serializable]
public class GameSaveData
{
    public string sceneName;
    public PlayerSaveData player;
    public InventorySaveData inventory;
    public QuestSaveData[] quests;
    public EnemySaveData[] enemies;
    public string lastUsedPortalID;
}

[Serializable]
public class PlayerSaveData
{
    public int currentHealth;
    public int maxHealth;
    public int damage;
    public Vector3 position;
    
}

[Serializable]
public class InventorySaveData
{
    public string[] items;
    public string equippedWeapon;
}

[Serializable]
public class QuestSaveData
{
    public string questId;
    public bool isCompleted;
    public int[] requirementProgress;
}

[Serializable]
public class EnemySaveData
{
    public string enemyId;
    public int currentHealth;
    public float[] position;
}