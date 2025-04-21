using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuestUIController : MonoBehaviour
{
    public GameObject questButtonPrefab; // 任务按钮的预制体
    public Transform questListContent; // 任务列表的父对象（通常是 Content）
    public Text questNameText; // 任务名称文本
    public Text questDescriptionText; // 任务描述文本
    public Text questRequirementText; // 任务需求文本
    public Text questProgressText; // 任务进度文本

    private List<QuestButton> questButtons = new List<QuestButton>(); // 存储所有任务按钮

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // 防止场景切换时销毁
    }

    private void Start()
    {
        // 初始化任务面板
        ClearQuestUI();
    }

    // 清空任务面板
    public void ClearQuestUI()
    {
        // 获取 Content 中的所有 QuestName Button
        QuestButton[] existingButtons = questListContent.GetComponentsInChildren<QuestButton>();

        // 清空现有的任务按钮
        foreach (var button in existingButtons)
        {
            Destroy(button.gameObject);
        }
        questButtons.Clear();

        // 清空任务信息
        if (questNameText != null) questNameText.text = "";
        if (questDescriptionText != null) questDescriptionText.text = "";
        if (questRequirementText != null) questRequirementText.text = "";
        if (questProgressText != null) questProgressText.text = "";
    }

    // 更新任务列表
    public void UpdateQuestList()
    {
        // 清空现有的任务按钮
        ClearQuestUI();

        // 从 QuestManager 中获取所有任务
        foreach (var task in QuestManager.instance.tasks)
        {
            // 实例化任务按钮
            GameObject buttonObj = Instantiate(questButtonPrefab, questListContent);
            QuestButton questButton = buttonObj.GetComponent<QuestButton>();

            // 绑定任务数据
            questButton.Initialize(task.questData, this);
            questButtons.Add(questButton);
        }
    }

    // 更新任务详细信息
    public void UpdateQuestUI(QuestData_SO questData)
    {
        if (questData == null) return;

        if (questNameText != null) questNameText.text = questData.questName;
        if (questDescriptionText != null) questDescriptionText.text = questData.description;

        // 更新任务需求
        string requirementText = "";
        foreach (var require in questData.questRequires)
        {
            requirementText += $"{require.name}: {require.currentAmount}/{require.requireAmount}\n";
        }
        if (questRequirementText != null) questRequirementText.text = requirementText;

        // 更新任务进度
        if (questProgressText != null) questProgressText.text = $"进度: {GetQuestProgress(questData)}%";
    }

    // 计算任务进度
    private float GetQuestProgress(QuestData_SO questData)
    {
        if (questData.questRequires.Count == 0) return 0;

        float totalProgress = 0;
        foreach (var require in questData.questRequires)
        {
            totalProgress += (float)require.currentAmount / require.requireAmount;
        }

        return (totalProgress / questData.questRequires.Count) * 100;
    }
}