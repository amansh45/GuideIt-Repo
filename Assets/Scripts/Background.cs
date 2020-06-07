using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {

    [SerializeField] Camera mainCamera;
    [SerializeField] float borderSize = 0.2f, extraOffset = 0.5f, borderZAxis = -0.6f, topMargin = 0.5f;
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

    private void InstantiateBorders()
    {
        var bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        var bottomRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0));
        var topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));
        cameraWidth = bottomRight.x - bottomLeft.x;
        cameraHeight = topRight.y - bottomRight.y;
        GameObject backgroundQuad = transform.GetChild(0).transform.gameObject;

        // left border
        GameObject borderInstance = Instantiate(borderQuadPrefab, transform.position, transform.rotation);
        borderInstance.transform.parent = transform;
        borderInstance.transform.localScale = new Vector3(borderSize, cameraHeight + extraOffset, 0);
        borderInstance.transform.position = new Vector3(bottomLeft.x + (borderSize/2.0f), backgroundQuad.transform.position.y, borderZAxis);
        borderInstance.GetComponent<MeshRenderer>().material = backgroundMaterials[currentActiveBackgroundIndex].main;
        borderInstance.transform.name = "Left Border";
        

        // right border
        borderInstance = Instantiate(borderQuadPrefab, transform.position, transform.rotation);
        borderInstance.transform.parent = transform;
        borderInstance.transform.localScale = new Vector3(borderSize, cameraHeight + extraOffset, 0);
        borderInstance.transform.position = new Vector3(bottomRight.x - (borderSize / 2.0f), backgroundQuad.transform.position.y, borderZAxis);
        borderInstance.GetComponent<MeshRenderer>().material = backgroundMaterials[currentActiveBackgroundIndex].main;
        borderInstance.transform.name = "Right Border";


        // bottom border
        borderInstance = Instantiate(borderQuadPrefab, transform.position, transform.rotation);
        borderInstance.transform.localScale = new Vector3(cameraWidth, borderSize, 0);
        borderInstance.transform.position = new Vector3(backgroundQuad.transform.position.x, (bottomRight.y + (borderSize / 2.0f)), borderZAxis);
        borderInstance.GetComponent<MeshRenderer>().material = backgroundMaterials[currentActiveBackgroundIndex].bottom;
        borderInstance.transform.name = "Bottom Border";


        // top border
        borderInstance = Instantiate(borderQuadPrefab, transform.position, transform.rotation);
        borderInstance.transform.parent = transform;
        borderInstance.transform.localScale = new Vector3(cameraWidth, topMargin, 0);
        borderInstance.transform.position = new Vector3(backgroundQuad.transform.position.x, (topRight.y - (borderSize / 2.0f)), borderZAxis);
        borderInstance.GetComponent<MeshRenderer>().material = backgroundMaterials[currentActiveBackgroundIndex].top;
        borderInstance.transform.name = "Top Border";

        // draw border lines
        borderLines.DrawBorderLines(bottomLeft.x + borderSize, bottomLeft.y + borderSize, bottomRight.x - borderSize, topMargin, borderZAxis);
    }
    

    private void Start()
    {
        borderLines = FindObjectOfType<GameLines>().GetComponent<GameLines>();
        currentActiveBackgroundIndex = 0;
        InstantiateBorders();
    }

    void Update() {
        transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0.5f);
    }
}
