using UnityEngine;
using UnityEngine.UI;

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

        PlayerColor = new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f)
        );

        DontDestroyOnLoad(gameObject);
    }
    public void SetName(string name)
    {
        PlayerName = name;
        PlayerPrefs.SetString("PlayerName", name);
    }
    public void Load()
    {
        PlayerName = PlayerPrefs.GetString("PlayerName", "Puzzler");
        Debug.Log(PlayerName);
    }
}