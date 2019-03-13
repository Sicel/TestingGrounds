using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Each interactable object (with text) needs 2 colliders
/// A normal one and one with istrigger checked
/// The istrigger one should be larger than the normal one
/// And is the range from where you can "talk" with the object
/// </summary>
public class Interactable : MonoBehaviour {

    public Dialogue dialogue;
    public Choice choice;
    public static DialogueManager manager;
    public bool notStarted = true;
    public List<DialogueType> dialogueTree = new List<DialogueType>();

    private void Start()
    {
    }

    public void SaveDialogue(List<DialogueType> treeToSave)
    {
        for (int i = 0; i < treeToSave.Count; i++)
        { 
            Debug.Log("Windows To Save " + i +  ": " + treeToSave[i]);
        }
        treeToSave.CopyTo(dialogueTree.ToArray());
        for (int i = 0; i < dialogueTree.Count; i++)
        {
            Debug.Log("Current " + i + ": " + dialogueTree[i]);
        }
        if (dialogueTree.Equals(treeToSave))
        {
            Debug.Log("Lists Match");
        }
    }

    /// <summary>
    /// Begins coversation with object
    /// </summary>
    public void TriggerDialogue()
    {
        //manager.StartDialogue(dialogue);
        for (int i = 0; i < dialogueTree.Count; i++)
        {
            Debug.Log("Current " + i + ": " + dialogueTree[i]);
        }
        manager.StartDialogue(dialogueTree, this);
        //FindObjectOfType<DialogueManager>().StartDialogue(Windows);
    }

    /// <summary>
    /// Makes sure player is within range
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Prevents bug that constantly restarts conversation
        if (Input.GetKeyDown(KeyCode.Space) && notStarted)
        {
            notStarted = false;
            TriggerDialogue();
        }
        if (FindObjectOfType<DialogueManager>().endText)
        {
            notStarted = true;
        }
    }
}
