using UnityEngine;

public class QuitGame : MonoBehaviour
{
   
    public void ConfirmQuit()
    {
        Debug.Log("Spiel wird beendet.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
