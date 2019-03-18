﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InteractableList))]
public class InteractableListEditor : Editor {

    public override void OnInspectorGUI()
    {
        InteractableList m_target = (InteractableList)target;

        base.OnInspectorGUI();

        foreach (InteractableObjects objects in m_target.interactables)
        {
            if (objects.script != null)
            {
                objects.prefab = objects.script.gameObject;
            }
            else if (objects.prefab != null)
            {
                objects.script = objects.prefab.GetComponent<Interactable>();
            }
            objects.name = objects.prefab.name;
        }
    }
}
