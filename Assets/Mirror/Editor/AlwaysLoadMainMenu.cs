
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public class AlwaysLoadMainMenu{
    [InitializeOnLoad]
public class EditorInit
{
    static EditorInit()
    {
        var editorSceneName = EditorSceneManager.GetActiveScene().name; // to be implemented: start with scene that is currently loaded in editor
        var pathOfFirstScene = EditorBuildSettings.scenes[0].path;
        var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);
        EditorSceneManager.playModeStartScene = sceneAsset;
    }
}
}