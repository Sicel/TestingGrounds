using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ChoiceNode : BaseInputNode {

    public Choice choice;

    private List<BaseInputNode> inputNodes;

    private List<Rect> inputNodeRects;

    private int index = 0;

    private string question = "";
    private string[] choices;

    private List<Interactable> objects;
    private string[] interactables;

    public ChoiceNode()
    {
        windowTitle = "Choice Node";
        hasInputs = true;
    }

    public override void DrawWindow()
    {
        base.DrawWindow();

        Event e = Event.current;

        objects = new List<Interactable>();
        objects.AddRange(FindObjectsOfType<Interactable>());
        interactables = new string[objects.Count];
        int num = 0;
        foreach (Interactable i in objects)
        {
            interactables[num] = i.gameObject.name;
            num++;
        }
        
        inputNodes = new List<BaseInputNode>();
        inputNodeRects = new List<Rect>();

        for (int i = 0; i < 5; i++)
        {
            inputNodeRects.Add(new Rect());
        }

        EditorGUILayout.LabelField("Select object to modify:", GUILayout.Width(200));
        index = EditorGUILayout.Popup(index, interactables, EditorStyles.popup);
        
        List<string> inputTitles = new List<string>();

        GUILayout.Label("Atached Nodes: " + inputNodes.Count);

        GUILayout.Label("Rects1: " + inputNodeRects.Count);
        for (int i = 0; i < inputNodes.Count; i++)
        {
            if (e.type == EventType.Repaint)
            {
                inputNodeRects[i] = GUILayoutUtility.GetLastRect();
            }
        }

        GUILayout.Label("Rects2: " + inputNodeRects.Count);
    }

    public override void DrawCurves()
    {
        if (inputNodes != null)
        {
            for (int i = 0; i < inputNodes.Count; i++)
            {
                if (inputNodes[i])
                {
                    Rect rect = windowRect;
                    rect.x += inputNodeRects[i].x;
                    rect.y += inputNodeRects[i].y + inputNodeRects[i].height / 2;
                    rect.width = 1;
                    rect.height = 1;

                    NodeEditor.DrawNodeCurve(inputNodes[i].windowRect, rect);
                }
            }
        }
    }

    public override void NodeDeleted(BaseNode node)
    {
        for (int i = 0; i < inputNodes.Count; i++)
        {
            if (node.Equals(inputNodes[i]))
            {
                inputNodes[i] = null;
            }
        }
    }

    public override BaseInputNode ClickedOnInput(Vector2 pos)
    {
        BaseInputNode reVal = null;

        pos.x -= windowRect.x;
        pos.y -= windowRect.y;

        for (int i = 0; i < inputNodes.Count; i++)
        {
            if (inputNodeRects[i].Contains(pos))
            {
                reVal = inputNodes[i];
                inputNodes[i] = null;
                continue;
            }
        }

        return reVal;
    }

    public override void SetInput(BaseInputNode input, Vector2 clickPos)
    {
        clickPos.x -= windowRect.x;
        clickPos.y -= windowRect.y;

        Debug.Log("From: " + input.windowRect + " to Here: " + windowRect);
        for (int i = 0; i < inputNodeRects.Count; i++)
        {
            Debug.Log("Entered1");
            if (inputNodeRects[i].Equals(input.windowRect))
            {
                return;
            }
            else if (inputNodeRects[i].Equals(Rect.zero))
            {
                Debug.Log(i + " Before: " + inputNodeRects[i]);
                inputNodeRects[i] = input.windowRect;
                Debug.Log(i + " After: " + inputNodeRects[i]);
                break;
            }
        }
        Debug.Log("Click At: " + clickPos);
        for (int i = 0; i < inputNodeRects.Count; i++)
        {
            Debug.Log("At: " + i + " Rect: " + inputNodeRects[i] + " Contains: " + inputNodeRects[i].Contains(clickPos));
            if (inputNodeRects[i].Contains(clickPos))
            {
                Debug.Log("Entered2");
                inputNodes[i] = input;
            }
        }
    }
}
