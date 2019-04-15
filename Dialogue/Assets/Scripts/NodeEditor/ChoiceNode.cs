using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


class ChoiceNode : BaseNode {
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

    public Dictionary<int, BaseNode> ChoiceNodePair { get; set; }

    public List<Rect> ChoiceRects { get; set; }

    public List<string> Choices { get; set; }

    public int NumChoices { get; set; }

    public string Prompt { get; set; }
    #endregion

    public ChoiceNode()
    {
        windowTitle = "Choice Node";
        Choices = new List<string>();

        inputs = new List<BaseNode>();
        inputRects = new List<Rect>();

        outputs = new List<BaseNode>();
        outputRects = new List<Rect>();

        ChoiceRects = new List<Rect>();

        ChoiceNodePair = new Dictionary<int, BaseNode>();

        dialogueType = new DialogueType()
        {
            dialogueType = "Choice",
            windowRect = windowRect,
            index = index,
            choices = new List<string>(),
            choiceRects = new List<Rect>(),
            choiceDialogueKeys = new List<int>(),
            choiceDialogueValues = new List<int>(),
            inputIndexes = new List<int>(),
            inputRects = new List<Rect>(),
            outputIndexes = new List<int>(),
            outputRects = new List<Rect>(),
        };

        hasInputs = true;
    }

    public override void DrawWindow()
    {
        base.DrawWindow();

        Event e = Event.current;

        EditorGUILayout.LabelField("Prompt:");
        Prompt = EditorGUILayout.TextArea(Prompt);

        NumChoices = EditorGUILayout.IntField("Number of Choices", NumChoices);
        if (NumChoices < 0)
        {
            NumChoices = 0;
        }

        EditorGUILayout.LabelField("Choices:");
        EditorGUILayout.Space();
        
        int difference = Mathf.Abs(NumChoices - Choices.Count);
        if (Choices.Count < NumChoices)
        {
            for (int i = 0; i < difference; i++)
            {
                Choices.Add("");
                ChoiceRects.Add(new Rect());
            }
        }
        else if (Choices.Count > NumChoices)
        {
            if (NumChoices != 0)
            {
                Choices.RemoveRange(NumChoices - 1, difference);
                ChoiceRects.RemoveRange(NumChoices - 1, difference);
                for (int i = NumChoices; i < difference; i++)
                {
                    if (ChoiceNodePair.ContainsKey(i))
                    {
                        ChoiceNodePair.Remove(i);
                        dialogueType.choiceDialogueKeys.Remove(i);
                        dialogueType.choiceDialogueValues.RemoveAt(i);
                    }
                }
            }
            else
            {
                Choices.Clear();
                ChoiceRects.Clear();
                ChoiceNodePair.Clear();
                dialogueType.choiceDialogueKeys.Clear();
                dialogueType.choiceDialogueValues.Clear();
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
        for (int i = 0; i < Choices.Count; i++)
        {
            EditorGUILayout.LabelField("Option " + (i + 1));
            Choices[i] = EditorGUILayout.TextArea(Choices[i], GUILayout.Height(30));
        }
        EditorGUILayout.EndScrollView();

        // Location of TextAreas
        AddChoiceRects();

        dialogueType.windowRect = windowRect;
        dialogueType.index = index;
        dialogueType.question = Prompt;
        dialogueType.choices = Choices;
        dialogueType.choiceNum = NumChoices;
        dialogueType.inputCount = inputs.Count;
        dialogueType.inputRects = inputRects;
        dialogueType.outputCount = outputs.Count;
        dialogueType.outputRects = outputRects;
        dialogueType.choiceRects = ChoiceRects;

        if (GUILayout.Button("Clear All", GUILayout.Height(20)))
        {
            NumChoices = 0;
            inputRects.Clear();
            inputs.Clear();
            outputs.Clear();
            outputRects.Clear();
            ChoiceNodePair.Clear();
        }
    }

    public void AddChoiceRects()
    {
        for (int i = 0; i < NumChoices; i++)
        {
            ChoiceRects[i] = new Rect(5, 103 + (50 * i), 290, 50);
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
    }

    public override void SetOutput(BaseNode output, Vector2 clickPos)
    {
        clickPos.x -= windowRect.x;
        clickPos.y -= windowRect.y;
        
        for (int i = 0; i < ChoiceRects.Count; i++)
        {
            if (ChoiceRects[i].Contains(clickPos))
            {
                if (!ChoiceNodePair.ContainsKey(i))
                {
                    ChoiceNodePair.Add(i, output);
                    dialogueType.choiceDialogueKeys.Add(i);
                    dialogueType.choiceDialogueValues.Add(output.index);
                    break;
                }

                ChoiceNodePair[i] = output;
                dialogueType.choiceDialogueValues[i] = output.index;
            }
        }

        for (int i = 0; i < outputs.Count; i++)
        {
            if (outputs[i].Equals(output))
                return;
        }

        outputs.Add(output);
        outputRects.Add(output.windowRect);

        dialogueType.outputIndexes.Add(output.index);

        hasOutputs = true;
    }

    public override void DrawCurves()
    {
        if (ChoiceNodePair.Count != 0)
        {
            foreach (KeyValuePair<int, BaseNode> connection in ChoiceNodePair)
            {
                if (connection.Value)
                {
                    Rect rect = ChoiceRects[connection.Key];
                    rect.x = rect.x + rect.width + windowRect.x;
                    rect.y = rect.y + (rect.height / 2) + windowRect.y;
                    rect.width = 1;
                    rect.height = 1;

                    Rect outRect = connection.Value.windowRect;
                    outRect.x = outRect.x + (outRect.width / 2);
                    outRect.y = outRect.y + (outRect.height / 2);
                    outRect.width = 1;
                    outRect.height = 1;

                    NodeEditor.DrawNodeCurve(rect, outRect);
                }
            }
        }

        else
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
    }

    public override void NodeDeleted(BaseNode node)
    {
        for (int i = 0; i < inputs.Count; i++)
        {
            if (node.Equals(inputs[i]))
            {
                inputs.Remove(inputs[i]);
                inputRects.Remove(inputRects[i]);

                break;
            }
        }

        for (int i = 0; i < outputs.Count; i++)
        {
            if (node.Equals(outputs[i]))
            {
                outputs.Remove(outputs[i]);
                outputRects.Remove(outputRects[i]);

                break;
            }
        }

        for (int i = 0; i < ChoiceNodePair.Count; i++)
        {
            if (node.Equals(ChoiceNodePair[i]))
            {
                ChoiceNodePair.Remove(i);
                
                dialogueType.choiceDialogueKeys.Remove(i);
                dialogueType.choiceDialogueValues.Remove(i);
                break;
            }
        }
    }
}

