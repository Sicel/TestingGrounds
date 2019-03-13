using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


class DialogueNode : BaseInputNode
{
    private List<BaseNode> inputs;
    private List<Rect> inputRects;

    private List<BaseNode> outputs;
    private List<Rect> outputRects;

    private int numText;

    private List<string> sentences;

    private Dialogue dialogue;

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

    public Dialogue Dialogue
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

        dialogue = new Dialogue();
        dialogue.ConnectedNode = this;

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

        dialogue.sentences = sentences;

        if (GUILayout.Button("Clear All", GUILayout.Height(20)))
        {
            numText = 0;
            sentences.Clear();
            inputs.Clear();
            inputRects.Clear();
            outputs.Clear();
            outputRects.Clear();
        }

        dialogue.ConnectedNode = this;
    }

    public override void SetInput(BaseInputNode input, Vector2 clickPos)
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

        // TODO: Comment
        if (input is DialogueNode)
        {
            DialogueNode dInput = input as DialogueNode;
            dialogue.Inputs.Add(dInput.dialogue);
        }
        else if (input is ChoiceNode)
        {
            ChoiceNode cInput = input as ChoiceNode;
            dialogue.Inputs.Add(cInput.Choice);
        }
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

        // TODO: Comment
        if (output is DialogueNode)
        {
            DialogueNode dOutput = output as DialogueNode;
            dialogue.Outputs.Add(dOutput.dialogue);
        }
        else if (output is ChoiceNode)
        {
            ChoiceNode cOutput = output as ChoiceNode;
            dialogue.Outputs.Add(cOutput.Choice);
        }

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

                // TODO: Comment
                if (inputs[i] is DialogueNode)
                {
                    DialogueNode dInput = inputs[i] as DialogueNode;
                    dialogue.Inputs.Remove(dInput.dialogue);
                }
                else if (inputs[i] is ChoiceNode)
                {
                    ChoiceNode cInput = inputs[i] as ChoiceNode;
                    dialogue.Inputs.Remove(cInput.Choice);
                }

                break;
            }
        }

        for (int i = 0; i < outputs.Count; i++)
        {
            if (node.Equals(outputs[i]))
            {
                outputs.Remove(outputs[i]);
                outputRects.Remove(outputRects[i]);

                // TODO: Comment
                if (outputs[i] is DialogueNode)
                {
                    DialogueNode dOutput = outputs[i] as DialogueNode;
                    dialogue.Outputs.Remove(dOutput.dialogue);
                }
                else if (outputs[i] is ChoiceNode)
                {
                    ChoiceNode cOutput = outputs[i] as ChoiceNode;
                    dialogue.Outputs.Remove(cOutput.Choice);
                }

                break;
            }
        }
    }
}

