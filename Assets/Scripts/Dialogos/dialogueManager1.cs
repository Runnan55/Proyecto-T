 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;

public class dialogueManager1 : MonoBehaviour
{
    [Header("DialogueUI")]

    [SerializeField] private GameObject DialoguePanel;
    [SerializeField] private TextMeshProUGUI DialogueText;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    private Story currentStory;
    public bool dialogueIsPlaying { get; private set; }
    private static dialogueManager1 instanceDialogue;
    

    private void Awake()
    {
        if (instanceDialogue != null)
        {
            Debug.LogWarning("Se ha producido un error en el dialogo");
        }
        instanceDialogue = this; 
    }
    public static dialogueManager1 GetInstance()
    {
        return instanceDialogue;
    }
    private void Start()
    {     
        dialogueIsPlaying = false;
        DialoguePanel.SetActive(false);
        
        //Conseguir todas las opciones del texto
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            ContinueStory();
        }
    }

    public void EnterDialogueMode( TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        DialoguePanel.SetActive(true);

       ContinueStory(); 
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);

        dialogueIsPlaying = false;
        DialoguePanel?.SetActive(false);
        DialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            //Mostrar el texto de la line correspondiente
            DialogueText.text = currentStory.Continue();
            //mostrar opciones, si hay, para la linea de dialogo correspondiente
            DisplayChoices();
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count > choices.Length)
        {
            Debug.Log("No tienes mas opciones disponibles. Numero de opciones restantes: " + currentChoices.Count);
        }

        int index = 0;

        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }
    }

}
