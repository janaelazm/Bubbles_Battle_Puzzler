using UnityEngine;
using TMPro;

public class HomeUIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject homePanel;
    public GameObject gamePanel;
    public GameObject difficultyPanel;
    public GameObject profilePanel;

    [Header("Profile")]
    public TMP_Text profileNameText;
    public TMP_Text selectedDifficultyText;

    private string selectedDifficulty = "Leicht";

    void Start()
    {
        ShowHome();

        profileNameText.text = "Name: Muller\nVorname: Marie";
        selectedDifficultyText.text = "Level: " + selectedDifficulty;
    }

    public void ShowHome()
    {
        homePanel.SetActive(true);
        gamePanel.SetActive(false);
        difficultyPanel.SetActive(false);
        profilePanel.SetActive(false);
    }

    public void PlayGame()
    {
        homePanel.SetActive(false);
        gamePanel.SetActive(true);
        difficultyPanel.SetActive(false);
        profilePanel.SetActive(false);
    }

    public void ShowDifficulty()
    {
        homePanel.SetActive(false);
        difficultyPanel.SetActive(true);
    }

    public void ShowProfile()
    {
        homePanel.SetActive(false);
        profilePanel.SetActive(true);
    }

    public void SetDifficultyEasy()
    {
        selectedDifficulty = "Leicht 🟢";
        selectedDifficultyText.text = "Level: " + selectedDifficulty;
        ShowHome();
    }

    public void SetDifficultyMedium()
    {
        selectedDifficulty = "Mittel 🟡";
        selectedDifficultyText.text = "Level: " + selectedDifficulty;
        ShowHome();
    }

    public void SetDifficultyHard()
    {
        selectedDifficulty = "Schwer 🔴";
        selectedDifficultyText.text = "Level: " + selectedDifficulty;
        ShowHome();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}