using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueType {

    [SerializeField]
    int index;

    protected List<DialogueType> outputs = new List<DialogueType>();
    protected List<DialogueType> inputs = new List<DialogueType>();

    private BaseNode connectedNode;

    public List<DialogueType> Outputs
    {
        get { return outputs; }
        set { outputs = value; }
    }

    public List<DialogueType> Inputs
    {
        get { return inputs; }
        set { inputs = value; }
    }

    public BaseNode ConnectedNode
    {
        get { return connectedNode; }
        set { connectedNode = value; }
    }
}
