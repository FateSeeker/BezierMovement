

using UnityEngine;
using System.IO;


public static class ScriptableObjectUtils
{
#if UNITY_EDITOR
    public static T CreateAsset<T>() where T : ScriptableObject
    {
        
        T asset = ScriptableObject.CreateInstance<T>();

        string path = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject);

        if (path == "")
        {
            path = "Assets";
        }
        else if (System.IO.Path.GetExtension(path) != "")
        {
            path = path.Replace(System.IO.Path.GetFileName(UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject)), "");
        }

        string assetPathAndName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

        UnityEditor.AssetDatabase.CreateAsset(asset, assetPathAndName);

        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();

        UnityEditor.EditorUtility.FocusProjectWindow();

        UnityEditor.Selection.activeObject = asset;

        return asset;
    }
#endif
}

