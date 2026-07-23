using UnityEngine;
using TMPro;

public class GameHUD : MonoBehaviour
{
    [Header("HUD Texts")]
    public TMP_Text timerText;
    public TMP_Text pointsText;
    public TMP_Text opponentText;
    public TMP_Text playerLevelText;
    public TMP_Text currentNodeText;
    public TMP_Text perkText;

    [Header("Game Values")]
    public float levelDuration = 120f;

    private float remainingTime;
    private int points;
    private int playerLevel = 1;
    private int opponentLevel = 1;

    private string currentNodeColor = "Grün";
    private string currentPerk = "Kleine Teile";

    void Start()
    {
        remainingTime = levelDuration;
        RefreshHUD();
    }

    void Update()
    {
        if (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime < 0f)
                remainingTime = 0f;

            UpdateTimer();
        }
    }

    private void UpdateTimer()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);

        timerText.text = $"Zeit: {minutes:00}:{seconds:00}";
    }

    private void RefreshHUD()
    {
        UpdateTimer();

        if (pointsText != null)
            pointsText.text = $"Punkte: {points}";

        if (playerLevelText != null)
            playerLevelText.text = $"Du: Level {playerLevel}";

        if (opponentText != null)
            opponentText.text = $"Gegner: Level {opponentLevel}";

        if (currentNodeText != null)
            currentNodeText.text = $"Aktueller Knoten: {currentNodeColor}";

        if (perkText != null)
            perkText.text = $"Perk: {currentPerk}";
    }

    public void StartLevel(
        int newPlayerLevel,
        string nodeColor,
        string perkName,
        float duration)
    {
        playerLevel = newPlayerLevel;
        currentNodeColor = nodeColor;
        currentPerk = perkName;
        levelDuration = duration;
        remainingTime = duration;

        RefreshHUD();
    }

    public void AddPoints(int amount)
    {
        points += amount;

        if (pointsText != null)
            pointsText.text = $"Punkte: {points}";
    }

    public void SetOpponentLevel(int level)
    {
        opponentLevel = level;

        if (opponentText != null)
            opponentText.text = $"Gegner: Level {opponentLevel}";
    }

    public void SetPlayerLevel(int level)
    {
        playerLevel = level;

        if (playerLevelText != null)
            playerLevelText.text = $"Du: Level {playerLevel}";
    }

    public void SetCurrentNode(string nodeColor)
    {
        currentNodeColor = nodeColor;

        if (currentNodeText != null)
            currentNodeText.text =
                $"Aktueller Knoten: {currentNodeColor}";
    }

    public void SetPerk(string perkName)
    {
        currentPerk = perkName;

        if (perkText != null)
            perkText.text = $"Perk: {currentPerk}";
    }

    public bool IsTimeFinished()
    {
        return remainingTime <= 0f;
    }
}