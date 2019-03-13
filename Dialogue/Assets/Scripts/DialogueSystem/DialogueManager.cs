using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour {

    public Queue<string> sentences; // Text to be displayed

    public GameObject textBox; // Box for dialogue
    public GameObject choiceBox; // Box for Question + choices

    public Text dialogueText;
    public Text question;

    public Choice currentChoice;
    public int currentIndex;

    public List<string> choices;

    public List<Text> texts; // Options 
    public List<string> altText; // Text following selected option

    public List<DialogueType> currentTree;

    public bool endText = false; // Conversation ended
    public bool makeChoice = false; // Will this conversation have a question?

	// Use this for initialization
	void Start () {
        Interactable.manager = this;
        sentences = new Queue<string>();
    }

    // Update is called once per frame
    void Update () {

    }

    /// <summary>
    /// Begins dialogue
    /// </summary>
    /// <param name="dialogue"></param>
    public void StartDialogue(Dialogue dialogue)
    {
        endText = false; // Text has started
        texts = new List<Text>();
        texts.AddRange(choiceBox.GetComponentsInChildren<Text>()); // Puts choices in ChoiceBox into list
        sentences.Clear(); // Gets rid of previous conversation
        textBox.SetActive(true); // Displays text box

        foreach (string sentence in dialogue.sentences)
        { 
            sentences.Enqueue(sentence);
        }
        
        // If there are no choices to make the choiceBox never shows up
        if (dialogue.choices.Count != 0)
        {
            //foreach (FakeDictionary fD in dialogue.fakeDictionaries)
            //    altText.Add(fD.value);
            altText = dialogue.altDialogue; // Gets possible alternative text
            texts[0].text = dialogue.question;
            choices = dialogue.choices;
            makeChoice = true;
        }

        DisplayNextSentence(); // Shows first sentence

        if (sentences.Count == 0)
        {
            DisplayChoices(choices.ToArray());
        }
    }

    /// <summary>
    /// Start Dialogue from object's dialogue tree
    /// </summary>
    /// <param name="dialogueTree"></param>
    public void StartDialogue(List<DialogueType> dialogueTree, Interactable inter)
    {
        currentTree = dialogueTree;
        endText = false;
        sentences.Clear();
        currentIndex = 0;
        inter.dialogue = currentTree[currentIndex] as Dialogue;

        Debug.Log("From SD: " + dialogueTree.Count);

        if (currentTree[currentIndex] is Choice)
        {
            Choice choice = currentTree[currentIndex] as Choice;
            makeChoice = true;
            DisplayChoices(choice.choices.ToArray());
            return;
        }

        Dialogue normalDialogue = currentTree[currentIndex] as Dialogue;
        textBox.SetActive(true); // Displays text box

        foreach (string sentence in normalDialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        //foreach (BaseNode node in dialogue)
        //{
        //    Debug.Log(node);
        //}
    }

    void ContinueDialogue()
    {
        currentIndex++;
        if (currentTree[currentIndex] == null)
        {
            EndDialogue();
            return;
        }

        if (currentTree[currentIndex] is Choice)
        {
            DisplayChoices(currentTree[currentIndex] as Choice);
        }
        else
        {
            Dialogue dialogue = currentTree[currentIndex] as Dialogue;
            foreach (string sentence in dialogue.sentences)
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
        if (currentChoice.ChoiceDialoguePair[choiceIndex] is Choice)
        {
            DisplayChoices(currentChoice.ChoiceDialoguePair[choiceIndex] as Choice);
        }
        else
        {
            Dialogue dialogue = currentChoice.ChoiceDialoguePair[choiceIndex] as Dialogue;
            foreach (string sentence in dialogue.sentences)
            {
                sentences.Enqueue(sentence);
            }
            choiceBox.SetActive(false);
            makeChoice = false;
            textBox.SetActive(true);
            DisplayNextSentence();
        }

        //Debug.Log(altText[index - 1]);
        //if (altText[index - 1] != "")
        //{
        //    sentences.Enqueue(altText[index - 1]);
        //    choiceBox.SetActive(false);
        //    makeChoice = false;
        //    textBox.SetActive(true);
        //    DisplayNextSentence();
        //}
        //else
        //    EndDialogue();
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
            if (currentTree[currentIndex].Outputs.Count == 0)
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
        // If there are any choices to make they'll be shown after the orginal conversation ends
        if (!makeChoice || choiceBox.activeSelf)
        {
            choiceBox.SetActive(false);
            endText = true; // End of conversation
        }
        else
        {
            DisplayChoices(choices.ToArray());  
        }
    }

    // Displays choices
    public void DisplayChoices(string[] choices)
    {
        texts = new List<Text>();
        texts.AddRange(choiceBox.GetComponentsInChildren<Text>()); // Puts choices in ChoiceBox into list

        // Displays the correct number of choices
        if (choices.Length < texts.Count)
        {
            for (int i = 1; i < texts.Count - choices.Length; i++)
            {
                texts[texts.Count - i].gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 1; i == texts.Count - choices.Length; i++)
            {
                texts[texts.Count - i].gameObject.SetActive(true);
            }
        }

        for (int i = 0; i < choices.Length; i++)
        {
            texts[i+1].text = choices[i];
        }
        choiceBox.SetActive(true); // Displays choices Box
    }
    
    // Displays choices
    public void DisplayChoices(Choice choice)
    {
        currentChoice = choice;
        string[] choices = choice.choices.ToArray();
        texts = new List<Text>();
        texts.AddRange(choiceBox.GetComponentsInChildren<Text>()); // Puts choices in ChoiceBox into list

        // Displays the correct number of choices
        if (choices.Length < texts.Count)
        {
            for (int i = 1; i < texts.Count - choices.Length; i++)
            {
                texts[texts.Count - i].gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 1; i == texts.Count - choices.Length; i++)
            {
                texts[texts.Count - i].gameObject.SetActive(true);
            }
        }

        for (int i = 0; i < choices.Length; i++)
        {
            texts[i+1].text = choices[i];
        }
        choiceBox.SetActive(true); // Displays choices Box
    }

    public void MakeChoice()
    {
        return;
    }
}
