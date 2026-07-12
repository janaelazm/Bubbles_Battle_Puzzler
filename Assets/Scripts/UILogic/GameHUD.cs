using UnityEngine;
using TMPro;

public class GameHUD : MonoBehaviour
{
    public TMP_Text timerText;
    public TMP_Text pointsText;
    public TMP_Text opponentText;

    private float elapsedTime;
    private int points = 0;
    private int opponentStand = 1;

    void Update()
    {
        elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        if (timerText != null)
            timerText.text = "Zeit: " + minutes.ToString("00") + ":" + seconds.ToString("00");

        if (pointsText != null)
            pointsText.text = "Punkte: " + points;

        if (opponentText != null)
            opponentText.text = "Gegner Stand: " + opponentStand;
    }

    public void AddPoints()
    {
        points += 10;
    }

    public void OpponentNextLevel()
    {
        opponentStand += 1;
    }
}