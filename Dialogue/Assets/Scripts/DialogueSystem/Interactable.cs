using System;
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

    public Dialogue dialogue; // To be deleted
    public int index;
    public static DialogueManager manager; // Dialogue Manager
    public bool dialogueNotStarted = true; // Conversation not in progress
    public List<DialogueType> dialogueTree = new List<DialogueType>(); // Serialized version of dialogue tree in node editor
    
    BaseNode node; // First node in node editor

    private void Start()
    {
        int i = 0;
    }

    public void SaveDialogue(BaseNode nodeToSave)
    {
        node = nodeToSave;
        Debug.Log(node);
        Debug.Log(node.outputs.Count);
    }

    /// <summary>
    /// Makes sure player is within range.
    /// Starts conversation with object when space is pressed.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Prevents bug that constantly restarts conversation
        if (Input.GetKeyDown(KeyCode.Space) && dialogueNotStarted)
        {
            dialogueNotStarted = false;
            DialogueManager.dialogueManger.StartDialogue(dialogueTree);
            dialogueNotStarted = true;
        }
    }
}
