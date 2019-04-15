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

    public float typeSpeed = 0.01f;

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

    /// <summary>
    /// Begins dialogue
    /// </summary>
    /// <param name="dialogueTree"></param>
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

    /// <summary>
    /// Goes to next node in dialogue tree and continues dialogue based on node
    /// </summary>
    void ContinueDialogue()
    {
        currentIndex++;
        if (currentIndex >= serializedTree.Count)
        {
            EndDialogue();
            return;
        }

        switch (serializedTree[currentIndex].dialogueType)
        {
            case "Choice":
                DisplayChoices(serializedTree[currentIndex]);
                break;
            case "Dialogue":
                foreach (string sentence in serializedTree[currentIndex].sentences)
                {
                    sentences.Enqueue(sentence);
                }
                choiceBox.SetActive(false);
                makeChoice = false;
                textBox.SetActive(true);
                DisplayNextSentence();
                break;
            default:
                EndDialogue();
                return;
        }
    }


    /// <summary>
    /// Continues dialogue based on choice player made
    /// </summary>
    /// <param name="choiceIndex"></param>
    public void ContinueDialogueFromChoice(int choiceIndex)
    {
        if (choiceIndex > currentSChoice.choiceDialogueValues.Count - 1)
        {
            EndDialogue();
            return;
        }

        currentIndex = serializedTree[currentSChoice.choiceDialogueValues[choiceIndex]].index;

        switch (serializedTree[currentSChoice.choiceDialogueValues[choiceIndex]].dialogueType)
        {
            case "Choice":
                DisplayChoices(serializedTree[currentSChoice.choiceDialogueValues[choiceIndex]]);
                break;
            case "Dialogue":
                foreach (string sentence in serializedTree[currentSChoice.choiceDialogueValues[choiceIndex]].sentences)
                {
                    sentences.Enqueue(sentence);
                }
                choiceBox.SetActive(false);
                makeChoice = false;
                textBox.SetActive(true);
                DisplayNextSentence();
                break;
            default:
                EndDialogue();
                break;
        }
    }

    /// <summary>
    /// Types out sentence 
    /// </summary>
    /// <param name="sentence"></param>
    /// <returns></returns>
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    /// <summary>
    /// Goes to next sentence 
    /// </summary>
    public void DisplayNextSentence()
    {
        // When out of sentences in the queue
        if (sentences.Count == 0 || sentences == null)
        {
            // End conversation when there are no more sentences and at end of tree 
            if (serializedTree[currentIndex].outputCount == 0)
            {
                EndDialogue();
            }
            // Continue conversation if not at end of tree
            else
            {
                ContinueDialogue();
            }
            return;
        }

        // Displays next sentence
        string sentence = sentences.Dequeue();
        StopAllCoroutines(); // Stops typing if sentence skipped while in the middle of displaying
        StartCoroutine(TypeSentence(sentence));
    }

    /// <summary>
    /// Stops conversation 
    /// </summary>
    public void EndDialogue()
    {
        textBox.SetActive(false);
        choiceBox.SetActive(false);
        endText = true; // End of conversation
    }

    /// <summary>
    /// Displays choices 
    /// </summary>
    /// <param name="choice"></param>
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

        if (textBox.activeSelf)
        {
            textBox.SetActive(false);
        }

        choiceBox.SetActive(true); // Displays choices Box

        ChoiceSelection();
    }

    /// <summary>
    /// I have no idea why this is a thing
    /// </summary>
    public void ChoiceSelection()
    {
        int currentSelection = 1;
        //while (choiceBox.activeSelf)
        //{
            if (Input.GetKeyDown(KeyCode.D))
            {
                currentSelection++;

                if (currentSelection == texts.Count)
                {
                    currentSelection = 1;
                }
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                currentSelection--;

                if (currentSelection == 0)
                {
                    currentSelection = texts.Count;
                }
            }

            texts[currentSelection].text += " <-";
            
            //if (Input.GetKeyDown(KeyCode.Space) && displayedTime >= 1)
            //{
            //    texts[currentSelection].GetComponent<Button>().onClick.Invoke();
            //}
        //}
    }
}
