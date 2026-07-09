using UnityEngine;

public class ScoreTester : MonoBehaviour
{
    private void Start()
    {
        ScoreManager.Instance.AddPoints(10);
        ScoreManager.Instance.AddPoints(20);
        ScoreManager.Instance.RemovePoints(5);
    }
}