using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDialogue {

    List<IDialogue> Outputs
    {
        get;
        set;
    }

    List<IDialogue> Inputs
    {
        get;
        set;
    }

    /// <summary>
    /// Used to check if this element would be the first node in the Node Editor
    /// </summary>
    bool NoInputs
    {
        get;
        set;
    }

    /// <summary>
    /// Converts node to an IDialogue to make it usable by Interactable
    /// </summary>
    /// <param name="ndoe"></param>
    void ConvertNode(BaseNode ndoe);
}
