using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileUI : MonoBehaviour
{
    [SerializeField] private  TMP_Text playerName;
    [SerializeField] private  Button NameEditButton;
    [SerializeField] private  TMP_InputField NameEditField;
    [SerializeField] private Image Avatar;
 
    public void ChangeName()
    {
        string newName = NameEditField.text;

        if (string.IsNullOrWhiteSpace(newName))
            return;

        PlayerProfile.Instance.SetName(newName);

        playerName.text = "Hello, " + PlayerProfile.Instance.PlayerName + "!";

        NameEditField.gameObject.SetActive(false);
    }

    public void OpenNameField()
    {
        NameEditField.gameObject.SetActive(true);
        NameEditField.Select();
        NameEditField.ActivateInputField();
    }

    public void initPlayerUI()
    {
        if (PlayerProfile.Instance != null)
            playerName.text = "Hello, " + PlayerProfile.Instance.PlayerName + "!";
        NameEditButton.gameObject.SetActive(true);
        NameEditField.gameObject.SetActive(false);
        NameEditButton.image.color = PlayerProfile.Instance.PlayerColor;
        Avatar.color = PlayerProfile.Instance.PlayerColor;
    }
}
