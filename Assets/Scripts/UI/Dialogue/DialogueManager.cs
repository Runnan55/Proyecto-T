using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class DialogueCharInfo
{
    public string dialogueCharName;
    public Sprite dialogueCharSprite;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image characterImage;
    public Dialogue dialogue;
    public GameObject dialogueSubPanel;

    public List<DialogueCharInfo> characters;

    private Dictionary<string, DialogueCharInfo> characterDictionary;
    private Queue<DialogueEntry> dialogueQueue = new Queue<DialogueEntry>();
    private bool isDialogueRunning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeCharacterDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeCharacterDictionary()
    {
        characterDictionary = new Dictionary<string, DialogueCharInfo>();
        foreach (var character in characters)
        {
            characterDictionary[character.dialogueCharName] = character;
        }
    }

    public void CreateDialogue(string characterName, string line)
    {
        dialogueQueue.Enqueue(new DialogueEntry(characterName, new string[] { line }));
        if (!isDialogueRunning)
        {
            StartCoroutine(ProcessDialogueQueue());
        }
    }

    private IEnumerator ProcessDialogueQueue()
    {
        isDialogueRunning = true;

        while (dialogueQueue.Count > 0)
        {
            var dialogueEntry = dialogueQueue.Dequeue();
            if (characterDictionary.TryGetValue(dialogueEntry.CharacterName, out DialogueCharInfo character))
            {
                nameText.text = character.dialogueCharName;
                characterImage.sprite = character.dialogueCharSprite;
                dialogueSubPanel.SetActive(true);
                dialogue.SetDialogue(dialogueEntry.Lines, 0.05f);

                yield return new WaitUntil(() => dialogue.IsDialogueComplete);
            }
            else
            {
                Debug.LogError($"Character {dialogueEntry.CharacterName} not found!");
            }
        }

        dialogueSubPanel.SetActive(false);
        isDialogueRunning = false;
    }
}

public class DialogueEntry
{
    public string CharacterName { get; private set; }
    public string[] Lines { get; private set; }

    public DialogueEntry(string characterName, string[] lines)
    {
        CharacterName = characterName;
        Lines = lines;
    }
}