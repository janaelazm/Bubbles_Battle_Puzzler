using System;
using UnityEngine;

public class Tile : MonoBehaviour
{
   [SerializeField] private Color baseColor, offsetColor;
   [SerializeField] private SpriteRenderer spriteRrenderer;
   [SerializeField] private GameObject highlight;

   public void Init(bool isOffset)
    {
        spriteRrenderer.color = isOffset? offsetColor: baseColor;
    }

    void OnMouseEnter()
    {
        highlight.SetActive(true);
    }

    void OnMouseExit()
    {
        highlight.SetActive(false);
    }
}
