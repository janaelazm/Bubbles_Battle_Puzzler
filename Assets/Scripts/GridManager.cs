using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width, height;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform cam;

    void Start()
    {
        GenerateTile();
    }
    void GenerateTile()
    {
        for(int w=0 ; w < width ; w++ )
        {
            for(int h=0 ; h < height ; h++ )
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(w, h), Quaternion.identity);
                spawnedTile.name = $"Tile {w}{h}";

                var isOffset = ((w % 2 == 0 && h % 2 != 0) || (w % 2 != 0 && h % 2 == 0));
                spawnedTile.Init(isOffset);
            }
        }

        cam.transform.position = new Vector3((float)width /2 -0.5f, (float) height /2 -0.5f, -10);
    }
}
