using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EImageSelector))]
public class EImageSelectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EImageSelector imageSelector = (EImageSelector)target;
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Set All Images to Source Sprite"))
        {
            imageSelector.SetAllImagesToSource();
            EditorUtility.SetDirty(imageSelector);
        }
    }
}
