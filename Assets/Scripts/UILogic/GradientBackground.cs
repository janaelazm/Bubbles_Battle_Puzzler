using UnityEngine;
using UnityEngine.UI;

public class GradientBackground : BaseMeshEffect
{
    public Color topColor = new Color(0.0f, 0.8f, 0.6f);
    public Color bottomColor = new Color(0.0f, 0.2f, 0.8f);

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive()) return;

        UIVertex vertex = new UIVertex();

        for (int i = 0; i < vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);

            float t = Mathf.InverseLerp(-500f, 500f, vertex.position.y);
            vertex.color = Color.Lerp(bottomColor, topColor, t);

            vh.SetUIVertex(vertex, i);
        }
    }
}