using UnityEngine;

public class PlayerProfile : MonoBehaviour
{
    public static PlayerProfile Instance;

    public string PlayerName { get; private set; }
    public Color PlayerColor { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();

        PlayerColor = new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f)
        );
    }

    public void SetName(string newName)
    {
        PlayerName = newName;
        PlayerPrefs.SetString("PlayerName", newName);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        PlayerName = PlayerPrefs.GetString(
            "PlayerName",
            "Puzzler"
        );
    }
}