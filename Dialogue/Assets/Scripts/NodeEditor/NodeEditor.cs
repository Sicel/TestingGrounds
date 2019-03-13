using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeEditor : EditorWindow {

    public List<BaseNode> windows = new List<BaseNode>(); // Nodes being displayed
    public List<DialogueType> dialogueTree = new List<DialogueType>();

    private Vector2 mousePos;
    private Vector2 rightClickPos;

    Rect box = new Rect(0, 0, 95, 35); // Side bar rectangle

    private BaseNode selectedNode;

    private bool makeTransitionMode = false;

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
                    menu.AddItem(new GUIContent("Add Input Node"), false, ContextCallback, "inputNode");
                    //menu.AddItem(new GUIContent("Add Output Node"), false, ContextCallback, "outputNode");
                    menu.AddItem(new GUIContent("Add Calculation Node"), false, ContextCallback, "calcNode");
                    menu.AddItem(new GUIContent("Add Comparison Node"), false, ContextCallback, "comparisonNode");
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
                windows[selectedIndex].SetInput((BaseInputNode)selectedNode, mousePos);
                windows[windows.IndexOf(selectedNode)].SetOutput((BaseNode)windows[selectedIndex], rightClickPos);
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
            
            if (clickedOnWindow)
            {
                BaseInputNode nodeToChange = windows[selectedIndex].ClickedOnInput(mousePos);

                if (nodeToChange != null)
                {
                    selectedNode = nodeToChange;
                    makeTransitionMode = true;
                }

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
            foreach (BaseNode node in windows)
            {
                Debug.Log(node);
            }
            foreach (DialogueType d in dialogueTree)
            {
                Debug.Log(d);
            }
            windows.Clear();
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
        if (clb.Equals("inputNode"))
        {
            InputNode inputNode = new InputNode();
            inputNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 100);

            windows.Add(inputNode);
        }
        //else if(clb.Equals("outputNode"))
        //{
        //    OutputNode outputNode = new OutputNode();
        //    outputNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 100);

        //    windows.Add(outputNode);
        //}
        else if (clb.Equals("calcNode"))
        {
            CalcNode calcNode = new CalcNode();
            calcNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 100);

            windows.Add(calcNode);
        }
        else if (clb.Equals("comparisonNode"))
        {
            ComparisonNode comparisonNode = new ComparisonNode();
            comparisonNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 100);

            windows.Add(comparisonNode);
        }
        else if (clb.Equals("dialogueNode"))
        {
            DialogueNode dialogueNode = new DialogueNode();
            dialogueNode.windowRect = new Rect(mousePos.x, mousePos.y, 300, 300);

            windows.Add(dialogueNode);
            dialogueTree.Add(dialogueNode.Dialogue);
        }
        else if (clb.Equals("choiceNode"))
        {
            ChoiceNode choiceNode = new ChoiceNode();
            choiceNode.windowRect = new Rect(mousePos.x, mousePos.y, 300, 300);

            windows.Add(choiceNode);
            dialogueTree.Add(choiceNode.Choice);
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

            if (clickedOnWindow)
            {
                BaseNode selNode = windows[selectedIndex];
                windows.RemoveAt(selectedIndex);
                dialogueTree.RemoveAt(selectedIndex);

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
            for (int i = 0; i < interactableList.interactables.Count; i++)
            {
                windows.Clear();
                //interactableList.interactables[i].script.Windows.Clear();
                GUI.FocusControl(null);
            }
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
            windows = ConvertToNodes(interactableList.interactables[index].script.dialogueTree);
            dialogueTree = interactableList.interactables[index].script.dialogueTree;
            //if (windows.Count == 0)
            //{
            //    if (interactableList.interactables[index].script.dialogueTree.Count == 0)
            //    {
            //        GetPreviousDialogue(index);
            //    }
            //    windows = ConvertToNodes(interactableList.interactables[index].script.dialogueTree);
            //}
            if (GUI.Button(new Rect(5, 0, 100, 20), "Save"))
            {
                interactableList.interactables[index].script.SaveDialogue(dialogueTree);

                Debug.Log(dialogueTree.Count);

                GUI.FocusControl(null);
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

            dialogueTree = interactableList.interactables[index].script.dialogueTree;
        }
    }

    /// <summary>
    /// Saves changes made in Node Editor to interactable
    /// </summary>
    void SaveDialogue(int index)
    {

    }

    /// <summary>
    /// Gets previous dialogue info from GameObject. 
    /// Only needed if using Node Editor with object for the first time.
    /// </summary>
    /// <param name="index"></param>
    void GetPreviousDialogue(int index)
    {
        // TODO: COMMENT EVERYTHING
        DialogueNode prevDialogue = null;
        ChoiceNode prevChoices = null;

        if (interactableList.interactables[index].script.dialogue.sentences.Count != 0)
        {
            prevDialogue = new DialogueNode();
            prevDialogue.windowRect = new Rect(110, 0, 300, 300);
            prevDialogue.windowRect.center = new Vector2(prevDialogue.windowRect.center.x, position.height / 2);
            if (prevDialogue.Sentences != null)
            {
                prevDialogue.NumText = interactableList.interactables[index].script.dialogue.sentences.Count;
                prevDialogue.Sentences.AddRange(interactableList.interactables[index].script.dialogue.sentences);
            }
            else
            {
                prevDialogue.Sentences = new List<string>();
                prevDialogue.NumText = interactableList.interactables[index].script.dialogue.sentences.Count;
                prevDialogue.Sentences.AddRange(interactableList.interactables[index].script.dialogue.sentences);
            }
            //windows.Add(prevDialogue);
            interactableList.interactables[index].script.dialogueTree.Add(prevDialogue.Dialogue);
        }

        if (interactableList.interactables[index].script.dialogue.choices.Count != 0)
        {
            prevChoices = new ChoiceNode();
            prevChoices.windowRect = new Rect(110, 0, 300, 300);
            prevChoices.windowRect.center = new Vector2(prevChoices.windowRect.center.x, position.height / 2);
            prevChoices.Prompt = interactableList.interactables[index].script.dialogue.question;

            if (prevDialogue != null)
            {
                prevChoices.windowRect.x = prevDialogue.windowRect.x + prevDialogue.windowRect.width + 20;

                prevDialogue.Outputs.Add(prevChoices);
                prevDialogue.OutputRects.Add(prevChoices.windowRect);
                prevChoices.Inputs.Add(prevDialogue);
                prevChoices.InputRects.Add(prevDialogue.windowRect);
            }

            if (prevChoices.Choices != null)
            {
                prevChoices.NumChoices = interactableList.interactables[index].script.dialogue.choices.Count;
                prevChoices.Choices.AddRange(interactableList.interactables[index].script.dialogue.choices);
                for (int i = 0; i < prevChoices.NumChoices; i++)
                {
                    prevChoices.ChoiceRects.Add(new Rect());
                }
                prevChoices.AddChoiceRects();
            }
            else
            {
                prevChoices.Choices = new List<string>();
                prevChoices.NumChoices = interactableList.interactables[index].script.dialogue.choices.Count;
                prevChoices.Choices.AddRange(interactableList.interactables[index].script.dialogue.choices);
                for (int i = 0; i < prevChoices.NumChoices; i++)
                {
                    prevChoices.ChoiceRects.Add(new Rect());
                }
                prevChoices.AddChoiceRects();
            }

            //windows.Add(prevChoices);
            interactableList.interactables[index].script.dialogueTree.Add(prevChoices.Choice);

            for (int i = 0; i < interactableList.interactables[index].script.dialogue.altDialogue.Count; i++)
            {
                DialogueNode followUps = new DialogueNode();
                followUps.windowRect = new Rect(0, 0, 300, 300);
                followUps.windowRect.x = prevChoices.windowRect.x + prevChoices.windowRect.width + 20;
                float totalHeight = prevChoices.windowRect.height * interactableList.interactables[i].script.dialogue.altDialogue.Count;
                followUps.windowRect.y = prevChoices.windowRect.center.y - totalHeight / 2.0f + (300 * i);
                followUps.NumText = 1;

                followUps.Sentences.Add(interactableList.interactables[index].script.dialogue.altDialogue[i]);
                prevChoices.Outputs.Add(followUps);
                prevChoices.OutputRects.Add(followUps.windowRect);
                prevChoices.ChoiceNodePair.Add(i, followUps);
                followUps.Inputs.Add(prevChoices);
                followUps.InputRects.Add(prevChoices.windowRect);

                //windows.Add(followUps);
                interactableList.interactables[index].script.dialogueTree.Add(followUps.Dialogue);
            }
        }
    }

    List<BaseNode> ConvertToNodes(List<DialogueType> dialogues)
    {
        List<BaseNode> nodesToAdd = new List<BaseNode>();
        foreach (DialogueType dialogue in dialogues)
        {
            nodesToAdd.Add(dialogue.ConnectedNode);
        }
        return nodesToAdd;
    }
}
