using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class InteractableObjects
{
    public string name;
    public Interactable script;
    public GameObject prefab;
    public List<DialogueType> dialogueTree = new List<DialogueType>();
}

[CreateAssetMenu(fileName = "InteractableList", menuName = "Interactable Objects")]
public class InteractableList : ScriptableObject {
    public List<InteractableObjects> interactables = new List<InteractableObjects>();

    public void NodeDeleted(int interactable, int index)
    {
        interactables[interactable].dialogueTree.RemoveAt(index);
    }
}