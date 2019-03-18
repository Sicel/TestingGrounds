using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


class ChoiceNode : BaseNode {
    private Dictionary<int, BaseNode> choiceNodePair;

    private List<Rect> choiceRects;

    private int numChoices;

    private string prompt;

    private List<string> choices;

    private DialogueType choice;

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

    public Dictionary<int, BaseNode> ChoiceNodePair
    {
        get
        {
            return choiceNodePair;
        }

        set
        {
            choiceNodePair = value;
        }
    }

    public List<Rect> ChoiceRects
    {
        get
        {
            return choiceRects;
        }

        set
        {
            choiceRects = value;
        }
    }

    public List<string> Choices
    {
        get
        {
            return choices;
        }

        set
        {
            choices = value;
        }
    }

    public int NumChoices
    {
        get
        {
            return numChoices;
        }

        set
        {
            numChoices = value;
        }
    }

    public string Prompt
    {
        get
        {
            return prompt;
        }

        set
        {
            prompt = value;
        }
    }

    public DialogueType Choice
    {
        get
        {
            return choice;
        }

        set
        {
            choice = value;
        }
    }
    #endregion

    public ChoiceNode()
    {
        windowTitle = "Choice Node";
        choices = new List<string>();

        inputs = new List<BaseNode>();
        inputRects = new List<Rect>();

        outputs = new List<BaseNode>();
        outputRects = new List<Rect>();

        choiceRects = new List<Rect>();

        choiceNodePair = new Dictionary<int, BaseNode>();

        choice = new DialogueType()
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
        prompt = EditorGUILayout.TextArea(prompt);

        numChoices = EditorGUILayout.IntField("Number of Choices", numChoices);
        if (numChoices < 0)
        {
            numChoices = 0;
        }

        EditorGUILayout.LabelField("Choices:");
        EditorGUILayout.Space();
        
        int difference = Mathf.Abs(numChoices - choices.Count);
        if (choices.Count < numChoices)
        {
            for (int i = 0; i < difference; i++)
            {
                choices.Add("");
                choiceRects.Add(new Rect());
            }
        }
        else if (choices.Count > numChoices)
        {
            if (numChoices != 0)
            {
                choices.RemoveRange(numChoices - 1, difference);
                choiceRects.RemoveRange(numChoices - 1, difference);
                for (int i = numChoices; i < difference; i++)
                {
                    if (choiceNodePair.ContainsKey(i))
                    {
                        choiceNodePair.Remove(i);
                        choice.choiceDialogueKeys.Remove(i);
                        choice.choiceDialogueValues.RemoveAt(i);
                    }
                }
            }
            else
            {
                choices.Clear();
                choiceRects.Clear();
                choiceNodePair.Clear();
                choice.choiceDialogueKeys.Clear();
                choice.choiceDialogueValues.Clear();
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
        for (int i = 0; i < choices.Count; i++)
        {
            EditorGUILayout.LabelField("Option " + (i + 1));
            choices[i] = EditorGUILayout.TextArea(choices[i], GUILayout.Height(30));
        }
        EditorGUILayout.EndScrollView();

        // Location of TextAreas
        AddChoiceRects();

        choice.windowRect = windowRect;
        choice.index = index;
        choice.question = prompt;
        choice.choices = choices;
        choice.choiceNum = numChoices;
        choice.inputCount = inputs.Count;
        choice.inputRects = inputRects;
        choice.outputCount = outputs.Count;
        choice.outputRects = outputRects;
        choice.choiceRects = choiceRects;

        if (GUILayout.Button("Clear All", GUILayout.Height(20)))
        {
            numChoices = 0;
            inputRects.Clear();
            inputs.Clear();
            outputs.Clear();
            outputRects.Clear();
            choiceNodePair.Clear();
        }
    }

    public void AddChoiceRects()
    {
        for (int i = 0; i < numChoices; i++)
        {
            choiceRects[i] = new Rect(5, 103 + (50 * i), 290, 50);
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

        choice.inputIndexes.Add(input.index);
    }

    public override void SetOutput(BaseNode output, Vector2 clickPos)
    {
        clickPos.x -= windowRect.x;
        clickPos.y -= windowRect.y;
        
        for (int i = 0; i < choiceRects.Count; i++)
        {
            if (choiceRects[i].Contains(clickPos))
            {
                if (!choiceNodePair.ContainsKey(i))
                {
                    choiceNodePair.Add(i, output);
                    choice.choiceDialogueKeys.Add(i);
                    choice.choiceDialogueValues.Add(output.index);
                    break;
                }

                choiceNodePair[i] = output;
                choice.choiceDialogueValues[i] = output.index;
            }
        }

        for (int i = 0; i < outputs.Count; i++)
        {
            if (outputs[i].Equals(output))
                return;
        }

        outputs.Add(output);
        outputRects.Add(output.windowRect);

        choice.outputIndexes.Add(output.index);

        hasOutputs = true;
    }

    public override void DrawCurves()
    {
        if (choiceNodePair.Count != 0)
        {
            foreach (KeyValuePair<int, BaseNode> connection in choiceNodePair)
            {
                if (connection.Value)
                {
                    Rect rect = choiceRects[connection.Key];
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

        for (int i = 0; i < choiceNodePair.Count; i++)
        {
            if (node.Equals(choiceNodePair[i]))
            {
                choiceNodePair.Remove(i);

                // TODO: Comment
                choice.choiceDialogueKeys.Remove(i);
                choice.choiceDialogueValues.Remove(i);
                break;
            }
        }
    }
}

