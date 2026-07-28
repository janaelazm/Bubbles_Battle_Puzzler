using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileUI : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private Button nameEditButton;
    [SerializeField] private TMP_InputField nameEditField;
    [SerializeField] private Image avatar;

    private void Start()
    {
        InitPlayerUI();

        nameEditField.onSubmit.AddListener(ChangeName);
        nameEditField.onEndEdit.AddListener(OnNameEditEnded);
    }

    private void OnDestroy()
    {
        nameEditField.onSubmit.RemoveListener(ChangeName);
        nameEditField.onEndEdit.RemoveListener(OnNameEditEnded);
    }

    public void OpenNameField()
    {
        if (PlayerProfile.Instance == null)
            return;

        playerNameText.gameObject.SetActive(false);
        nameEditField.gameObject.SetActive(true);

        nameEditField.text = PlayerProfile.Instance.PlayerName;

        nameEditField.Select();
        nameEditField.ActivateInputField();

        nameEditField.caretPosition = nameEditField.text.Length;
        nameEditField.selectionAnchorPosition = nameEditField.text.Length;
        nameEditField.selectionFocusPosition = nameEditField.text.Length;
    }

    private void ChangeName(string newName)
    {
        SaveAndCloseNameField(newName);
    }

    private void OnNameEditEnded(string newName)
    {
        SaveAndCloseNameField(newName);
    }

    private void SaveAndCloseNameField(string newName)
    {
        newName = newName.Trim();

        if (!string.IsNullOrWhiteSpace(newName))
        {
            PlayerProfile.Instance.SetName(newName);
            UpdateNameText();
        }

        nameEditField.gameObject.SetActive(false);
        playerNameText.gameObject.SetActive(true);
    }

    private void UpdateNameText()
    {
        playerNameText.text = PlayerProfile.Instance.PlayerName;
    }

    public void InitPlayerUI()
    {
        if (PlayerProfile.Instance == null)
            return;

        UpdateNameText();

        playerNameText.gameObject.SetActive(true);
        nameEditButton.gameObject.SetActive(true);
        nameEditField.gameObject.SetActive(false);

        avatar.color = PlayerProfile.Instance.PlayerColor;
    }
}