using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Path_Segment))]
public class Path_SegmentEditor : Editor
{
    Path_Segment Target;
    void OnEnable()
    {
        Target = (Path_Segment)target;
    }
    public override void OnInspectorGUI()
    {

        Target.ShowDetails = EditorGUILayout.Foldout(Target.ShowDetails, "Details");
        if(Target.ShowDetails)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            Target.Start = EditorGUILayout.Vector3Field("Start", Target.Start + Target.Offset) - Target.Offset;
            Target.End = EditorGUILayout.Vector3Field("End", Target.End + Target.Offset) - Target.Offset;
            Target.TangentA = EditorGUILayout.Vector3Field("TangentA", Target.TangentA + Target.Offset) - Target.Offset;
            Target.TangentB = EditorGUILayout.Vector3Field("TangentB", Target.TangentB + Target.Offset) - Target.Offset;
            EditorGUILayout.FloatField("Longitude",Target.Longitude);
            EditorGUILayout.Vector3Field("Offset",Target.Offset);
            EditorGUILayout.EndVertical();

            EditorGUI.EndChangeCheck();

            
        }
        Target.Longitude = Target.SetLongitude();
        serializedObject.ApplyModifiedProperties();

        

        if (GUI.changed)
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
            
    }
}

