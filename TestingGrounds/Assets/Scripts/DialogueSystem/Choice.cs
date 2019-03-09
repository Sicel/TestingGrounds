using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Choice : IDialogue {

    public string question; // Question to be asked if there is one
    public List<string> choices; // Responses to question
    
    private List<IDialogue> outputs = new List<IDialogue>(); 
    private List<IDialogue> inputs = new List<IDialogue>();
    private Dictionary<int, IDialogue> choiceDialoguePair = new Dictionary<int, IDialogue>();
    private bool noInputs = true;

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

    public Dictionary<int, IDialogue> ChoiceDialoguePair
    {
        get { return choiceDialoguePair; }
        set { choiceDialoguePair = value; }
    }
    public bool NoInputs
    {
        get { return noInputs; }
        set { noInputs = value; }
    }

    void IDialogue.ConvertNode(BaseNode rootNode)
    {
        ChoiceNode choiceNode = rootNode as ChoiceNode;
        choices = choiceNode.Choices;
        question = choiceNode.Prompt;

        // Converts outputs from node to IDialogue and adds them to the outputs in this class
        if (choiceNode.Outputs.Count != 0)
        {
            foreach (BaseNode outputNode in choiceNode.Outputs)
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

                // Populates Dictionary to know what a choice leads to
                foreach (KeyValuePair<int, BaseNode> connection in choiceNode.ChoiceNodePair)
                {
                    if (connection.Value.Equals(output) || !ChoiceDialoguePair.ContainsKey(connection.Key))
                    {
                        choiceDialoguePair.Add(connection.Key, output);
                    }
                }
            }
        }

        // Converts inputs from node to IDialogue and adds them to the inputs in this class
        if (choiceNode.Inputs.Count != 0)
        {
            foreach (BaseNode inputNode in choiceNode.Inputs)
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
            noInputs = true;
        }
    }
}
