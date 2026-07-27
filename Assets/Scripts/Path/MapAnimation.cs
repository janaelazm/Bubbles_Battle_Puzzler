using UnityEngine;
using UnityEngine.UI;

public class MapAnimation : MonoBehaviour
{
    [SerializeField] private Image mapImage;
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float frameDuration = 0.2f;

    private int currentFrame;
    private float timer;

    private void Awake()
    {
        if (mapImage == null)
            mapImage = GetComponent<Image>();

        if (frames.Length > 0)
            mapImage.sprite = frames[0];
    }

    private void Update()
    {
        if (mapImage == null || frames == null || frames.Length == 0)
            return;

        timer += Time.deltaTime;

        if (timer < frameDuration)
            return;

        timer -= frameDuration;
        currentFrame = (currentFrame + 1) % frames.Length;
        mapImage.sprite = frames[currentFrame];
    }
}