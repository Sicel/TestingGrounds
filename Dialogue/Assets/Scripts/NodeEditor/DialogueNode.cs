using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


class DialogueNode : BaseNode {
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

    public List<string> Sentences { get; set; }

    public int NumText { get; set; }
    #endregion

    public DialogueNode()
    {
        windowTitle = "Dialogue Node";

        inputs = new List<BaseNode>();
        inputRects = new List<Rect>();

        outputs = new List<BaseNode>();
        outputRects = new List<Rect>();

        Sentences = new List<string>();

        dialogueType = new DialogueType()
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
        base.DrawWindow();

        Event e = Event.current;

        NumText = EditorGUILayout.IntField("Number of Sentences", NumText);
        if (NumText < 0)
        {
            NumText = 0;
        }

        EditorGUILayout.LabelField("Dialogue:");
        EditorGUILayout.Space();

        int difference = Mathf.Abs(NumText - Sentences.Count);
        if (Sentences.Count < NumText)
        {
            for (int i = 0; i < difference; i++)
            {
                Sentences.Add("Enter Dialogue Here");
            }
        }
        else if (Sentences.Count > NumText)
        {
            if (NumText != 0)
            {
                Sentences.RemoveRange(NumText - 1, difference);
            }
            else
            {
                Sentences.Clear();
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
        for (int i = 0; i < Sentences.Count; i++)
        {
            Sentences[i] = EditorGUILayout.TextArea(Sentences[i], GUILayout.Height(30));
        }
        EditorGUILayout.EndScrollView();

        dialogueType.windowRect = windowRect;
        dialogueType.index = index;
        dialogueType.sentences = Sentences;
        dialogueType.inputRects = inputRects;
        dialogueType.inputCount = inputRects.Count;
        dialogueType.outputRects = outputRects;
        dialogueType.outputCount = outputRects.Count;

        if (GUILayout.Button("Clear All", GUILayout.Height(20)))
        {
            NumText = 0;
            Sentences.Clear();
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
        dialogueType.inputIndexes.Add(input.index);
        dialogueType.inputRects.Add(input.windowRect);
    }

    public override void SetOutput(BaseNode output, Vector2 clickPos)
    {
        for (int i = 0; i < outputs.Count; i++)
        {
            if (outputs[i].Equals(output))
                return;
        }

        outputs.Add(output);
        outputRects.Add(output.windowRect);
        dialogueType.outputIndexes.Add(output.index);
        dialogueType.outputRects.Add(output.windowRect);

        hasOutputs = true;
    }

    public override void DrawCurves()
    {
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

                dialogueType.inputIndexes.Remove(dialogueType.inputIndexes[i]);
                dialogueType.inputRects.Remove(dialogueType.inputRects[i]);
                break;
            }
        }

        for (int i = 0; i < outputs.Count; i++)
        {
            if (node.Equals(outputs[i]))
            {
                outputs.Remove(outputs[i]);
                outputRects.Remove(outputRects[i]);

                dialogueType.outputIndexes.Remove(dialogueType.outputIndexes[i]);
                dialogueType.outputRects.Remove(dialogueType.outputRects[i]);
                break;
            }
        }
    }
}

