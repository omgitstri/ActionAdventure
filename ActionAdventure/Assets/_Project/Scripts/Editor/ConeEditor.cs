using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Cone))]
public class ConeEditor : Editor
{
    Cone editObject = null;

    private void OnEnable()
    {
        editObject = (Cone)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

    }

    private void OnSceneGUI()
    {


    }
}