using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Collection of possible outcomes from choice made
/// </summary>
public class ChoiceManager : MonoBehaviour {

    public Player player;
    private Color originalColor;

    // Continue conversation
    public void ContinueDialogue(GameObject choice)
    {
        string name = choice.name;
        name = name.Remove(0, 6);
        Debug.Log(name);
        int index = int.Parse(name);
        Debug.Log(index);
        DialogueManager.dialogueManger.ContinueDialogueFromChoice(index);
    }

    // End conversation
    public void StopDialogue()
    {
        DialogueManager.dialogueManger.EndDialogue();
    }

    // Teleport to location
    public void GlitchOut()
    {
        Debug.Log("Clicked!");
        DialogueManager.dialogueManger.EndDialogue();
        player.Teleport();
    }

    // Highlight option mouse is hovering over
    public void Highlight(GameObject caller)
    {
        originalColor = caller.GetComponent<Text>().color;
        caller.GetComponent<Text>().color = Color.yellow;
    }

    // Removes highlight from choice
    public void DeHighlight(GameObject caller)
    {
        caller.GetComponent<Text>().color = originalColor;
    }
}
