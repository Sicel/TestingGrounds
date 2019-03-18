using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


class DialogueNode : BaseNode {
    private int numText;

    private List<string> sentences;

    private DialogueType dialogue;

    Vector2 scrollPos;

    #region Properties
    public List<BaseNode> Inputs
    {
        get
        {
            return inputs;
        }

        set
        {
            inputs = value;
        }
    }

    public List<BaseNode> Outputs
    {
        get
        {
            return outputs;
        }

        set
        {
            outputs = value;
        }
    }

    public List<string> Sentences
    {
        get
        {
            return sentences;
        }

        set
        {
            sentences = value;
        }
    }

    public List<Rect> InputRects
    {
        get
        {
            return inputRects;
        }

        set
        {
            inputRects = value;
        }
    }

    public List<Rect> OutputRects
    {
        get
        {
            return outputRects;
        }

        set
        {
            outputRects = value;
        }
    }

    public int NumText
    {
        get
        {
            return numText;
        }

        set
        {
            numText = value;
        }
    }

    public DialogueType Dialogue
    {
        get
        {
            return dialogue;
        }

        set
        {
            dialogue = value;
        }
    }
    #endregion

    public DialogueNode()
    {
        windowTitle = "Dialogue Node";

        inputs = new List<BaseNode>();
        inputRects = new List<Rect>();

        outputs = new List<BaseNode>();
        outputRects = new List<Rect>();

        sentences = new List<string>();

        dialogue = new DialogueType()
        {
            dialogueType = "Dialogue",
            windowRect = windowRect,
            index = index,
            sentences = new List<string>(),
            inputIndexes = new List<int>(),
            inputRects = new List<Rect>(),
            outputIndexes = new List<int>(),
            outputRects = new List<Rect>()
        };

        hasInputs = true;
    }

    public override void DrawWindow()
    {
        // TODO: Paste input code from ChoiceNode
        base.DrawWindow();

        Event e = Event.current;

        numText = EditorGUILayout.IntField("Number of Sentences", numText);
        if (numText < 0)
        {
            numText = 0;
        }

        EditorGUILayout.LabelField("Dialogue:");
        EditorGUILayout.Space();

        int difference = Mathf.Abs(numText - sentences.Count);
        if (sentences.Count < numText)
        {
            for (int i = 0; i < difference; i++)
            {
                sentences.Add("Enter Dialogue Here");
            }
        }
        else if (sentences.Count > numText)
        {
            if (numText != 0)
            {
                sentences.RemoveRange(numText - 1, difference);
            }
            else
            {
                sentences.Clear();
            }
        }

        for (int i = 0; i < inputRects.Count; i++)
        {
            if (e.type == EventType.Repaint)
            {
                inputRects[i] = GUILayoutUtility.GetLastRect();
            }
        }

        for (int i = 0; i < outputRects.Count; i++)
        {
            if (e.type == EventType.Repaint)
            {
                outputRects[i] = GUILayoutUtility.GetLastRect();
            }
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
        for (int i = 0; i < sentences.Count; i++)
        {
            sentences[i] = EditorGUILayout.TextArea(sentences[i], GUILayout.Height(30));
        }
        EditorGUILayout.EndScrollView();

        dialogue.windowRect = windowRect;
        dialogue.index = index;
        dialogue.sentences = sentences;
        dialogue.inputRects = inputRects;
        dialogue.inputCount = inputRects.Count;
        dialogue.outputRects = outputRects;
        dialogue.outputCount = outputRects.Count;

        if (GUILayout.Button("Clear All", GUILayout.Height(20)))
        {
            numText = 0;
            sentences.Clear();
            inputs.Clear();
            inputRects.Clear();
            outputs.Clear();
            outputRects.Clear();
        }
    }

    public override void SetInput(BaseNode input, Vector2 clickPos)
    {
        clickPos.x -= windowRect.x;
        clickPos.y -= windowRect.y;

        for (int i = 0; i < inputs.Count; i++)
        {
            if (inputs[i].Equals(input))
                return;
        }

        inputs.Add(input);
        inputRects.Add(input.windowRect);
        dialogue.inputIndexes.Add(input.index);
        dialogue.inputRects.Add(input.windowRect);
    }

    public override void SetOutput(BaseNode output, Vector2 clickPos)
    {
        // TODO: Add ways to get outputs
        for (int i = 0; i < outputs.Count; i++)
        {
            if (outputs[i].Equals(output))
                return;
        }

        outputs.Add(output);
        outputRects.Add(output.windowRect);
        dialogue.outputIndexes.Add(output.index);
        dialogue.outputRects.Add(output.windowRect);

        hasOutputs = true;
    }

    public override void DrawCurves()
    {
        //for (int i = 0; i < inputs.Count; i++)
        //{
        //    if (inputs[i])
        //    {
        //        Rect rect = windowRect;
        //        rect.x = rect.x + (rect.width / 2);
        //        rect.y = rect.y + (rect.height / 2);
        //        rect.width = 1;
        //        rect.height = 1;

        //        NodeEditor.DrawNodeCurve(inputs[i].windowRect, rect);
        //    }
        //}

        for (int i = 0; i < outputs.Count; i++)
        {
            if (outputs[i])
            {
                Rect rect = windowRect;
                rect.x = rect.x + (rect.width / 2);
                rect.y = rect.y + (rect.height / 2);
                rect.width = 1;
                rect.height = 1;

                Rect outRect = outputs[i].windowRect;
                outRect.x = outRect.x + (outRect.width / 2);
                outRect.y = outRect.y + (outRect.height / 2);
                outRect.width = 1;
                outRect.height = 1;

                NodeEditor.DrawNodeCurve(rect, outRect);
            }
        }
    }

    public override void NodeDeleted(BaseNode node)
    {
        for (int i = 0; i < inputs.Count; i++)
        {
            if (node.Equals(inputs[i]))
            {
                inputs.Remove(inputs[i]);
                inputRects.Remove(inputRects[i]);

                dialogue.inputIndexes.Remove(dialogue.inputIndexes[i]);
                dialogue.inputRects.Remove(dialogue.inputRects[i]);
                break;
            }
        }

        for (int i = 0; i < outputs.Count; i++)
        {
            if (node.Equals(outputs[i]))
            {
                outputs.Remove(outputs[i]);
                outputRects.Remove(outputRects[i]);

                dialogue.outputIndexes.Remove(dialogue.outputIndexes[i]);
                dialogue.outputRects.Remove(dialogue.outputRects[i]);
                break;
            }
        }
    }
}

