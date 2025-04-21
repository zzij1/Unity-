using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;
    public GameObject QuestCanvas;

    [System.Serializable]
    public class QuestTask
    {
        public QuestData_SO questData;

        public bool isStarted
        {
            get { return questData.isStarted; }
            set { questData.isStarted = value; }
        }

        public bool isCompleted
        {
            get { return questData.isCompleted; }
            set { questData.isCompleted = value; }
        }

        public bool isFinished
        {
            get { return questData.isFinished; }
            set { questData.isFinished = value; }
        }
    }

    public List<QuestTask> tasks = new List<QuestTask>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 防止场景切换时销毁
        }
        else
        {
            Destroy(gameObject); // 销毁多余的实例
        }
    }

    private void OnEnable()
    {
        GameEvents.Instance.OnEnemyKilled += HandleEnemyKilled;
        GameEvents.Instance.OnItemPickedUp += HandleItemPickedUp;
    }

    private void OnDisable()
    {
        GameEvents.Instance.OnEnemyKilled -= HandleEnemyKilled;
        GameEvents.Instance.OnItemPickedUp -= HandleItemPickedUp;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            QuestCanvas.SetActive(!QuestCanvas.activeSelf);

            if (QuestCanvas.activeSelf)
            {
                QuestUIController questUIController = QuestCanvas.GetComponentInParent<QuestUIController>();
                if (questUIController != null)
                {
                    questUIController.UpdateQuestList();
                }
            }
        }
    }

    public bool HaveQuest(QuestData_SO data)
    {
        if (data != null)
        {
            return tasks.Any(q => q.questData.questName == data.questName);
        }
        else
            return false;
    }

    public QuestTask GetTask(QuestData_SO data)
    {
        return tasks.Find(q => q.questData.questName == data.questName);
    }

    private void HandleEnemyKilled(string enemyType)
    {
        // 更新所有需要杀死该类型敌人的任务
        foreach (var task in tasks)
        {
            UpdateQuestProgress(task.questData, enemyType, 1);
        }
    }

    private void HandleItemPickedUp(string itemType)
    {
        // 更新所有需要收集该类型物品的任务
        foreach (var task in tasks)
        {
            UpdateQuestProgress(task.questData, itemType, 1);
        }
    }

    public void UpdateQuestProgress(QuestData_SO questData, string requireKey, int amount)
    {
        var task = GetTask(questData);
        if (task != null)
        {
            var require = task.questData.questRequires.Find(r => 
                r.itemKey == requireKey || r.name == requireKey
            );
            if (require != null)
            {
                require.currentAmount += amount;
                if (require.currentAmount >= require.requireAmount)
                {
                    require.currentAmount = require.requireAmount;
                    CheckQuestCompleted(task);
                }
            }
        }
    }

    private void CheckQuestCompleted(QuestTask task)
    {
        if (task.questData.questRequires.All(r => r.currentAmount >= r.requireAmount))
        {
            task.isCompleted = true;
            task.questData.isCompleted = true;
            Debug.Log($"任务 {task.questData.questName} 已完成！");
        }
    }
}
