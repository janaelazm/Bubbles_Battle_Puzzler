using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FixedAspectCamera : MonoBehaviour
{
    [SerializeField] private float targetWidth = 1440f;
    [SerializeField] private float targetHeight = 2048f;

    private Camera targetCamera;

    private void Awake()
    {
        targetCamera = GetComponent<Camera>();
        ApplyAspectRatio();
    }

    private void OnEnable()
    {
        ApplyAspectRatio();
    }

    private void Update()
    {
        ApplyAspectRatio();
    }

    private void ApplyAspectRatio()
    {
        if (Screen.width <= 0 || Screen.height <= 0)
            return;

        float targetAspect = targetWidth / targetHeight;
        float screenAspect = (float)Screen.width / Screen.height;

        if (screenAspect > targetAspect)
        {
            // Display ist breiter als das Zielformat:
            // Balken links und rechts.
            float viewportWidth = targetAspect / screenAspect;

            targetCamera.rect = new Rect(
                (1f - viewportWidth) * 0.5f,
                0f,
                viewportWidth,
                1f
            );
        }
        else
        {
            // Display ist höher/schmaler als das Zielformat:
            // Balken oben und unten.
            float viewportHeight = screenAspect / targetAspect;

            targetCamera.rect = new Rect(
                0f,
                (1f - viewportHeight) * 0.5f,
                1f,
                viewportHeight
            );
        }
    }
}