using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Interactable))]
[CanEditMultipleObjects]
public class InteractableEditor : Editor {

    bool showTree = true; // Shows elements in dialogue tree
    bool showElement = true; // Shows element's information

    public override void OnInspectorGUI()
    {
        SerializedProperty dialogueTree = serializedObject.FindProperty("dialogueTree");
        SerializedProperty notStarted = serializedObject.FindProperty("dialogueNotStarted");

        EditorGUILayout.PropertyField(notStarted);

        // Shows elements in dialogue tree
        showTree = EditorGUILayout.Foldout(showTree, "Dialogue Tree", true);
        if (showTree)
        {
            EditorGUI.indentLevel++;
            for (int i = 0; i < dialogueTree.arraySize; i++)
            {
                // Gets the parts of the node that will be displayed
                SerializedProperty currentDialogueType = dialogueTree.GetArrayElementAtIndex(i);
                SerializedProperty index = currentDialogueType.FindPropertyRelative("index");
                SerializedProperty dialogueType = currentDialogueType.FindPropertyRelative("dialogueType");
                SerializedProperty inputCount = currentDialogueType.FindPropertyRelative("inputCount");
                SerializedProperty outputCount = currentDialogueType.FindPropertyRelative("outputCount");
                SerializedProperty sentences = currentDialogueType.FindPropertyRelative("sentences");
                SerializedProperty question = currentDialogueType.FindPropertyRelative("question");
                SerializedProperty choices = currentDialogueType.FindPropertyRelative("choices");
                SerializedProperty choiceNum = currentDialogueType.FindPropertyRelative("choiceNum");

                // Shows specific element in tree
                showElement = EditorGUILayout.Foldout(showElement, index.intValue + ": " + dialogueType.stringValue, true);
                if (showElement)
                {
                    EditorGUILayout.LabelField("Number of Inputs: " + inputCount.intValue);
                    EditorGUILayout.LabelField("Number of Outputs: " + outputCount.intValue);

                    EditorGUILayout.Separator();

                    // Only shows the parts that are necessary for each dialogue type
                    if (dialogueType.stringValue == "Dialogue")
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(sentences, true);
                        EditorGUI.indentLevel--;
                    }
                    else if (dialogueType.stringValue == "Choice")
                    {
                        EditorGUILayout.PropertyField(question);
                        EditorGUILayout.LabelField("Number of Choices: " + choiceNum.intValue);

                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(choices, true);
                        EditorGUI.indentLevel--;
                    }
                }
            }

            EditorGUI.indentLevel--;
        }
        // Saves dialogue tree
        serializedObject.ApplyModifiedProperties();
    }
}
