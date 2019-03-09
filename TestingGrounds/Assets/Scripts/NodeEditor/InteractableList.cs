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
}

// TODO: Get Interactable objects in scene and display them
[CreateAssetMenu(fileName = "InteractableList", menuName = "Interactable Objects")]
public class InteractableList : ScriptableObject {

    public List<InteractableObjects> interactables = new List<InteractableObjects>();
}