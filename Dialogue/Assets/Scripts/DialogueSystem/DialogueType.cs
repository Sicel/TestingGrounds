using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DialogueType that will be used for serialization
/// </summary>
[System.Serializable]
public struct DialogueType
{
    [HideInInspector]
    public string dialogueType; // type of dialogue; Choice or Dialogue
    public int index; // location in dialogueTree
    public Rect windowRect; // rectangle of node
    public int inputCount; // number of inputs
    public List<Rect> inputRects; // rects of input nodes
    public List<int> inputIndexes; // location of inputs in tree
    public int outputCount; // number of outputs
    public List<Rect> outputRects; // rects of ouput nodes
    public List<int> outputIndexes; // location of outputs in tree
    public int indexOfFirstInput; // location of first input in dialogueTree
    public int indexOfFirstOutput; // location of first ouput in dialogueTree

    // For DialogueNode
    public List<string> sentences; // Dialogue

    // For ChoiceNode
    public string question; // Prompt
    public List<string> choices; // Choices that can be made
    public int choiceNum; // Number of choices
    public List<Rect> choiceRects; // Location of choices in node window
    public List<int> choiceDialogueKeys; // Choice number 
    public List<int> choiceDialogueValues; // Index of outcome of choice
}
