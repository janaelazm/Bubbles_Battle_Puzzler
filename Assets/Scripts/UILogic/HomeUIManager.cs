using UnityEngine;
using TMPro;

public class HomeUIManager : MonoBehaviour
{
    [Header("Main Panels")]
    public GameObject homePanel;
    public GameObject pathSelectionPanel;
    public GameObject gamePanel;
    public GameObject profilePanel;

    [Header("Popup")]
    public GameObject quitPopup;

    [Header("Profile Inputs")]
    public TMP_InputField firstNameInput;
    public TMP_InputField lastNameInput;
    public TMP_Text profileNameText;
    public TMP_Text profileConfirmationText;

    [Header("Path Page")]
    public TMP_Text pathPlayerInfoText;

    private string firstName = "Marie";
    private string lastName = "Muller";

    void Start()
    {
        LoadProfile();
        ShowHome();

        if (quitPopup != null)
            quitPopup.SetActive(false);
    }

    private void HideAllMainPanels()
    {
        if (homePanel != null)
            homePanel.SetActive(false);

        if (pathSelectionPanel != null)
            pathSelectionPanel.SetActive(false);

        if (gamePanel != null)
            gamePanel.SetActive(false);

        if (profilePanel != null)
            profilePanel.SetActive(false);
    }

    public void ShowHome()
    {
        HideAllMainPanels();

        if (homePanel != null)
            homePanel.SetActive(true);
    }

    public void ShowPathSelection()
    {
        HideAllMainPanels();

        if (pathSelectionPanel != null)
            pathSelectionPanel.SetActive(true);

        UpdatePathPlayerInfo();
    }

    public void StartGame()
    {
        HideAllMainPanels();

        if (gamePanel != null)
            gamePanel.SetActive(true);
    }

    public void ShowProfile()
    {
        HideAllMainPanels();

        if (profilePanel != null)
            profilePanel.SetActive(true);

        if (firstNameInput != null)
            firstNameInput.text = firstName;

        if (lastNameInput != null)
            lastNameInput.text = lastName;

        if (profileConfirmationText != null)
            profileConfirmationText.text = "";
    }

    public void SaveProfile()
    {
        if (firstNameInput != null)
            firstName = firstNameInput.text.Trim();

        if (lastNameInput != null)
            lastName = lastNameInput.text.Trim();

        if (string.IsNullOrWhiteSpace(firstName))
            firstName = "Spieler";

        if (string.IsNullOrWhiteSpace(lastName))
            lastName = "";

        PlayerPrefs.SetString("FirstName", firstName);
        PlayerPrefs.SetString("LastName", lastName);
        PlayerPrefs.Save();

        UpdateProfileTexts();

        if (profileConfirmationText != null)
            profileConfirmationText.text = "Profil gespeichert!";
    }

    private void LoadProfile()
    {
        firstName = PlayerPrefs.GetString("FirstName", "Marie");
        lastName = PlayerPrefs.GetString("LastName", "Muller");

        UpdateProfileTexts();
    }

    private void UpdateProfileTexts()
    {
        if (profileNameText != null)
        {
            profileNameText.text =
                $"Name: {lastName}\nVorname: {firstName}";
        }

        UpdatePathPlayerInfo();
    }

    private void UpdatePathPlayerInfo()
    {
        if (pathPlayerInfoText != null)
        {
            pathPlayerInfoText.text =
                $"Spieler: {firstName} {lastName}\n" +
                "Aktuelles Level: 1";
        }
    }

    public void OpenQuitPopup()
    {
        if (quitPopup != null)
            quitPopup.SetActive(true);
    }

    public void CloseQuitPopup()
    {
        if (quitPopup != null)
            quitPopup.SetActive(false);
    }

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