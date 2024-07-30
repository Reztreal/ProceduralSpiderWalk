using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PointAtTargetEditor : EditorWindow
{
    private Transform objectToRotate;
    private Transform targetObject;

    [MenuItem("Tools/Point At Target")]
    public static void ShowWindow()
    {
        GetWindow<PointAtTargetEditor>("Point At Target");
    }

    private void OnGUI()
    {
        GUILayout.Label("Rotate object to point at target", EditorStyles.boldLabel);

        objectToRotate =
            (Transform)EditorGUILayout.ObjectField("Object To Rotate", objectToRotate, typeof(Transform), true);
        targetObject = (Transform)EditorGUILayout.ObjectField("Target Object", targetObject, typeof(Transform), true);

        if (GUILayout.Button("Rotate"))
        {
            if (objectToRotate is not null && targetObject is not null)
            {
                RotateObjectToTarget();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please assign both objects.", "OK");
            }
        }
    }

    private void RotateObjectToTarget()
    {
        objectToRotate.LookAt(targetObject);
        Debug.Log(objectToRotate.name + " is now pointing towards " + targetObject.name);
        objectToRotate = null;
        targetObject = null;
    }
}