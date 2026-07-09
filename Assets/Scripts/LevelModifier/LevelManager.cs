using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelModifier[] possibleModifiers;
    [SerializeField] private PieceSpawner pieceSpawner;

    private LevelModifier activeModifier;

    private void Start()
    {
        SelectRandomModifier();

        pieceSpawner.SetModifier(activeModifier);
    }

    private void SelectRandomModifier()
    {
        int randomIndex = Random.Range(0, possibleModifiers.Length + 1);

        if (randomIndex == possibleModifiers.Length)
        {
            activeModifier = null;
            Debug.Log("Level Modifier: None");
            return;
        }

        activeModifier = possibleModifiers[randomIndex];

        Debug.Log($"Level Modifier: {activeModifier.displayName}");
    }

    public LevelModifier GetActiveModifier()
    {
        return activeModifier;
    }
}