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
    public List<BaseNode> windows = new List<BaseNode>();

    public List<BaseNode> Windows
    {
        get
        {
            return windows;
        }

        set
        {
            windows = value;
        }
    }

    private void Start()
    {
    }

    public List<BaseNode> SaveDialogue(List<BaseNode> windowsToSave)
    {
        for (int i = 0; i < windowsToSave.Count; i++)
        { 
            Debug.Log("Windows To Save " + i +  ": " + windowsToSave[i]);
        }
        Windows = windowsToSave;
        for (int i = 0; i < Windows.Count; i++)
        {
            Debug.Log("Current " + i + ": " + Windows[i]);
        }
        if (Windows.Equals(windowsToSave))
        {
            Debug.Log("Lists Match");
        }
        return Windows;
    }

    // Starts conversatioin
    public void TriggerDialogue()
    {
        //FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        for (int i = 0; i < Windows.Count; i++)
        {
            Debug.Log("Current " + i + ": " + Windows[i]);
        }
        FindObjectOfType<DialogueManager>().StartDialogue(Windows);
    }

    // Must be within range to start conversation
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
