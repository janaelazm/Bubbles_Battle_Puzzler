using UnityEngine;
using UnityEngine.UI;

public class BackgroundAnimation : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float frameDuration = 0.2f;

    private int currentFrame;
    private float timer;

    private void Awake()
    {
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();

        if (frames.Length > 0)
            backgroundImage.sprite = frames[0];
    }

    private void Update()
    {
        if (backgroundImage == null || frames == null || frames.Length == 0)
            return;

        timer += Time.deltaTime;

        if (timer < frameDuration)
            return;

        timer -= frameDuration;
        currentFrame = (currentFrame + 1) % frames.Length;
        backgroundImage.sprite = frames[currentFrame];
    }
}
