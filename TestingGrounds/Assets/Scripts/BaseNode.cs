using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class BaseNode : ScriptableObject {

    public Rect windowRect;

    public bool hasInputs = false;

    public string windowTitle = "";

    public virtual void DrawWindow()
    {
        windowTitle = EditorGUILayout.TextField("Title", windowTitle);
    }

    // Draws lines connecting nodes
    public abstract void DrawCurves();

    // Used for nodes that have an input when window is clicked durring transition
    public virtual void SetInput(BaseInputNode input, Vector2 clickPos)
    {

    }

    
    public virtual void NodeDeleted(BaseNode node) { }

    // When window clicked
    public virtual BaseInputNode ClickedOnInput(Vector2 Pos)
    {
        return null;
    }
}
