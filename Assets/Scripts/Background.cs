using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {

    [SerializeField] Camera mainCamera;
    [SerializeField] float borderSize = 0.2f, extraOffset = 0.3f, borderZAxis = -0.6f, topMargin = 0.7f;
    [SerializeField] GameObject borderQuadPrefab;
    [SerializeField] List<BackgroundMaterial> backgroundMaterials;

    [System.Serializable]
    public struct BackgroundMaterial
    {
        public Material bottom;
        public Material top;
        public Material main;
    }


    int currentActiveBackgroundIndex;
    float cameraWidth, cameraHeight;
    GameLines borderLines;

    private void InstantiateBorder(float x, float y, float z, float xScale, float yScale, Material backgroundMaterial, string borderName) {
        GameObject borderInstance = Instantiate(borderQuadPrefab, transform.position, transform.rotation);
        if(borderName != "Bottom Border")
            borderInstance.transform.parent = transform;
        borderInstance.transform.localScale = new Vector3(xScale, yScale, 0);
        borderInstance.transform.position = new Vector3(x, y, z);
        borderInstance.GetComponent<MeshRenderer>().material = backgroundMaterial;
        borderInstance.transform.name = borderName;
    }


    private void FrameBorders()
    {
        var bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        var bottomRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0));
        var topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));
        cameraWidth = bottomRight.x - bottomLeft.x;
        cameraHeight = topRight.y - bottomRight.y;
        GameObject backgroundQuad = transform.GetChild(0).transform.gameObject;

        // left border
        InstantiateBorder(bottomLeft.x + (borderSize / 2.0f),
            backgroundQuad.transform.position.y,
            borderZAxis,
            borderSize,
            cameraHeight + extraOffset,
            backgroundMaterials[currentActiveBackgroundIndex].main,
            "Left Border");


        // right border
        InstantiateBorder(bottomRight.x - (borderSize / 2.0f), 
            backgroundQuad.transform.position.y,
            borderZAxis,
            borderSize,
            cameraHeight + extraOffset,
            backgroundMaterials[currentActiveBackgroundIndex].main,
            "Right Border");

        // bottom border
        InstantiateBorder(backgroundQuad.transform.position.x, 
            (bottomRight.y + (borderSize / 2.0f)),
            borderZAxis,
            cameraWidth, 
            borderSize,
            backgroundMaterials[currentActiveBackgroundIndex].bottom,
            "Bottom Border");

        // top border
        InstantiateBorder(backgroundQuad.transform.position.x, 
            (topRight.y - (topMargin / 2.0f) + extraOffset),
            borderZAxis,
            cameraWidth,
            topMargin,
            backgroundMaterials[currentActiveBackgroundIndex].top,
            "Top Border");


        // draw border lines
        borderLines.DrawBorderLines(bottomLeft.x + borderSize, bottomLeft.y + borderSize, bottomRight.x - borderSize, borderZAxis);

        borderLines.ShowLevelProgress(bottomLeft.x + (borderSize / 2.0f), 
            bottomRight.x - (borderSize / 2.0f), 
            bottomLeft.y + borderSize,
            topRight.y - topMargin + extraOffset,
            gameObject);

    }
    

    private void Start()
    {
        borderLines = FindObjectOfType<GameLines>().GetComponent<GameLines>();
        currentActiveBackgroundIndex = 0;
        FrameBorders();
    }

    void Update() {
        transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0.5f);
    }
}
