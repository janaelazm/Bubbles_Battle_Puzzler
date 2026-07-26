using UnityEngine;

public class DeleteZone : MonoBehaviour
{
    public RectTransform RectTransform { get; private set; }

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
    }
}
