using Item;
using UnityEngine;

/// <summary>
/// Draw colliders in the scene view using GL.
/// </summary>
public class GlDrawer : MonoBehaviour
{
    public Material mat;
    public ItemPlayBehaviour itemPlayBehaviour;
    public int circleSegments = 32;
    private Matrix4x4 worldToGlViewport;

    private void Awake()
    {
        var cam = Camera.main;
        var worldToView = cam.worldToCameraMatrix;
        var projection = cam.projectionMatrix;

        // transform xy from [-1, 1](projection coordinate) to [0, 1](gl coordinate)
        var matrix = new Matrix4x4();
        matrix.SetRow(0, new Vector4(0.5f, 0, 0, 0.5f));
        matrix.SetRow(1, new Vector4(0, 0.5f, 0, 0.5f));
        matrix.SetRow(3, new Vector4(0, 0, 0, 1));

        this.worldToGlViewport = matrix * projection * worldToView;
    }

    private void OnPostRender()
    {
        Debug.Log("OnPostRender");
        if (!mat)
        {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }

        mat.SetPass(0);

        // DrawItem();
    }

    private void DrawItem()
    {
        var colliders = itemPlayBehaviour.GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
        {
            DrawCollider(col);
        }
    }

    private void DrawCollider(Collider2D col)
    {
        GL.PushMatrix();
        GL.LoadOrtho();
        var matrix = this.worldToGlViewport * col.transform.localToWorldMatrix;
        GL.MultMatrix(matrix);

        switch (col)
        {
            case BoxCollider2D boxCollider:
                this.DrawBoxCollider(boxCollider);
                break;
            case CircleCollider2D circleCollider:
                this.DrawCircleCollider(circleCollider);
                break;
            case CapsuleCollider2D capsuleCollider:
                this.DrawCapsuleCollider(capsuleCollider);
                break;
        }

        GL.PopMatrix();
    }

    private void DrawCapsuleCollider(CapsuleCollider2D capsuleCollider)
    {
        var center = capsuleCollider.offset;
        var size = capsuleCollider.size;

        switch (capsuleCollider.direction)
        {
            case CapsuleDirection2D.Vertical:
            {
                var radius = size.x * 0.5f;
                var height = size.y - 2 * radius;
                // center rectangle
                InnerDrawQuad(new Vector2(center.x - radius, center.y - height * 0.5f),
                    new Vector2(center.x + radius, center.y + height * 0.5f));
                // top half circle
                InnerDrawSector(new Vector2(center.x, center.y + height * 0.5f), radius, 0, 180);
                // bottom half circle
                InnerDrawSector(new Vector2(center.x, center.y - height * 0.5f), radius, 180, 360);
                break;
            }
            case CapsuleDirection2D.Horizontal:
            {
                var radius = size.y * 0.5f;
                var width = size.x - 2 * radius;
                // center rectangle
                InnerDrawQuad(new Vector2(center.x - width * 0.5f, center.y - radius),
                    new Vector2(center.x + width * 0.5f, center.y + radius));
                // left half circle
                InnerDrawSector(new Vector2(center.x - width * 0.5f, center.y), radius, 90, 270);
                // right half circle
                InnerDrawSector(new Vector2(center.x + width * 0.5f, center.y), radius, 270, 450);
                break;
            }
        }
    }

    private void DrawCircleCollider(CircleCollider2D circleCollider)
    {
        var center = circleCollider.offset;
        var radius = circleCollider.radius;

        InnerDrawSector(center, radius, 0, 360);
    }

    private void DrawBoxCollider(BoxCollider2D boxCollider)
    {
        GL.Begin(GL.QUADS);

        var min = boxCollider.offset - 0.5f * boxCollider.size;
        var max = boxCollider.offset + 0.5f * boxCollider.size;
        var radius = boxCollider.edgeRadius;

        // center quad
        InnerDrawQuad(min, max);

        // top quad
        InnerDrawQuad(new Vector2(min.x, max.y), new Vector2(max.x, max.y + radius));

        // bottom quad
        InnerDrawQuad(new Vector2(min.x, min.y - radius), new Vector2(max.x, min.y));

        // left quad
        InnerDrawQuad(new Vector2(min.x - radius, min.y), new Vector2(min.x, max.y));

        // right quad
        InnerDrawQuad(new Vector2(max.x, min.y), new Vector2(max.x + radius, max.y));

        // top left quarter
        InnerDrawSector(new Vector2(min.x, max.y), radius, 90, 180);

        // top right quarter
        InnerDrawSector(new Vector2(max.x, max.y), radius, 0, 90);

        // bottom left quarter
        InnerDrawSector(new Vector2(min.x, min.y), radius, 180, 270);

        // bottom right quarter
        InnerDrawSector(new Vector2(max.x, min.y), radius, 270, 360);
    }


    private void InnerDrawQuad(Vector2 min, Vector2 max)
    {
        GL.Begin(GL.QUADS);

        // center quad
        GL.Vertex3(min.x, min.y, 0);
        GL.Vertex3(min.x, max.y, 0);
        GL.Vertex3(max.x, max.y, 0);
        GL.Vertex3(max.x, min.y, 0);

        GL.End();
    }

    private void InnerDrawSector(Vector2 center, float radius, float startAngle, float endAngle)
    {
        var deltaAngle = endAngle - startAngle;
        if (deltaAngle < 0f || deltaAngle > 360f)
        {
            Debug.LogError("Invalid angle range for InnerDrawSector");
            return;
        }

        // startAngle *= Mathf.Deg2Rad;
        endAngle *= Mathf.Deg2Rad;
        var segments = circleSegments * (int)Mathf.Ceil(deltaAngle / 360f);
        var angle = (deltaAngle / segments) * Mathf.Deg2Rad;

        GL.Begin(GL.TRIANGLES);

        for (var i = 0; i <= segments; i++)
        {
            GL.Vertex3(center.x, center.y, 0);

            var x1 = center.x + radius * Mathf.Cos(endAngle - angle * i);
            var y1 = center.y + radius * Mathf.Sin(endAngle - angle * i);
            GL.Vertex3(x1, y1, 0);

            var x2 = center.x + radius * Mathf.Cos(endAngle - angle * (i + 1));
            var y2 = center.y + radius * Mathf.Sin(endAngle - angle * (i + 1));
            GL.Vertex3(x2, y2, 0);
        }

        GL.End();
    }
}