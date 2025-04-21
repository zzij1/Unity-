using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI instance;
    public Image icon;
    public Text mainText;
    public Button nextButton;
    public GameObject dialoguePanel;
    public DialogueData_SO currentData;
    public int currentIndex=0;
    public RectTransform optionPanel;
    public OptionUI optionPrefab;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // 销毁多余的实例
        }
        DontDestroyOnLoad(gameObject); // 防止场景切换时销毁
        nextButton.onClick.AddListener(ContinueDialogue);
    }


    public void UpdateDialogueData(DialogueData_SO Data)
    {
        currentData = Data;
        currentIndex = 0;
    }

    public void UpdateDialogue(DialoguePiece Piece)
    {
        dialoguePanel.SetActive(true);
        currentIndex++;

        if (Piece.image != null)
        {
            icon.enabled = true;
            icon.sprite = Piece.image;
        }
        else
        {
            icon.enabled = false;
        }

        mainText.text = "";
        mainText.DOText(Piece.text, 0.75f);

        // 如果是任务完成后的对话
        if (Piece == currentData.questCompleteDialogue)
        {
            // 显示 nextButton，并将其功能改为关闭 UI
            nextButton.interactable = true;
            nextButton.gameObject.SetActive(true);
            nextButton.transform.GetChild(0).gameObject.SetActive(true);

            // 修改 nextButton 的点击事件为关闭 UI
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(CloseDialogueUI);
        }
        else if (Piece.options.Count == 0 && currentData.dialoguePieces.Count > 0)
        {
            // 普通对话的逻辑
            nextButton.interactable = true;
            nextButton.gameObject.SetActive(true);
            nextButton.transform.GetChild(0).gameObject.SetActive(true);

            // 恢复 nextButton 的点击事件为继续对话
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(ContinueDialogue);
        }
        else
        {
            // 如果有选项，隐藏 nextButton
            nextButton.interactable = false;
            nextButton.gameObject.SetActive(false);
            nextButton.transform.GetChild(0).gameObject.SetActive(false);
        }

        CreateOptions(Piece);
    }

// 关闭对话 UI 的方法
    private void CloseDialogueUI()
    {
        dialoguePanel.SetActive(false);
        currentIndex = 0; // 重置 currentIndex
    }

// 继续对话的方法
    private void ContinueDialogue()
    {
        if (currentIndex < currentData.dialoguePieces.Count)
        {
            UpdateDialogue(currentData.dialoguePieces[currentIndex]);
        }
        else
        {
            dialoguePanel.SetActive(false);
            currentIndex = 0; // 重置 currentIndex
        }
    }

    void CreateOptions(DialoguePiece Piece)
    {
        if (optionPanel.childCount>0)
        {
            for (int i = 0; i < optionPanel.childCount; i++)
            {
                Destroy(optionPanel.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < Piece.options.Count; i++)
        {
            var option= Instantiate(optionPrefab, optionPanel);
            option.UpdateOption(Piece,Piece.options[i]);
        }
    }
} 
