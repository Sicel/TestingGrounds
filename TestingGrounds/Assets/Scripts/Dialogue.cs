using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue {

    public string name; // Name of person or thing
    public string question; // Question to be asked if there is one
    public string[] choices; // Responses to question

    [TextArea(3, 10)]
    public string[] sentences; // Dialogue

    [TextArea(3, 10)]
    public List<string> altDialogue; // Dialogue that follows selected option

    public List<FakeDictionary> fakeDictionaries; // Will allow events to be decided based on choice

    
}

[System.Serializable]
public struct FakeDictionary
{ 
    public string key;

    [TextArea(3, 10)]
    public string value;
}
