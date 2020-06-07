using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLines : MonoBehaviour {

    [SerializeField] float screenBorderOffset = 0.2f;
    [SerializeField] Camera mainCamera;
    [SerializeField] float borderWidth = 0.025f;
    [SerializeField] Color color = Color.white;
    List<GameObject> gameObjectsForLineRenderer = new List<GameObject>();
    List<LineRenderer> lineRenderersList = new List<LineRenderer>();
    
    GameObject finishParticle, playSpace;

    private void RenderLine(Vector3 fpoint, Vector3 spoint, int index)
    {
        LineRenderer borderLineRenderer = lineRenderersList[index];
        borderLineRenderer.material = new Material(Shader.Find("Mobile/Particles/Alpha Blended"));
        borderLineRenderer.startColor = color;
        borderLineRenderer.endColor = color;
        borderLineRenderer.startWidth = borderWidth;
        borderLineRenderer.endWidth = borderWidth;
        borderLineRenderer.SetPosition(0, fpoint);
        borderLineRenderer.SetPosition(1, spoint);
        borderLineRenderer.useWorldSpace = false;
    }
    

    public void DrawBorderLines(float bottomLeftX, float bottomLeftY, float bottomRightX, float topMargin, float borderZaxis)
    {

        var bottomLeft = new Vector3(bottomLeftX, bottomLeftY, borderZaxis);
        var bottomRight = new Vector3(bottomRightX, bottomLeftY, borderZaxis);
        Renderer cornerPoints = playSpace.GetComponent<Renderer>();
        float topY = cornerPoints.bounds.max.y;
        var topLeft = new Vector3(bottomLeftX, topY - topMargin, borderZaxis);
        var topRight = new Vector3(bottomRightX, topY - topMargin, borderZaxis);
        
        RenderLine(topRight, bottomRight, 0);
        RenderLine(bottomLeft, bottomRight, 1);
        RenderLine(bottomLeft, topLeft, 2);
        RenderLine(topLeft, topRight, 3);
    }

    private void Start()
    {
        finishParticle = transform.GetChild(0).gameObject;
        finishParticle.SetActive(false);
        int currentIndex = gameObject.transform.GetSiblingIndex();
        playSpace = transform.parent.GetChild(currentIndex + 1).gameObject;

        for (int i = 0; i < 4; i++)
        {
            GameObject customGameObject = new GameObject();
            customGameObject.transform.parent = transform.parent;
            customGameObject.name = "Border Line " + i;
            gameObjectsForLineRenderer.Add(customGameObject);
            lineRenderersList.Add(gameObjectsForLineRenderer[i].AddComponent<LineRenderer>());
        }
    }

    

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == ObjectsDescription.Player.ToString())
            finishParticle.SetActive(true);
    }
}
