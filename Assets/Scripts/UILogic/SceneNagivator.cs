using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigation : MonoBehaviour
{
    [Header("Pause Menu")]
    [SerializeField] private GameObject pausePopup;

    private void Start()
    {
        Time.timeScale = 1f;

        if (pausePopup != null)
        {
            pausePopup.SetActive(false);
        }
    }

    public void OpenMainScreen()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScreen");
    }

    public void OpenPathOverview()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("PathOverview");
    }

    public void OpenLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level");
    }

    public void OpenEndScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("EndScene");
    }

    public void ReplayLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level");
    }

    public void Pause()
    {
        if (pausePopup == null)
        {
            Debug.LogWarning("PausePopup is not working.");
            return;
        }

        pausePopup.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        if (pausePopup == null)
        {
            Debug.LogWarning("PausePopup is not working");
            return;
        }

        Time.timeScale = 1f;
        pausePopup.SetActive(false);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}