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
public class Interactable : MonoBehaviour, ISerializationCallbackReceiver {

    public Dialogue dialogue; // To be deleted
    public static DialogueManager manager; // Dialogue Manager
    public bool dialogueNotStarted = true; // Conversation not in progress
    public List<DialogueType> dialogueTree = new List<DialogueType>(); // Serialized version of dialogue tree in node editor
    
    BaseNode node; // First node in node editor

    private void Start()
    {
    }

    public void SaveDialogue(List<BaseNode> nodesToSave)
    {
        node = nodesToSave[0];
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

    // Before Unity starts serializing, the correct data must be written in
    public void OnBeforeSerialize()
    {
        if (node == null)
        {
            return;
        }
        dialogueTree.Clear();
        AddNodeToSerializedTree(node);
        // Now Unity can serialize the above field and we should get back
        // the expected data during seserialization
    }
    
    /// <summary>
    /// Populates serializable tree using 
    /// </summary>
    /// <param name="baseNode">Node to be serialized</param>
    private void AddNodeToSerializedTree(BaseNode baseNode)
    {
        DialogueType serializableDT = new DialogueType();
        if (baseNode is DialogueNode)
        {
            DialogueNode serializableDialogue = baseNode as DialogueNode;
            serializableDT = serializableDialogue.Dialogue;
        }
        else if (baseNode is ChoiceNode)
        {
            ChoiceNode serializableChoice = baseNode as ChoiceNode;
            serializableDT = serializableChoice.Choice;
        }

        dialogueTree.Add(serializableDT);
        foreach (BaseNode output in baseNode.outputs)
        {
            AddNodeToSerializedTree(output);
        }
    }

    // Unity has just written new data into the serializedDialogueTree field
    // Must be included because of ISerializationCallbackReciever
    public void OnAfterDeserialize() { }
}
