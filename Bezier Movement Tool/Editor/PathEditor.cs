using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    Path Target;
    void OnEnable()
    {
        Target = (Path)target;
    }
    public override void OnInspectorGUI()
    {

        EditorGUI.BeginChangeCheck();

        

        //EditorGUILayout.PropertyField(serializedObject.FindProperty("Segments"), true);

        Target.ShowSegmentsInfo = EditorGUILayout.Foldout(Target.ShowSegmentsInfo, "Segments");
        
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        if (Target.ShowSegmentsInfo)
        {
            
            foreach (Path_Segment Segment in Target.Segments)
            {
                EditorGUILayout.LabelField("Segment" + (Target.Segments.IndexOf(Segment)+1));
                Editor e = CreateEditor(Segment);
                if(e!=null)
                {
                    e.OnInspectorGUI();
                    
                    e.serializedObject.ApplyModifiedProperties();
                    
                }
                
            }
            
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();


        if (Target.Segments != null)
        {
            if (GUILayout.Button("+ Segment"))
            {

                Path_Segment newSegment = CreateInstance<Path_Segment>();
                
                if (Target.Segments.Count > 0)
                    newSegment.Initialize(Target.Segments[Target.Segments.Count - 1].Start,
                        Target.Segments[Target.Segments.Count - 1].End,
                        Target.Segments[Target.Segments.Count - 1].TangentA,
                        Target.Segments[Target.Segments.Count - 1].TangentB, Target.Segments[Target.Segments.Count - 1].Offset);
                else
                    newSegment.Initialize(Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Target.Offset);

                Target.Segments.Add(newSegment);
                newSegment.name = "Segment " + Target.Segments.IndexOf(newSegment);
                AssetDatabase.AddObjectToAsset(newSegment, Target);

            }
            if (GUILayout.Button("- Segment"))
            {
                if (Target.Segments.Count > 0)
                {
                    Path_Segment toDelete = Target.Segments[Target.Segments.Count - 1];
                    Target.Segments.Remove(toDelete);
                    DestroyImmediate(toDelete,true);
                }
                
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUI.EndChangeCheck();

        
        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            AssetDatabase.SaveAssets();
            Target.Segments.ForEach(s => EditorUtility.SetDirty(s));
        }
            
        
    }
}

