using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour {

    public static DialogueManager dialogueManger;

    public Queue<string> sentences; // Text to be displayed

    public GameObject textBox; // Box for dialogue
    public GameObject choiceBox; // Box for Question + choices

    public Text dialogueText;
    public Text question;
    
    public int currentIndex;

    public List<Text> texts; // Options 
    public List<string> altText; // Text following selected option

    public List<DialogueType> currentTree;
    public DialogueType currentSChoice;

    public List<DialogueType> serializedTree;

    public bool endText = false; // Conversation ended
    public bool makeChoice = false; // Player is making choice

    private void Awake()
    {
        dialogueManger = this;
    }

    // Use this for initialization
    void Start () {
        sentences = new Queue<string>();
    }

    // Update is called once per frame
    void Update () {

    }

    public void StartDialogue(List<DialogueType> dialogueTree)
    {
        serializedTree = dialogueTree;
        endText = false;
        sentences.Clear();
        currentIndex = 0;

        Debug.Log("From SD: " + dialogueTree.Count);

        if (serializedTree[0].dialogueType == "Choice")
        {
            makeChoice = true;
            DisplayChoices(serializedTree[currentIndex]);
            return;
        }
        textBox.SetActive(true); // Displays text box

        foreach (string sentence in serializedTree[0].sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    void ContinueDialogue()
    {
        currentIndex++;
        if (serializedTree[currentIndex].dialogueType == null)
        {
            EndDialogue();
            return;
        }

        if (serializedTree[currentIndex].dialogueType == "Choice")
        {
            DisplayChoices(serializedTree[currentIndex]);
        }
        else if (serializedTree[currentIndex].dialogueType == "Dialogue")
        {
            foreach (string sentence in serializedTree[currentIndex].sentences)
            {
                sentences.Enqueue(sentence);
            }
            choiceBox.SetActive(false);
            makeChoice = false;
            textBox.SetActive(true);
            DisplayNextSentence();
        }
    }


    /// <summary>
    /// Continues dialogue based on choice player made
    /// </summary>
    /// <param name="choiceIndex"></param>
    public void ContinueDialogueFromChoice(int choiceIndex)
    {
        currentIndex++;
        if (serializedTree[currentSChoice.choiceDialogueValues[choiceIndex]].dialogueType == "Choice")
        {
            DisplayChoices(serializedTree[currentSChoice.choiceDialogueValues[choiceIndex]]);
        }
        else
        {
            foreach (string sentence in serializedTree[currentSChoice.choiceDialogueValues[choiceIndex]].sentences)
            {
                sentences.Enqueue(sentence);
            }
            choiceBox.SetActive(false);
            makeChoice = false;
            textBox.SetActive(true);
            DisplayNextSentence();
        }
    }

    // Types out sentence
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            // TODO: Make 0.01 a variable named typing so that it can be adjusted
            yield return new WaitForSeconds(0.01f);
        }
    }

    // Goes to next sentence
    public void DisplayNextSentence()
    {
        // When out of sentences in the queue
        if (sentences.Count == 0 || sentences == null)
        {
            // End conversation when there are no more sentences and at end of tree 
            if (serializedTree[currentIndex].outputCount == 0)
            {
                EndDialogue();
                return;
            }
            // Continue conversation if not at end of tree
            else
            {
                ContinueDialogue();
            }
        }

        // Displays next sentence
        string sentence = sentences.Dequeue();
        StopAllCoroutines(); // Stops typing if sentence skipped while in the middle of displaying
        StartCoroutine(TypeSentence(sentence));
    }

    // Stops conversation
    public void EndDialogue()
    {
        textBox.SetActive(false);
        choiceBox.SetActive(false);
        endText = true; // End of conversation

    }

    // Displays choices
    public void DisplayChoices(DialogueType choice)
    {
        currentSChoice = choice;
        texts = new List<Text>();
        question.text = choice.question;
        texts.AddRange(choiceBox.GetComponentsInChildren<Text>()); // Puts choices in ChoiceBox into list

        // Displays the correct number of choices
        if (choice.choices.Count < texts.Count)
        {
            for (int i = 1; i < texts.Count - choice.choices.Count; i++)
            {
                texts[texts.Count - i].gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 1; i == texts.Count - choice.choices.Count; i++)
            {
                texts[texts.Count - i].gameObject.SetActive(true);
            }
        }

        for (int i = 0; i < choice.choices.Count; i++)
        {
            texts[i + 1].text = choice.choices[i];
        }
        choiceBox.SetActive(true); // Displays choices Box
    }

    public void MakeChoice()
    {
        return;
    }
}
