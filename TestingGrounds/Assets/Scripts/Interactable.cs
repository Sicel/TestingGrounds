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
    public DialogueManager _manager;
    public bool notStarted = true;

    private void Start()
    {
        //int index = 0;
        //for (int i = 0; i < dialogue.fakeDictionaries.Count; i++)
        //{
        //    FakeDictionary fD = dialogue.fakeDictionaries[i];
        //    fD.value = dialogue.altDialogue[i];
        //    Debug.Log("value:" + fD.value);
        //    dialogue.fakeDictionaries[i] = fD;
        //}
    }

    // Starts conversatin
    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
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
