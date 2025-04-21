using UnityEngine;
using UnityEngine.UI;

public class QuestButton : MonoBehaviour
{
    public Text questNameText; // 任务名称文本
    private QuestData_SO questData; // 任务数据
    private QuestUIController questUIController; // 任务UI控制器

    // 初始化任务按钮
    public void Initialize(QuestData_SO data, QuestUIController controller)
    {
        questData = data;
        questUIController = controller;

        // 更新按钮上的任务名称
        if (questNameText != null)
        {
            questNameText.text = questData.questName;
        }

        // 绑定点击事件
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(OnQuestButtonClick);
    }

    // 点击任务按钮时调用
    private void OnQuestButtonClick()
    {
        if (questUIController != null)
        {
            questUIController.UpdateQuestUI(questData);
        }
    }
}