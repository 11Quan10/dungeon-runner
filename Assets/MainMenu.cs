using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void QuitGame()
    {
        // Exits the game in a built application
        Application.Quit();

        // Stops play mode in the Unity Editor
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #endif
    }
}
