using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTree : MonoBehaviour {
    [HideInInspector]
    DialogueNode root;

    public List<DialogueNode> tree = new List<DialogueNode>();

	public void StartTree(DialogueNode node)
    {
        if (root == null)
        {
            tree.Add(root = node);
        }
    }

    public void AddNode(DialogueNode parentNode, DialogueNode node)
    {
        DialogueNode currentNode = parentNode;
        if (currentNode.nextNode == null)
        {
            node.previousNode = currentNode;
            currentNode.nextNode = node;
        }
        else
        {
            AddNode(currentNode.nextNode, node);
        }
    }

    public void AddNode(DialogueNode node)
    {
        if (root.nextNode == null)
        {
            node.previousNode = root;
            root.nextNode = node;
        }
    }
}
