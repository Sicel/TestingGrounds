using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InputNode : BaseInputNode {

    private InputType inputType;

    public enum InputType
    {
        Number, // Given number
        Randomization // Random number from range
    }

    // Set random values
    private string randomFrom = "";
    private string randomTo = "";

    // Number to be given
    private string inputValue = "";

    // Default window property
    public InputNode()
    {
        windowTitle = "Input Node";
    }

    public override void DrawWindow()
    {
        base.DrawWindow();

        inputType = (InputType)EditorGUILayout.EnumPopup("Input Type: ", inputType);

        if (inputType == InputType.Number)
        {
            inputValue = EditorGUILayout.TextField("Value", inputValue);
        }
        else if (inputType == InputType.Randomization)
        {
            randomFrom = EditorGUILayout.TextField("From", randomFrom);
            randomTo = EditorGUILayout.TextField("To", randomTo);

            // Prevents random number from being generated every frame
            if (GUILayout.Button("Calculate Button"))
            {
                CalculateRandom();
            }
        }
    }

    public override void DrawCurves()
    {
        base.DrawCurves();
    }

    private void CalculateRandom()
    {
        float rFrom = 0;
        float rTo = 0;

        // Tries to turn our strings into floats
        float.TryParse(randomFrom, out rFrom);
        float.TryParse(randomTo, out rTo);

        // Gets rid of decimals
        int randFrom = (int)(rFrom * 10);
        int randTo = (int)(rTo * 10);

        int selected = UnityEngine.Random.Range(randFrom, randTo + 1); // + 1 takes exact number user inputed

        // Returns it to float value
        float selectedValue = selected / 10;

        inputValue = selectedValue.ToString();
    }

    public override string GetResult()
    {
        return inputValue;//.ToString();
    }
}

