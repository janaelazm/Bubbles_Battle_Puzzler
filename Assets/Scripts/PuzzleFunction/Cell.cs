using UnityEngine;

public class Cell : MonoBehaviour
{
    [Header("Colors")]
    public Color defaultColor = Color.white;
    public Color hoverColor = Color.green;
    public Color occupiedColor = Color.gray;

    public Vector2Int GridPosition { get; private set; }
    public bool IsOccupied { get; private set; }

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetDefaultColor();
    }

    public void Init(Vector2Int gridPosition)
    {
        GridPosition = gridPosition;
    }

    public void SetHover(bool hover)
    {
        if (IsOccupied) return;

        spriteRenderer.color = hover ? hoverColor : defaultColor;
    }

    public void SetOccupied(bool occupied)
    {
        IsOccupied = occupied;

        spriteRenderer.color = occupied ? occupiedColor : defaultColor;
    }

    private void SetDefaultColor()
    {
        spriteRenderer.color = defaultColor;
    }


}