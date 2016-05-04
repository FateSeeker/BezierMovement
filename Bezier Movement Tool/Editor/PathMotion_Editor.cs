using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PathMotion))]
public class PathMotion_Editor : Editor {

    PathMotion Target;
    PathImportWindow importer;

    void OnEnable()
    {
        Target = (PathMotion)target;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Import Path"))
        {
            importer = EditorWindow.GetWindow<PathImportWindow>() as PathImportWindow;

            importer.Init(Target);
        }
    }

    void OnSceneGUI()
    {
        if(importer!=null)
        {
            importer.ShowSelectionOnScene();
        }
    }
}
