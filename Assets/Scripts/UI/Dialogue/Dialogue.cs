using System.Collections;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    private string[] lines;
    [SerializeField] float textSpeed;
    private float originalTextSpeed;
    private int index;
    public bool IsTyping { get; private set; }
    public bool IsDialogueComplete { get; private set; }

    void Start()
    {
        textMeshProUGUI.text = string.Empty;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (IsTyping)
            {
                StopAllCoroutines();
                textMeshProUGUI.text = lines[index];
                IsTyping = false;
                textSpeed = originalTextSpeed;
            }
            else
            {
                NextLine();
            }
        }
    }

    public void SetDialogue(string[] newLines, float newTextSpeed)
    {
        lines = newLines;
        textSpeed = newTextSpeed;
        originalTextSpeed = textSpeed;
        IsDialogueComplete = false;
        StartDialogue();
    }

    void StartDialogue()
    {
        index = 0;
        textMeshProUGUI.text = string.Empty;
        IsTyping = true;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textMeshProUGUI.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        IsTyping = false;
        textSpeed = originalTextSpeed;
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textMeshProUGUI.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            IsDialogueComplete = true;
        }
    }
}