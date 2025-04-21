using System.Linq;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public DialogueData_SO currentData;
    public bool canTalk = false;
    public QuestData_SO quest;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentData != null)
        {
            canTalk = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 当玩家离开触发器范围时，设置为不可以对话
        if (other.CompareTag("Player"))
        {
            canTalk = false;
        }
    }

    private void Start()
    {
        DialoguePiece taskDialoguePiece = currentData.dialoguePieces.Find(piece => piece.quest != null);
        if (taskDialoguePiece != null)
        {
            quest = taskDialoguePiece.quest;
        }
    }

    private void Update()
    {
        if (canTalk && Input.GetKeyDown(KeyCode.E))
        {
            OpenDialogue();
        }
    }

    void OpenDialogue()
    {
        var questTask = QuestManager.instance.GetTask(quest);
        if (questTask != null)
        {
            if (questTask.isStarted && questTask.isCompleted && !questTask.isFinished)
            {
                // 显示任务完成后的对话
                DialogueUI.instance.UpdateDialogueData(currentData);
                DialogueUI.instance.UpdateDialogue(currentData.questCompleteDialogue);

                // 给予奖励
                foreach (var reward in quest.rewards)
                {
                    Inventory.instance.Add(reward);
                }

                questTask.isFinished = true;
                ClearQuestRequiredItems(quest);
                // 任务完成后终止对话逻辑
                return;
            }
        }

        // 如果没有任务或任务未完成，显示默认对话
        if (questTask == null)
        {
            DialogueUI.instance.UpdateDialogueData(currentData);
            DialogueUI.instance.UpdateDialogue(currentData.dialoguePieces[0]);
        }
    }
    void ClearQuestRequiredItems(QuestData_SO quest)
    {
        foreach (var require in quest.questRequires)
        {
            // 查找背包中与任务需求匹配的物品并移除
            var itemsToRemove = Inventory.instance.items.Where(item => item.name== require.name).ToList();
            foreach (var item in itemsToRemove)
            {
                Inventory.instance.Remove(item);
            }
        }
    }
}
