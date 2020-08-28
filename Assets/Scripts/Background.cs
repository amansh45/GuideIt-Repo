using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {

    [SerializeField] Camera mainCamera;
    [SerializeField] float borderSize = 0.2f, extraOffset = 0.3f, borderZAxis = -0.6f, topMargin = 0.7f;
    [SerializeField] GameObject borderQuadPrefab;
    [SerializeField] List<BackgroundMaterial> backgroundMaterials;
    [SerializeField] GameObject playerPrefab;

    PlayerActions playerActionsClass;
    ProceduralGeneration proceduralGeneration;

    bool firstTime = true;

    [System.Serializable]
    public struct BackgroundMaterial
    {
        public string PlayerSkinColor;
        public string PlayerSkinCategory;
        public Material Bottom;
        public Material Top;
        public Material Main;

        public BackgroundMaterial(string playerSkinColor, string playerSkinCategory, Material bottom, Material top, Material main)
        {
            PlayerSkinCategory = playerSkinCategory;
            PlayerSkinColor = playerSkinColor;
            Bottom = bottom;
            Top = top;
            Main = main;
        }
    }


    int currentActiveBackgroundIndex;
    float cameraWidth, cameraHeight;
    GameLines borderLines;
    PlayerStatistics playerStats;

    private int RetrieveBackgroundIndex(SkinColors playerSkinColor, SkinCategory playerSkinCategory)
    {
        int numBackgroundMaterials = backgroundMaterials.Count, i = 0;
        while(i < numBackgroundMaterials)
        {
            if (backgroundMaterials[i].PlayerSkinCategory == playerSkinCategory.ToString() && backgroundMaterials[i].PlayerSkinColor == playerSkinColor.ToString())
                break;
            i += 1;
        }
        return (i < numBackgroundMaterials) ? i : numBackgroundMaterials - 1;
    }

    private void SetActiveBackgroundIndex()
    {
        int numUpgrades = playerStats.upgradesList.Count;

        for (int i = 0; i < numUpgrades; i++)
        {
            PlayerStatistics.Upgrade currUpgrade = playerStats.upgradesList[i];
            if (currUpgrade.IsActive)
            {
                if (currUpgrade.ApplicableOn == ObjectsDescription.Player)
                {
                    currentActiveBackgroundIndex = RetrieveBackgroundIndex(currUpgrade.ParticlesColor, currUpgrade.UpgradeCategory);
                    break;
                }
            }
        }

        GameObject backgroundInstance = gameObject.transform.GetChild(0).gameObject;
        backgroundInstance.GetComponent<MeshRenderer>().material = backgroundMaterials[currentActiveBackgroundIndex].Main;

    }

    private void InstantiateBorder(float x, float y, float z, float xScale, float yScale, Material backgroundMaterial, string borderName)
    {
        GameObject borderInstance = Instantiate(borderQuadPrefab, transform.position, transform.rotation);
        if (borderName != "Bottom Border")
            borderInstance.transform.parent = transform;
        borderInstance.transform.localScale = new Vector3(xScale, yScale, 0);
        borderInstance.transform.position = new Vector3(x, y, z);
        borderInstance.GetComponent<MeshRenderer>().material = backgroundMaterial;
        borderInstance.transform.name = borderName;
    }

    private void FrameBordersPlayerAreaAndLevelProgress()
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
            backgroundMaterials[currentActiveBackgroundIndex].Main,
            "Left Border");


        // right border
        InstantiateBorder(bottomRight.x - (borderSize / 2.0f), 
            backgroundQuad.transform.position.y,
            borderZAxis,
            borderSize,
            cameraHeight + extraOffset,
            backgroundMaterials[currentActiveBackgroundIndex].Main,
            "Right Border");

        // bottom border
        InstantiateBorder(backgroundQuad.transform.position.x, 
            (bottomRight.y + (borderSize / 2.0f)),
            borderZAxis,
            cameraWidth, 
            borderSize,
            backgroundMaterials[currentActiveBackgroundIndex].Bottom,
            "Bottom Border");

        // top border
        InstantiateBorder(backgroundQuad.transform.position.x, 
            (topRight.y - (topMargin/2.0f) + extraOffset),
            borderZAxis,
            cameraWidth,
            topMargin,
            backgroundMaterials[currentActiveBackgroundIndex].Top,
            "Top Border");


        // draw border lines
        borderLines.DrawBorderLines(bottomLeft.x + borderSize, bottomLeft.y + borderSize, bottomRight.x - borderSize,
            topMargin - extraOffset,
            borderZAxis);

        borderLines.ShowLevelProgress(bottomLeft.x + (borderSize / 2.0f), 
            bottomRight.x - (borderSize / 2.0f), 
            bottomLeft.y + borderSize,
            topRight.y - topMargin + extraOffset,
            gameObject);

        playerActionsClass.SetPlayerMovementArea(bottomLeft.x + (borderSize / 2.0f),
            bottomRight.x - (borderSize / 2.0f),
            bottomLeft.y + borderSize,
            topRight.y - topMargin + extraOffset,
            gameObject);

    }

    
    private void Start()
    {
        borderLines = FindObjectOfType<GameLines>().GetComponent<GameLines>();
        playerActionsClass = playerPrefab.GetComponent<PlayerActions>();
        currentActiveBackgroundIndex = 0;
        proceduralGeneration = FindObjectOfType<ProceduralGeneration>();
        playerStats = FindObjectOfType<PlayerStatistics>();
        if(proceduralGeneration == null)
        {
            SetActiveBackgroundIndex();
            FrameBordersPlayerAreaAndLevelProgress();
        }
    }

    void Update() {
        if(proceduralGeneration != null && proceduralGeneration.hasLevelGenerationCompleted && firstTime)
        {
            SetActiveBackgroundIndex();
            FrameBordersPlayerAreaAndLevelProgress();
            firstTime = false;
        }

        transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0.5f);
    }
}
