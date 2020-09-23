using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    static int vertexCount = 40;
    static float lineWidth = 0.075f, borderOffset = 0.2f;

    public static Color HexToRGB(string code)
    {
        Color testColor;
        ColorUtility.TryParseHtmlString(code, out testColor);
        return testColor;
    }

    public static List<Vector3> DrawCircle(float xPos, float yPos, bool adjustWrtBorders, float radius)
    {
        List<Vector3> points = new List<Vector3>();
        GameObject newCircle = new GameObject("Circle Holder");
        newCircle.transform.position = new Vector3(xPos, yPos, 0f);
        LineRenderer lineRenderer = newCircle.AddComponent<LineRenderer>();

        lineRenderer.widthMultiplier = lineWidth;
        lineRenderer.loop = true;

        if (adjustWrtBorders)
        {
            var bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
            var bottomRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));
            float width = bottomRight.x - bottomLeft.x;
            radius = Mathf.Min((width/2f) - lineWidth - borderOffset, Mathf.Abs(xPos - bottomLeft.x), Mathf.Abs(xPos - bottomRight.x));
            radius = radius - lineWidth - borderOffset;
        }

        float deltaTheta = (2f * Mathf.PI) / vertexCount;
        float theta = 0f;

        lineRenderer.positionCount = vertexCount;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = HexToRGB("#B7A3A3");
        lineRenderer.endColor = HexToRGB("#B7A3A3");
        for (int i=0;i<lineRenderer.positionCount;i++)
        {
            Vector3 pos = new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), 0f);
            lineRenderer.SetPosition(i, pos);
            points.Add(pos);
            theta += deltaTheta;
        }
        return points;
    }


    public static void DrawLine(Vector3 firstPos, Vector3 secondPos, Sprite incomingSprite)
    {
        GameObject first = new GameObject("Line Starter");
        first.transform.position = firstPos;
        first.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        SpriteRenderer renderer = first.AddComponent<SpriteRenderer>();
        renderer.sprite = incomingSprite;
        renderer.sortingOrder = 1;
        GameObject second = new GameObject("Line End point 1");
        second.transform.position = secondPos;
        second.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        renderer = second.AddComponent<SpriteRenderer>();
        renderer.sprite = incomingSprite;
        renderer.sortingOrder = 1;

        GameObject newLine = new GameObject("Line End point 2");
        newLine.transform.position = new Vector3(0,0,0);
        LineRenderer lineRenderer = newLine.AddComponent<LineRenderer>();

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = HexToRGB("#B7A3A3");
        lineRenderer.endColor = HexToRGB("#B7A3A3");

        lineRenderer.widthMultiplier = lineWidth;
        lineRenderer.loop = true;

        lineRenderer.SetPosition(0, firstPos);
        lineRenderer.SetPosition(1, secondPos);

    }

}
