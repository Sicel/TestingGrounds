using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Will need the messages Unity sends to ScriptableObjects (OnDestroy, OnEnable, etc.)
public abstract class BaseNode : ScriptableObject {

    public Rect windowRect; // Node Window

    public bool hasInputs = false; // Indicates if node has inputs 

    public bool hasOutputs = false; // Indicates if node has outputs

    public int index; // Position in list

    public List<BaseNode> inputs;
    public List<Rect> inputRects;
    
    public List<BaseNode> outputs;
    public List<Rect> outputRects;

    public string windowTitle = "";

    /// <summary>
    /// Draws node window
    /// </summary>
    public virtual void DrawWindow()
    {
        //windowTitle = EditorGUILayout.TextField("Title", windowTitle);
    }

    /// <summary>
    /// Draws curves from nodes
    /// </summary>
    public abstract void DrawCurves();

    /// <summary>
    /// Used by sub-classes; called when clicked on window during transition
    /// Optional for nodes that don't have inputs
    /// </summary>
    /// <param name="input">Node being set as input</param>
    /// <param name="clickPos">Where mouse left-clicked</param>
    public virtual void SetInput(BaseNode input, Vector2 clickPos) { }

    /// <summary>
    /// Used by dialogue and choice node to set up outputs
    /// </summary>
    /// <param name="output">Node being set as output</param>
    /// <param name="clickPos">Where mouse right-clicked</param>
    public virtual void SetOutput(BaseNode output, Vector2 clickPos) { }


    public virtual void NodeDeleted(BaseNode node) { }

    // Called when click happens on "input" in window, otherwise returns null
    public virtual BaseNode ClickedOnInput(Vector2 pos)
    {
        return null;
    }
}
