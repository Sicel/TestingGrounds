using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueTree))]
public class DialogueTreeEditor : Editor {

    DialogueTree m_target;
    public override void OnInspectorGUI()
    {
        m_target = (DialogueTree)target;

        DrawDefaultInspector();
        DrawDialogueInspector();
        DrawDialogue();
    }

    void DrawDialogueInspector()
    {
        GUILayout.Space(10);
            DrawAddNodeButton();
    }

    void DrawDialogue()
    {
        EditorGUI.BeginChangeCheck();
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(m_target, "Modify Tree");

            EditorUtility.SetDirty(m_target);
        }
    }

    void DrawStartTreeButton()
    {
        if (GUILayout.Button("Begin Tree", GUILayout.Height(20)))
        {
            Undo.RecordObject(m_target, "Begin Tree");
            m_target.StartTree(new DialogueNode());
            EditorUtility.SetDirty(m_target);
        }
    }

    void DrawAddNodeButton()
    {
        if (GUILayout.Button("Add New Node", GUILayout.Height(20)))
        {
            Undo.RecordObject(m_target, "Add New Node");
            m_target.tree.Add(new DialogueNode());
            EditorUtility.SetDirty(m_target);
        }
    }
}
