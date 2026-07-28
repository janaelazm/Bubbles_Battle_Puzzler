using UnityEngine;

public class HowToPlayUI : MonoBehaviour
{
    [SerializeField] private GameObject howToPlayPanel;

    private void Start()
    {
        howToPlayPanel.SetActive(false);
    }

    public void OpenHowToPlay()
    {
        howToPlayPanel.SetActive(true);
    }

    public void CloseHowToPlay()
    {
        howToPlayPanel.SetActive(false);
    }
}
