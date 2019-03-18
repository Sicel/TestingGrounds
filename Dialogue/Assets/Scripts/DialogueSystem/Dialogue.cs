using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue {

    public string name; // Name of person or thing
    public string question; // Question to be asked if there is one
    public List<string> choices; // Responses to question
    public int numText;
    
    [TextArea(3, 10)]
    public List<string> sentences = new List<string>(); // Dialogue

    [TextArea(3, 10)]
    public List<string> altDialogue; // Dialogue
}
