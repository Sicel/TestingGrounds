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

    public List<Text> texts; // Options 
    public List<string> altText; // Text following selected option

    public string[] choices; // Possible responses to question

    public bool endText = false; // Conversation ended
    public bool makeChoice = false; // Will this conversation have a question?

	// Use this for initialization
	void Start () {
        sentences = new Queue<string>();
    }

    // Update is called once per frame
    void Update () {

    }

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
        if (dialogue.choices.Length != 0)
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
            DisplayChoices(choices);
        }
    }

    // Types out sentence
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.01f);
        }
    }

    // Goes to next sentence
    public void DisplayNextSentence()
    {
        // End conversation when there are no more sentences
        if (sentences.Count == 0 || sentences == null)
        {
            EndDialogue();
            return;
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
            DisplayChoices(choices);  
        }
    }

    // Displays choices
    public void DisplayChoices(string[] choices)
    {
        // Displayes the correct number of choices
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

    public void AltText(int index)
    {
        endText = false;
        Debug.Log(altText[index -1]);
        if (altText[index-1] != "")
        {
            sentences.Enqueue(altText[index - 1]);
            choiceBox.SetActive(false);
            makeChoice = false;
            textBox.SetActive(true);
            DisplayNextSentence();
        }
        else
            EndDialogue();
    }
}
