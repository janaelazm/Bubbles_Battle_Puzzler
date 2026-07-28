using System;
using UnityEngine;

public class FloatingPiece : MonoBehaviour
{
    private Piece piece;

    private float moveSpeed;
    private float despawnX;

    private bool isInRiver;

    public event Action<FloatingPiece> LeftRiver;
    public event Action<FloatingPiece> TakenFromRiver;

    private void Awake()
    {
        piece = GetComponent<Piece>();
    }

    public void Init(
        float speed,
        float leftDespawnPosition
    )
    {
        moveSpeed = speed;
        despawnX = leftDespawnPosition;
        isInRiver = true;
    }

    private void Update()
    {
        if (!isInRiver)
            return;

        if (piece == null ||
            piece.IsDragging ||
            piece.IsPlaced)
        {
            return;
        }

        transform.position +=
            Vector3.left *
            moveSpeed *
            Time.deltaTime;

        if (transform.position.x <= despawnX)
        {
            LeaveRiver();
        }
    }

    public void RemoveFromRiver()
    {
        if (!isInRiver)
            return;

        isInRiver = false;
        TakenFromRiver?.Invoke(this);
    }

    private void LeaveRiver()
    {
        if (!isInRiver)
            return;

        isInRiver = false;
        LeftRiver?.Invoke(this);

        Destroy(gameObject);
    }
}