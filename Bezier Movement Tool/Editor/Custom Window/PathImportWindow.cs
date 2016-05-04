using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

public class PathImportWindow:EditorWindow {

    PathMotion Caller;

    List<bool> pathsFoldout;
    List<Path> pathsFounded;


    int selectedPathToAdd;

    List<string> addLabels;

    int selectedPathToDelete;

    List<string> deleteLabels;

	public void Init(PathMotion WhoCalls)
    {
        Caller = WhoCalls;
        pathsFoldout = new List<bool>();
        pathsFounded = new List<Path>();

        selectedPathToAdd = -1;

        addLabels = new List<string>();

        selectedPathToAdd = -1;

        deleteLabels = new List<string>();
        

        foreach(string t in AssetDatabase.FindAssets("t:Path"))
        {
            Path founded = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(t), typeof(Path)) as Path;
            pathsFounded.Add(founded);
            pathsFoldout.Add(false);
            
            //selectionPerPath2.Add(founded.Segments[0]);
            
            founded.Segments.ForEach(s => addLabels.Add("Path " + (pathsFounded.Count) + " Segment " + (founded.Segments.IndexOf(s) + 1)));
            
        }
    }

    void OnGUI()
    {
        //Debug.Log(position);





        GUILayout.BeginHorizontal();

        #region Path Segment Selection

        GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.MinWidth(position.width / 2));

        GUILayout.Label("Paths Assets");



        selectedPathToAdd = GUILayout.SelectionGrid(selectedPathToAdd, addLabels.ToArray(), 1, GUILayout.MaxWidth(position.width / 4));

        GUILayout.EndVertical();

        #endregion


        #region Path Selected View

        GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.MinWidth(position.width / 2));
        GUILayout.Label("Paths To Import");

        selectedPathToDelete = GUILayout.SelectionGrid(selectedPathToDelete, deleteLabels.ToArray(), 1, GUILayout.MaxWidth(position.width / 4));


        GUILayout.EndVertical();

        #endregion

        GUILayout.EndHorizontal();


        GUILayout.BeginVertical();


        GUILayout.Space(25f);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Selected Segment") && selectedPathToAdd != -1)
        {

            deleteLabels.Add(addLabels[selectedPathToAdd]);
            addLabels.Remove(addLabels[selectedPathToAdd]);
            selectedPathToAdd = -1;
            selectedPathToDelete = -1;
        }

        if (GUILayout.Button("Delete Selected Segment") && selectedPathToDelete != -1)
        {

            addLabels.Add(deleteLabels[selectedPathToDelete]);
            addLabels.Sort();
            deleteLabels.Remove(deleteLabels[selectedPathToDelete]);
            selectedPathToAdd = -1;
            selectedPathToDelete = -1;
        }

        if (GUILayout.Button("Up") && selectedPathToDelete > 0)
        {
            string bubble = deleteLabels[selectedPathToDelete - 1];
            deleteLabels[selectedPathToDelete - 1] = deleteLabels[selectedPathToDelete];
            deleteLabels[selectedPathToDelete] = bubble;
            selectedPathToDelete -= 1;
        }

        if (GUILayout.Button("Down") && selectedPathToDelete < deleteLabels.Count - 1)
        {
            string bubble = deleteLabels[selectedPathToDelete + 1];
            deleteLabels[selectedPathToDelete + 1] = deleteLabels[selectedPathToDelete];
            deleteLabels[selectedPathToDelete] = bubble;
            selectedPathToDelete += 1;
        }

        if (GUILayout.Button("Import Constructed Path"))
        {
            if(deleteLabels.Count > 0)
            {
                List<Path_Segment> segmentsSelected = new List<Path_Segment>();
                foreach (string label in deleteLabels)
                {
                    int segmentIndex = int.Parse(label.Split(" "[0])[3]);
                    int pathIndex = int.Parse(label.Split(" "[0])[1]);
                    Path_Segment segmente = pathsFounded[pathIndex - 1].Segments[segmentIndex - 1];
                    segmentsSelected.Add(segmente);
                    Debug.Log("drawing" + deleteLabels.Count);
                    
                }
                Path constructedPath = ScriptableObject.CreateInstance<Path>();
                constructedPath.Initialize(segmentsSelected, segmentsSelected[0].Offset);
            }
        }

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        

    }

    public void ShowSelectionOnScene()
    {
        List<Path_Segment> segmentsSelected = new List<Path_Segment>();
        foreach(string label in deleteLabels)
        {
            int segmentIndex = int.Parse(label.Split(" "[0])[3]);
            int pathIndex = int.Parse(label.Split(" "[0])[1]);
            Path_Segment segmente = pathsFounded[pathIndex-1].Segments[segmentIndex-1];
            segmentsSelected.Add(segmente);
            Debug.Log("drawing" + deleteLabels.Count);
            Handles.DrawBezier(segmente.Start + segmente.Offset, segmente.End + segmente.Offset, segmente.TangentA + segmente.Start + segmente.Offset, segmente.TangentB + segmente.End + segmente.Offset, Color.yellow, null, 10f);
        }
        SceneView.RepaintAll();
    }

}
