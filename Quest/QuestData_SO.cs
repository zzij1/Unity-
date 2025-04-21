using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Quest/QuestData_SO")]
public class QuestData_SO : ScriptableObject
{
    public string questName;
    public string nameKey;
    public string description;
    public string descKey;
    public bool isStarted;
    public bool isCompleted;
    public bool isFinished;
    [System.Serializable]
    public class QuestRequire
    {
        public string name;
        public string itemKey;
        public int requireAmount;
        public int currentAmount;
    }
    public List<QuestRequire> questRequires = new List<QuestRequire>();
    
    public List<Item> rewards= new List<Item>();
}
 