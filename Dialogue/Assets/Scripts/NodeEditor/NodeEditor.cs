using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeEditor : EditorWindow {

    public List<BaseNode> windows = new List<BaseNode>(); // Nodes being displayed

    private Vector2 mousePos;
    private Vector2 rightClickPos;

    private BaseNode selectedNode;

    private bool makeTransitionMode = false;

    Rect box = new Rect(0, 0, 95, 35); // Side bar rectangle

    private int SelectedInteractable
    {
        get { return EditorPrefs.GetInt("SelectedInteractable", 0); }
        set { EditorPrefs.SetInt("SelectedInteractable", value); }
    }

    private static InteractableList interactableList;

    [MenuItem("Window/Node Editor")]
    static void ShowEditor()
    {
        NodeEditor editor = GetWindow<NodeEditor>();
        // Loads in the interactbles database from this file location
        interactableList = AssetDatabase.LoadAssetAtPath<InteractableList>("Assets/InteractableList.asset");
    }

    /// <summary>
    /// What the Node Editor window shows 
    /// </summary>
    private void OnGUI()
    {
        if (interactableList == null)
        {
            interactableList = AssetDatabase.LoadAssetAtPath<InteractableList>("Assets/InteractableList.asset");
        }

        if (windows.Count == 0)
        {
            box.center = new Vector2(position.width / 2, position.height / 2);
            GUI.Box(box, "No Dialogue Yet");
        }

        Event e = Event.current;

        mousePos = e.mousePosition;

        DisplayInteractables(position);

        // If right clicked and not making a node connection
        if (e.button == 1 && !makeTransitionMode)
        {
            if (e.type == EventType.MouseDown)
            {
                bool clickedOnWindow = false; // Clicked on any node
                int selectedIndex = -1; // Selected Node

                // Checks if clicked on any node
                for (int i = 0; i < windows.Count; i++)
                {
                    if (windows[i].windowRect.Contains(mousePos))
                    {
                        selectedIndex = i;
                        clickedOnWindow = true;
                        break;
                    }
                }

                // Clicked on area outside of node
                if (!clickedOnWindow)
                {
                    GenericMenu menu = new GenericMenu(); // Right-click menu

                    // Adds elements to the right-click menu
                    menu.AddItem(new GUIContent("Add Dialogue Node"), false, ContextCallback, "dialogueNode");
                    menu.AddItem(new GUIContent("Add Choice Node"), false, ContextCallback, "choiceNode");

                    menu.ShowAsContext();
                    e.Use();
                }
                // Clicked inside of a node
                else
                {
                    rightClickPos = e.mousePosition;

                    GenericMenu menu = new GenericMenu(); // Right-click menu

                    // Adds elements to the right-click menu
                    menu.AddItem(new GUIContent("Make Transition"), false, ContextCallback, "makeTransition");
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, "deleteNode");

                    menu.ShowAsContext();
                    e.Use();
                }
            }
        }
        // Left mouse button pressed while in transition mode
        else if(e.button == 0 && e.type == EventType.MouseDown && makeTransitionMode)
        {
            bool clickedOnWindow = false; // Clicked on any node
            int selectedIndex = -1; // Selected Node

            // Checks if clicked on any node
            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].windowRect.Contains(mousePos))
                {
                    selectedIndex = i;
                    clickedOnWindow = true;
                    break;
                }
            }

            // When clicked on another node it connects them
            if (clickedOnWindow && !windows[selectedIndex].Equals(selectedNode))
            {
                windows[selectedIndex].SetInput(selectedNode, mousePos);
                windows[windows.IndexOf(selectedNode)].SetOutput(windows[selectedIndex], rightClickPos);
                makeTransitionMode = false;

                //EditorUtility.SetDirty(selectedNode); // Meant to allow saves

                selectedNode = null;
            }

            // If clicked anywhere outside of a node, transition mode ends
            if (!clickedOnWindow)
            {
                makeTransitionMode = false;
                selectedNode = null;
            }

            e.Use();
        }
        // Left-click and not in transition mode
        else if(e.button == 0 && e.type == EventType.MouseDown && !makeTransitionMode)
        {
            bool clickedOnWindow = false; // Clicked on any node
            int selectedIndex = -1; // Selected Node

            // Checks if clicked on any node
            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].windowRect.Contains(mousePos))
                {
                    selectedIndex = i;
                    clickedOnWindow = true;
                    break;
                }
            }
            
            //TODO: MAKE WINDOWS RESIZABLE
            if (clickedOnWindow)
            {
                // Used to resize node windows
                Rect originalSize = windows[selectedIndex].windowRect;
                Vector2 prevMousePos = mousePos;

                if (mousePos.x >= (windows[selectedIndex].windowRect.width + windows[selectedIndex].windowRect.x) - 10)
                //|| mousePos.x <= windows[selectedIndex].windowRect.x + 5)
                {
                    EditorGUIUtility.AddCursorRect(windows[selectedIndex].windowRect, MouseCursor.ResizeHorizontal);
                    Debug.Log("in X");
                    if (e.type == EventType.MouseDrag)
                    {
                        if (mousePos.x < prevMousePos.x)
                        {
                            Debug.Log("entered");
                            windows[selectedIndex].windowRect.width -= Mathf.Abs(prevMousePos.x - mousePos.x);
                        }
                        else if (mousePos.x > prevMousePos.x)
                        {
                            windows[selectedIndex].windowRect.width += Mathf.Abs(prevMousePos.x - mousePos.x);
                        }
                    }
                }

                if (mousePos.y >= (windows[selectedIndex].windowRect.height + windows[selectedIndex].windowRect.y) - 10)
                //|| mousePos.y <= windows[selectedIndex].windowRect.y + 5)
                {
                    EditorGUIUtility.AddCursorRect(windows[selectedIndex].windowRect, MouseCursor.ResizeVertical);
                    Debug.Log("in Y");
                    if (e.type == EventType.MouseDrag)
                    {
                        Debug.Log("entered");
                        if (mousePos.y < prevMousePos.y)
                        {
                            windows[selectedIndex].windowRect.height -= Mathf.Abs(prevMousePos.y - mousePos.y);
                        }
                        else if (mousePos.y > prevMousePos.y)
                        {
                            windows[selectedIndex].windowRect.height += Mathf.Abs(prevMousePos.y - mousePos.y);
                        }
                    }
                }
            }

            
        }

        // Draws curve between two connected nodes
        if (makeTransitionMode && selectedNode != null)
        {
            Rect mouseRect = new Rect(e.mousePosition.x, e.mousePosition.y, 10, 10);

            DrawNodeCurve(selectedNode.windowRect, mouseRect);

            Repaint();
        }

        // If node editors open with Unity n.DrawCurves would give an error.
        // This catches that error and instead tries to display an object's dialogue tree
        try
        {
            // Draws curves
            foreach (BaseNode n in windows)
            {
                n.DrawCurves();
            }
        }
        catch (System.Exception)
        {
            ConvertTreeToNodes(SelectedInteractable);
        }

        // Draws node windows
        BeginWindows();

        for (int i = 0; i < windows.Count; i++)
        {
            windows[i].windowRect = GUI.Window(i, windows[i].windowRect, DrawNodeWindow, windows[i].windowTitle);
        }

        EndWindows();
    }

    /// <summary>
    /// Draws node windows 
    /// </summary>
    /// <param name="id"></param>
    void DrawNodeWindow(int id)
    {
        windows[id].DrawWindow();
        GUI.DragWindow(); // Allows the nodes to be dragged
    }

    /// <summary>
    /// Editor Update (I think) 
    /// </summary>
    /// <param name="obj"></param>
    void ContextCallback(object obj)
    {
        string clb = obj.ToString();

        // Creates Nodes depending on selection made in menu
        if (clb.Equals("dialogueNode"))
        {
            DialogueNode dialogueNode = new DialogueNode();
            dialogueNode.windowRect = new Rect(mousePos.x, mousePos.y, 300, 300);
            dialogueNode.index = windows.Count;

            windows.Add(dialogueNode);
        }
        else if (clb.Equals("choiceNode"))
        {
            ChoiceNode choiceNode = new ChoiceNode();
            choiceNode.windowRect = new Rect(mousePos.x, mousePos.y, 300, 300);
            choiceNode.index = windows.Count;

            windows.Add(choiceNode);
        }
        // When "Make Transition" is selected
        else if (clb.Equals("makeTransition"))
        {
            bool clickedOnWindow = false;
            int selectedIndex = -1;

            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].windowRect.Contains(mousePos))
                {
                    selectedIndex = i;
                    clickedOnWindow = true;
                    break;
                }
            }

            if (clickedOnWindow)
            {
                selectedNode = windows[selectedIndex];
                makeTransitionMode = true;
            }
        }
        // When "Delete Node" is selected
        else if (clb.Equals("deleteNode"))
        {
            bool clickedOnWindow = false;
            int selectedIndex = -1;

            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].windowRect.Contains(mousePos))
                {
                    selectedIndex = i;
                    clickedOnWindow = true;
                    break;
                }
            }

            // Deletes selected node and all references
            if (clickedOnWindow)
            {
                BaseNode selNode = windows[selectedIndex];
                windows.RemoveAt(selectedIndex);

                foreach(BaseNode n in windows)
                {
                    n.NodeDeleted(selNode);
                }
            }
        }
    }

    /// <summary>
    /// Curve drawn between nodes 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public static void DrawNodeCurve(Rect start, Rect end)
    {
        Vector2 startPos = new Vector3(start.x + start.width / 2, start.y + start.height / 2);
        Vector2 endPos = new Vector3(end.x + end.width / 2, end.y + end.height / 2);
        Vector2 startTan = startPos + Vector2.right * 50;
        Vector2 endTan = endPos + Vector2.left * 50;
        float angle = Mathf.Acos(Vector2.Dot(startPos.normalized, endPos.normalized));
        Color shadowCol = new Color(0, 0, 0, .06f);

        for (int i = 0; i < 3; i ++)
        {
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
        }

        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.green, null, 1);
        // TODO: Make Arrow (Not necessary but please do)
        Handles.ConeHandleCap(0, endPos, Quaternion.Euler(0, 90, angle), 200, EventType.Repaint);
    }

    /// <summary>
    /// Draws a list of interactables on the left side of the Node Editor window
    /// </summary>
    /// <param name="windowRect"></param>
    public void DisplayInteractables(Rect windowRect)
    {
        Vector2 scrollPos = new Vector2();

        // Draws the background box
        Handles.BeginGUI();

        GUI.Box(new Rect(0, 0, 110, windowRect.height), GUIContent.none, EditorStyles.textArea);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
        if (interactableList.interactables.Count != 0)
        {
            for (int i = 0; i < interactableList.interactables.Count; i++)
            {
                DisplayInteractables(i, windowRect);
            }
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Reset All"))
        {
            windows.Clear();
            GUI.FocusControl(null);
        }

        Handles.EndGUI();
    }

    /// <summary>
    /// Displays specific game object
    /// </summary>
    /// <param name="index"></param>
    /// <param name="windowRect"></param>
    public void DisplayInteractables(int index, Rect windowRect)
    {
        // Object selected
        bool isActive = false;

        if (index == SelectedInteractable)
        {
            isActive = true;
            //if (interactableList.interactables[index].dialogueTree.Count != 0)
            //{
            //    dialogueTree = interactableList.interactables[index].dialogueTree;
            //    ConvertTreeToNodes(index);
            //}
            if (GUI.Button(new Rect(5, 0, 50, 20), "Save"))
            {
                interactableList.interactables[index].script.SaveDialogue(windows);

                GUI.FocusControl(null);
            }
            if (GUI.Button(new Rect(55, 0, 50, 20), "Load"))
            {
                ConvertTreeToNodes(index);
            }
        }

        // Shows the object
        Texture2D previewImage = AssetPreview.GetAssetPreview(interactableList.interactables[index].prefab);
        GUIContent buttonContent = new GUIContent(previewImage);

        // Box that contains the object
        GUI.Label(new Rect(5, index * 128 + 25, 100, 20), interactableList.interactables[index].name);

        // Sets toggle
        bool isToggleDown = GUI.Toggle(new Rect(5, index * 128 + 45, 100, 100), isActive, buttonContent, GUI.skin.button);

        // If this button is clicked and it wasn't clicked before; button was just pressed
        if (isToggleDown && !isActive)
        {
            // Remove focus from whatever was previously selected
            GUI.FocusControl(null);

            SelectedInteractable = index;

            ConvertTreeToNodes(index);
        }
    }

    /// <summary>
    /// Saves changes made in Node Editor to interactable
    /// </summary>
    void SaveDialogue(int index)
    {

    }

    /// <summary>
    /// Gets previous dialogue info from GameObject and displays it.
    /// Only needed if using Node Editor with object for the first time.
    /// </summary>
    /// <param name="index"></param>
    void GetPreviousDialogue(int index)
    {
        // TODO: COMMENT EVERYTHING
        // Create the nodes that could be added to the list
        DialogueNode prevDialogue = null;
        ChoiceNode prevChoices = null;

        // If interactable has dialogue before a choice
        if (interactableList.interactables[index].script.dialogue.sentences.Count != 0)
        {
            // Creates a new DialogueNode and sets its windowRect properties
            prevDialogue = new DialogueNode();
            prevDialogue.windowRect = new Rect(110, 0, 300, 300);

            // Centers the node in the middle of the window right next to sidebar
            prevDialogue.windowRect.center = new Vector2(prevDialogue.windowRect.center.x, position.height / 2);

            // Not sure if needed; Adds the dialogue to node
            prevDialogue.NumText = interactableList.interactables[index].script.dialogue.sentences.Count;
            prevDialogue.Sentences.AddRange(interactableList.interactables[index].script.dialogue.sentences);

            // Sets its index
            prevDialogue.index = 0; // 0 since this will be the first item added

            // Adds node to list of windows to display
            windows.Add(prevDialogue);
        }

        // If interactable prompts a choice
        if (interactableList.interactables[index].script.dialogue.choices.Count != 0)
        {
            // Assigns prevChoices (declared above) to a new CHoiceNode and sets its windowRect properties
            prevChoices = new ChoiceNode();
            prevChoices.windowRect = new Rect(110, 0, 300, 300);

            // Centers the node in the middle of the window right next to sideba
            prevChoices.windowRect.center = new Vector2(prevChoices.windowRect.center.x, position.height / 2);
            prevChoices.Prompt = interactableList.interactables[index].script.dialogue.question;

            // Sets its index
            prevChoices.index = 0; // Stays 0 if there is no dialogue 

            // If there is dialogue 
            if (prevDialogue != null)
            {
                // Moves the window 20 units to the right of the dialogue windowRect
                prevChoices.windowRect.x = prevDialogue.windowRect.x + prevDialogue.windowRect.width + 20;

                // Resets index of choice
                prevChoices.index = 1; // Second element in list

                // Sets input and output of both dialogue and choice nodes
                prevDialogue.Outputs.Add(prevChoices);
                prevDialogue.OutputRects.Add(prevChoices.windowRect);
                prevChoices.Inputs.Add(prevDialogue);
                prevChoices.InputRects.Add(prevDialogue.windowRect);
            }

            // Adds the possible choices to choiceNode
            prevChoices.NumChoices = interactableList.interactables[index].script.dialogue.choices.Count;
            prevChoices.Choices.AddRange(interactableList.interactables[index].script.dialogue.choices);

            // Creates areas for each possible choice
            for (int i = 0; i < prevChoices.NumChoices; i++)
            {
                prevChoices.ChoiceRects.Add(new Rect());
            }
            prevChoices.AddChoiceRects(); // Sets them to appropriate size and location

            // Adds choice node to list of windows to display
            windows.Add(prevChoices);

            // Converts the follow up dialogue showed after a choice is made into their own node
            for (int i = 0; i < interactableList.interactables[index].script.dialogue.altDialogue.Count; i++)
            {
                // Creates a node for the followup dialogue and sets its windowRect
                DialogueNode followUps = new DialogueNode();
                followUps.windowRect = new Rect(0, 0, 300, 300);

                // Location is based on the number of followups and stacks them them on top of each other
                followUps.windowRect.x = prevChoices.windowRect.x + prevChoices.windowRect.width + 20;
                float totalHeight = prevChoices.windowRect.height * interactableList.interactables[i].script.dialogue.altDialogue.Count;
                followUps.windowRect.y = prevChoices.windowRect.center.y - totalHeight / 2.0f + (300 * i);

                followUps.NumText = 1; // Only one sentence
                followUps.index = windows.Count; // Sets index to last in windows list

                // Adds the dialogue to the node and sets its input
                followUps.Sentences.Add(interactableList.interactables[index].script.dialogue.altDialogue[i]);
                prevChoices.Outputs.Add(followUps);
                prevChoices.OutputRects.Add(followUps.windowRect);
                prevChoices.ChoiceNodePair.Add(i, followUps); // Connects choice to resulting followup
                followUps.Inputs.Add(prevChoices);
                followUps.InputRects.Add(prevChoices.windowRect);

                // Adds followup to window list to display
                windows.Add(followUps); 
            }
        }
    }

    /// <summary>
    /// Converts dialogueTree from interactable and turns them into nodes for visualization
    /// </summary>
    /// <returns>dialogueTree as nodes</returns>
    void ConvertTreeToNodes(int index)
    {
        windows.Clear();

        List<DialogueType> currentTree = interactableList.interactables[index].script.dialogueTree;
        
        // Converts the dialogue types into nodes and adds them to list
        foreach (DialogueType dialogueType in currentTree)
        {
            DialogueNode newDialogue = null;
            ChoiceNode newChoice = null;

            // Creates nodes based on the data from the serialized tree
            if (dialogueType.dialogueType == "Dialogue")
            {
                newDialogue = new DialogueNode()
                {
                    windowRect = dialogueType.windowRect,
                    index = dialogueType.index,
                    Sentences = dialogueType.sentences,
                    NumText = dialogueType.sentences.Count,
                    InputRects = dialogueType.inputRects,
                    OutputRects = dialogueType.outputRects
                };

                windows.Add(newDialogue);
            }
            else if (dialogueType.dialogueType == "Choice")
            {
                newChoice = new ChoiceNode()
                {
                    windowRect = dialogueType.windowRect,
                    index = dialogueType.index,
                    NumChoices = dialogueType.choiceNum,
                    Prompt = dialogueType.question,
                    Choices = dialogueType.choices,
                    InputRects = dialogueType.inputRects,
                    OutputRects = dialogueType.outputRects,
                    ChoiceRects = dialogueType.choiceRects
                };

                windows.Add(newChoice);
            }
        }

        // Connects nodes based on info from the dialogue type
        for (int i = 0; i < windows.Count; i++)
        {
            DialogueType dT = currentTree[i];

            // Sets inputs
            for (int j = 0; j < dT.inputIndexes.Count; j++)
            {
                windows[i].inputs.Add(windows[dT.inputIndexes[j]]);
            }

            // Sets outputs
            for (int j = 0; j < dT.outputIndexes.Count; j++)
            {
                windows[i].outputs.Add(windows[dT.outputIndexes[j]]);
            }

            if (windows[i] is ChoiceNode)
            {
                // Copy of node to edit
                ChoiceNode choiceNode = windows[i] as ChoiceNode;

                // Sets outputs with corresponding choice
                for (int j = 0; j < dT.choiceDialogueKeys.Count; j++)
                {
                    choiceNode.ChoiceNodePair.Add(j, windows[dT.choiceDialogueValues[j]]);
                }

                // Rewrites previous node
                windows[i] = choiceNode;
            }
        }
    }
}
