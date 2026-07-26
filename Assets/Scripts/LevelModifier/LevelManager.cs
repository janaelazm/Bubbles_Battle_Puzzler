using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private PieceSpawner pieceSpawner;

    private LevelModifier activeModifier;

    private void Start()
    {
        activeModifier =
            LevelTransferData.SelectedModifier;

        if (activeModifier == null)
        {
            Debug.Log("Level Modifier: None");
        }
        else
        {
            Debug.Log(
                $"Level Modifier: " +
                $"{activeModifier.displayName} | " +
                $"Bonus: {activeModifier.scoreBonus}"
            );
        }

        if (pieceSpawner == null)
        {
            Debug.LogError(
                "PieceSpawner is not assigned."
            );

            return;
        }

        pieceSpawner.SetModifier(activeModifier);
    }

    public LevelModifier GetActiveModifier()
    {
        return activeModifier;
    }
}