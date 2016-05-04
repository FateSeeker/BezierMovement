
using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(PathEditorBehaviour))]
public class PathEditorBehaviour_Editor : Editor
{
    PathEditorBehaviour Target;

    public string LastControlName="";
    void OnEnable()
    {
        Target = (PathEditorBehaviour)target;
        
    }

    public override void OnInspectorGUI()
    {

        EditorGUI.BeginChangeCheck();

        Target.ShowPath = EditorGUILayout.Toggle("Show Path", Target.ShowPath);



        Target.ActivateEditing = EditorGUILayout.Toggle("Activate Editting", Target.ActivateEditing && Target.ShowPath);
        Target.EditTangents = EditorGUILayout.Toggle("Position / Tangents", Target.EditTangents);
        Target.SmoothTangents = EditorGUILayout.Toggle("Smooth Path Tangents",Target.SmoothTangents );
        Target.AccurateEdit = EditorGUILayout.Toggle("Accurate Edit Mode",Target.AccurateEdit);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("PathToEdit"), true);
        EditorGUILayout.Space();
        if(Target.PathToEdit!=null)
        {
            Target.PathToEdit.Offset = Target.transform.position;
            EditorGUILayout.Space();
            Editor e = Editor.CreateEditor(Target.PathToEdit);
            
            e.OnInspectorGUI();
            e.serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            Target.PathToEdit = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(Target.PathToEdit), typeof(Path)) as Path;
        }

        EditorGUI.EndChangeCheck();
        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }

       
    }

    void OnSceneGUI()
    {
        Target.PathToEdit.UpdateOffset(Target.transform.position);

        GUI.FocusControl("");

        if(Target.PathToEdit!=null && Target.ShowPath)
        {            
            for (int i = 0; i<Target.PathToEdit.Segments.Count;i++)
            {
                Path_Segment segment = Target.PathToEdit.Segments[i];
                if (Target.ActivateEditing )
                {


                    Selection.activeGameObject = Target.transform.gameObject;



                    UnityEngine.Random.seed = i;

                    Handles.color = UnityEngine.Random.ColorHSV();



                    if (Target.PathToEdit.Segments.Count > 1)
                    {
                        segment.Start = Target.PathToEdit.Segments[(i - 1 < 0) ? Target.PathToEdit.Segments.Count - 1 : i - 1].End;
                        segment.End = Target.PathToEdit.Segments[(i + 1 > Target.PathToEdit.Segments.Count - 1) ? 0 : i + 1].Start;

                        if(Target.SmoothTangents)
                        {
                            Vector3 nD = Quaternion.Euler(0, 0, 180) * Target.PathToEdit.Segments[(i - 1 < 0) ? Target.PathToEdit.Segments.Count - 1 : i - 1].TangentB.normalized;
                            nD *= segment.TangentA.magnitude;
                            segment.TangentA = nD;

                            nD = Quaternion.Euler(0, 0, 180) * Target.PathToEdit.Segments[(i + 1 > Target.PathToEdit.Segments.Count - 1) ? 0 : i + 1].TangentA.normalized;
                            nD *= segment.TangentB.magnitude;
                            segment.TangentB = nD;
                        }
                    }


                    GUI.SetNextControlName("Segment" + i + "Start");
                    segment.Start = Handles.FreeMoveHandle(segment.Start + segment.Offset, Quaternion.identity, .3f, Vector3.one * 2, Handles.CubeCap) - segment.Offset;

                    segment.Start.x *= 100;
                    segment.Start.x = Mathf.Round(segment.Start.x);
                    segment.Start.x /= 100;
                    segment.Start.y *= 100;
                    segment.Start.y = Mathf.Round(segment.Start.y);
                    segment.Start.y /= 100;

                    GUI.SetNextControlName("Segment" + i + "End");
                    segment.End = Handles.FreeMoveHandle(segment.End + segment.Offset, Quaternion.identity, .3f, Vector3.one * 2, Handles.CubeCap) - segment.Offset;

                    segment.End.x *= 100;
                    segment.End.x = Mathf.Round(segment.End.x);
                    segment.End.x /= 100;
                    segment.End.y *= 100;
                    segment.End.y = Mathf.Round(segment.End.y);
                    segment.End.y /= 100;

                    GUI.SetNextControlName("Segment" + i + "TangentA");
                    segment.TangentA = Handles.FreeMoveHandle(segment.TangentA + segment.Start + segment.Offset, Quaternion.identity, .3f, Vector3.one * 2, Handles.CubeCap) - segment.Offset - segment.Start;
                    segment.TangentA.x *= 100;
                    segment.TangentA.x = Mathf.Round(segment.TangentA.x);
                    segment.TangentA.x /= 100;
                    segment.TangentA.y *= 100;
                    segment.TangentA.y = Mathf.Round(segment.TangentA.y);
                    segment.TangentA.y /= 100;

                    GUI.SetNextControlName("Segment" + i + "TangentB");
                    segment.TangentB = Handles.FreeMoveHandle(segment.TangentB + segment.End + segment.Offset, Quaternion.identity, .3f, Vector3.one * 2, Handles.CubeCap) - segment.Offset - segment.End;
                    segment.TangentB.x *= 100;
                    segment.TangentB.x = Mathf.Round(segment.TangentB.x);
                    segment.TangentB.x /= 100;
                    segment.TangentB.y *= 100;
                    segment.TangentB.y = Mathf.Round(segment.TangentB.y);
                    segment.TangentB.y /= 100;

                    Handles.DrawLine(segment.Start + segment.Offset, segment.TangentA + segment.Start + segment.Offset);
                    Handles.DrawLine(segment.End + segment.Offset, segment.TangentB + segment.End + segment.Offset);

                }



                if (Target.ShowPath)
                    Handles.DrawBezier(segment.Start + segment.Offset, segment.End + segment.Offset, segment.TangentA + segment.Start + segment.Offset, segment.TangentB + segment.End + segment.Offset, Handles.color, null, 10f);

                LastControlName = (GUI.GetNameOfFocusedControl().Contains("Segment")) ? GUI.GetNameOfFocusedControl():LastControlName;

                segment.Longitude = segment.SetLongitude();

            }
            Repaint();
        }
    }
}

