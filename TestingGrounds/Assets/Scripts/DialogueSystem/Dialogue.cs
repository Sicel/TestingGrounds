using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue : IDialogue {

    public string name; // Name of person or thing
    public string question; // Question to be asked if there is one
    public string[] choices; // Responses to question

    private List<IDialogue> outputs = new List<IDialogue>();
    private List<IDialogue> inputs = new List<IDialogue>();

    private bool noInputs = false;

    [TextArea(3, 10)]
    public List<string> sentences; // Dialogue

    [TextArea(3, 10)]
    public List<string> altDialogue; // Dialogue

    public List<IDialogue> Outputs
    {
        get { return outputs; }
        set { outputs = value; }
    }

    public List<IDialogue> Inputs
    {
        get { return inputs; }
        set { inputs = value; }
    }

    public bool NoInputs
    {
        get {  return noInputs; }
        set {  noInputs = value; }
    }

    void IDialogue.ConvertNode(BaseNode node)
    {
        DialogueNode dialogueNode = node as DialogueNode;
        sentences = dialogueNode.Sentences;

        if (dialogueNode.Outputs.Count != 0)
        {
            foreach (BaseNode outputNode in dialogueNode.Outputs)
            {
                IDialogue output = null;

                if (outputNode is ChoiceNode)
                {
                    output = new Choice();
                }
                else if (outputNode is DialogueNode)
                {
                    output = new Dialogue();
                }

                output.ConvertNode(outputNode);

                if (!Outputs.Contains(output))
                {
                    Outputs.Add(output);
                }
            }
        }

        if (dialogueNode.Inputs.Count != 0)
        {
            foreach (BaseNode inputNode in dialogueNode.Inputs)
            {
                IDialogue input = null;

                if (inputNode is ChoiceNode)
                {
                    input = new Choice();
                }
                else if (inputNode is DialogueNode)
                {
                    input = new Dialogue();
                }

                input.ConvertNode(inputNode);

                if (!Inputs.Contains(input))
                {
                    Inputs.Add(input);
                }
            }
        }
        else
        {
            NoInputs = true;
        }
    }
}
