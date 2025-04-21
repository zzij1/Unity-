using System;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    public Text optionText;
    private Button thisButton;
    private DialoguePiece currentPiece;
    private string nextPieceID;
    private bool takeQuest;
    private void Awake()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(OnOptionClicked);
    }

    public void UpdateOption(DialoguePiece piece,DialogueOption option)
    {
        currentPiece=piece;
        optionText.text=option.text;
        nextPieceID = option.targetID;
        takeQuest=option.takeQuest;
    }

    public void OnOptionClicked()
    {
        if (currentPiece.quest!=null)
        {
            var newTask = new QuestManager.QuestTask()
            {
                questData = Instantiate(currentPiece.quest)
            };
            if (takeQuest)
            {
                if (QuestManager.instance.HaveQuest(newTask.questData))
                {
                    
                }
                else
                {
                    QuestManager.instance.tasks.Add(newTask);
                    QuestManager.instance.GetTask(newTask.questData).isStarted=true;
                }
            }
        }
        if (nextPieceID=="")
        {
            DialogueUI.instance.dialoguePanel.SetActive(false);   
            return;
        }
        else
        {
            DialogueUI.instance.UpdateDialogue(DialogueUI.instance.currentData.dialogueIndex[nextPieceID]);
        }
    }
    
}
