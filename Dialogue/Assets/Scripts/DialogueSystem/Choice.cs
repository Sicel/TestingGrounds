using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Choice : DialogueType {

    public string question; // Question to be asked if there is one
    public List<string> choices = new List<string>(); // Responses to question

    private Dictionary<int, DialogueType> choiceDialoguePair = new Dictionary<int, DialogueType>();

    public Dictionary<int, DialogueType> ChoiceDialoguePair
    {
        get { return choiceDialoguePair; }
        set { choiceDialoguePair = value; }
    }
}
