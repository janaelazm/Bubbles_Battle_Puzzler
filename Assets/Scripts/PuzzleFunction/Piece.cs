using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField]
    private Vector2Int[] shape;
    public Vector2Int[] Shape => shape;

    public bool IsDragging { get; private set; }
    public bool IsPlaced { get; private set; }

    private Vector3 startPosition;
    public int RotationSteps { get; private set; }

    [SerializeField] private PieceCategory[] categories;
    public PieceCategory[] Categories => categories;

    [SerializeField] private int spawnWeight = 1;
    public int SpawnWeight => spawnWeight;

    public bool HasCategory(PieceCategory category)
    {
        return System.Array.IndexOf(categories, category) != -1;
    }


    private void Awake()
    {
        startPosition = transform.position;
    }

    public void StartDragging()
    {
        if (IsPlaced) return;

        IsDragging = true;
    }

    public void Release()
    {
        IsDragging = false;
    }

    public void Place(Vector3 position)
    {
        transform.position = position;
        IsPlaced = true;
        IsDragging = false;
    }

    public void PickUpFromGrid()
    {
        if (!IsPlaced)
            return;

        IsPlaced = false;
        IsDragging = true;
    }

    public void ResetToStart()
    {
        transform.position = startPosition;
        IsDragging = false;
    }

    public void SetRandomRotation()
    {
        RotationSteps = (Random.Range(0, 4)) % 4;
        transform.rotation = Quaternion.Euler(0f, 0f, RotationSteps * 90f);
    }

    public Vector2Int[] GetRotatedShape()
    {
        Vector2Int[] rotatedShape = new Vector2Int[shape.Length];

        for (int i = 0; i < shape.Length; i++)
        {
            Vector2Int cell = shape[i];

            for (int r = 0; r < RotationSteps; r++)
            {
                cell = new Vector2Int(-cell.y, cell.x);
            }

            rotatedShape[i] = cell;
        }

        return rotatedShape;
    }

}