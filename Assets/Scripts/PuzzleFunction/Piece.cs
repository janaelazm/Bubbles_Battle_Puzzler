using UnityEngine;

public class Piece : MonoBehaviour
{
    public bool IsDragging { get; private set; }
    public bool IsPlaced { get; private set; }

    private Vector3 startPosition;

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

    public void ResetToStart()
    {
        transform.position = startPosition;
        IsDragging = false;
    }
}