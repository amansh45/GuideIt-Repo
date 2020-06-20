using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeTypes
{
    Skin,
    Launcher,
    Bullet,
}

public enum UpgradeParticles
{
    BluePlayerSkin,
    RedPlayerSkin,
    YellowPlayerSkin,
    BlueLauncherSkin,
    RedLauncherSkin,
    RedBulletSkin,
    BlueBulletSkin,
    YellowBulletSkin,
}

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject uiPreviewArea, linePrefab, playerSkin, coinTextGO, moneyTextGO;
    [SerializeField] float thresholdDistanceBetweenCheckpoints = 0.3f;

    int previewUpgradeIndex;

    GameObject previewMovementArea, currentLine, previousFingerPosition;
    Vector3 previewMovementAreaScale;
    Player playerClass;

    PlayerStatistics playerStats;
    

    private void SetPreviewMovementArea(float leftX, float rightX, float bottomY, float topY)
    {
        previewMovementArea = new GameObject();
        previewMovementArea.transform.parent = transform;
        previewMovementArea.transform.position = new Vector3((leftX + rightX) / 2, (bottomY + topY) / 2, 0);
        previewMovementAreaScale = new Vector3((rightX - leftX), (topY - bottomY), 0);
        previewMovementArea.transform.localScale = previewMovementAreaScale;
        previewMovementArea.name = "Preview Movement Area";
    }

    bool ClickedInsidePreviewArea(Vector2 clickedPosition)
    {
        Vector3 playerMovementAreaPos = previewMovementArea.transform.position;
        float areaWidht = previewMovementAreaScale.x;
        float areaHeight = previewMovementAreaScale.y;
        if (clickedPosition.x >= (playerMovementAreaPos.x - (areaWidht / 2)) &&
            clickedPosition.x <= (playerMovementAreaPos.x + (areaWidht / 2)) &&
            clickedPosition.y >= (playerMovementAreaPos.y - (areaHeight / 2)) &&
            clickedPosition.y <= (playerMovementAreaPos.y + (areaHeight / 2)))
            return true;
        else
            return false;

    }

    private void EnablePlayerSkinPreview()
    {
        Vector2 tempFingerPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 currentPlayerPos = transform.position;

        if (Input.GetMouseButtonDown(0))
        {
            if (ClickedInsidePreviewArea(tempFingerPos))
            {
                CreateLine(tempFingerPos);
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (ClickedInsidePreviewArea(tempFingerPos))
            {
                try
                {
                    if (Vector2.Distance(tempFingerPos, previousFingerPosition.transform.position) > thresholdDistanceBetweenCheckpoints)
                        UpdateLine(tempFingerPos);
                }
                catch (System.Exception exception)
                {
                    CreateLine(tempFingerPos);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            playerClass.MovePlayer(PlayerState.Run);
        }
    }

    void CreateLine(Vector2 initialFingerPos)
    {
        //currentLine = objectPooler.SpawnFromPool("PathLine", initialFingerPos, Quaternion.identity);
        currentLine = Instantiate(linePrefab, initialFingerPos, Quaternion.identity) as GameObject;
        previousFingerPosition = currentLine;
        playerClass.SetWayPoints(previousFingerPosition, false);
    }


    void UpdateLine(Vector2 newFingerPos)
    {
        float angle = 0;
        /*
        if (numPoints > 1) {
            Vector2 secondVector = newFingerPos - fingerPositions[numPoints - 1];
            Vector2 firstVector = fingerPositions[numPoints - 1] - fingerPositions[numPoints - 2];
            angle = Vector2.Angle(firstVector, secondVector);
        }
        */
        var rotation = Quaternion.Euler(0, 0, angle);
        //GameObject lineInstance = objectPooler.SpawnFromPool("PathLine", newFingerPos, rotation);
        GameObject lineInstance = Instantiate(linePrefab, newFingerPos, rotation) as GameObject;
        previousFingerPosition = lineInstance;
        playerClass.SetWayPoints(previousFingerPosition, true);
    }

    public void UpgradeClicked(int index)
    {
        previewUpgradeIndex = index;
    }

    void Start()
    {
        Vector3[] corners = new Vector3[4];
        uiPreviewArea.GetComponent<RectTransform>().GetWorldCorners(corners);
        for (int i = 0; i < 4; i++)
        {
            corners[i] = mainCamera.ScreenToWorldPoint(corners[i]);
            Debug.Log(corners[i]);
        }
        playerClass = playerSkin.GetComponent<Player>();

        playerStats = FindObjectOfType<PlayerStatistics>();

        previewUpgradeIndex = playerStats.activeUpgradeIndex;

        SetPreviewMovementArea(corners[0].x, corners[3].x, corners[0].y, corners[1].y);
    }

    void Update()
    {
        PlayerStatistics.Upgrade activeUpgrade = playerStats.upgradesList[previewUpgradeIndex];
        if (activeUpgrade.ApplicableOn == ObjectsDescription.Player)
        {
            EnablePlayerSkinPreview();
        }
    }
}
