using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueNode {

    public bool isDialogue;
    public bool isChoice;

    public Dialogue dialogue;
    public Choice choice;

    [HideInInspector]
    public DialogueNode previousNode;
    [HideInInspector]
    public DialogueNode nextNode;
    //public List<DialogueNode> nextNodes;


    public List<DialogueNode> Traverse()
    {
        List<DialogueNode> children = new List<DialogueNode>();
        //foreach (DialogueNode node in nextNodes)
        //{
        //    children.AddRange(node.Traverse());
        //}
        children.AddRange(nextNode.Traverse());

        return children;
    }
}
