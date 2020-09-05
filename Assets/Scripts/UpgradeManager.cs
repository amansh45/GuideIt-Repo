using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum SkinCategory
{
    PlayerBasic,
    PlayerModerate,
/*
    PlayerAdvance,
    LauncherBasic,
    LauncherModerate,
    LauncherAdvance,
    BulletBasic,
    BulletModerate,
    BulletAdvance,
*/
}

public enum SkinColors
{
    Red,
    Blue,
    Purple,
    Yellow,
}


public class UpgradeManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject uiPreviewArea, linePrefab, playerSkin, playerLauncher, coinTextGO, moneyTextGO, lockButtonsGO, unlockButtonsGO, unlockTextGO;
    [SerializeField] GameObject colorSelector, numCoinsTextGO;
    [SerializeField] float thresholdDistanceBetweenCheckpoints = 0.3f, thresholdDoubleClickTime = 0.3f, thresholdBetweenDClickAndPlayer = 0.5f;
    [SerializeField] GameObject mainCanvas, secondaryCanvas, secondaryCanvasColors;

    // Note: The sequence for declaring these should be same as that of skinCategory and also the size of the list should be the size of the skinCategory...
    [SerializeField] List<GameObject> upgradesGO;

    int selectedUpgradeIndexForPreview;
    TextMeshProUGUI coinCostTMPro, moneyCostTMPro, unlockButtonTMPro, numCoinsTMPro;
    GameObject previewMovementArea, currentLine, previousFingerPosition, playerLauncherInstance, colorSelectorArea;
    Vector3 previewMovementAreaScale, colorSelectorAreaScale;
    Player playerClass;
    float previousClickTime = int.MinValue;
    PlayerStatistics playerStats;
    UpgradeScroller upgradeScrollerClass;
    Dictionary<string, GameObject> skinsAndMaterials = new Dictionary<string, GameObject>();
    TaskHandler taskHandlerClass;

    bool isShooting = false, previewHasFocus = true;

    private void SetPreviewMovementArea(float leftX, float rightX, float bottomY, float topY)
    {
        previewMovementArea = new GameObject();
        previewMovementArea.transform.parent = transform;
        previewMovementArea.transform.position = new Vector3((leftX + rightX) / 2, (bottomY + topY) / 2, 0);
        previewMovementAreaScale = new Vector3((rightX - leftX), (topY - bottomY), 0);
        previewMovementArea.transform.localScale = previewMovementAreaScale;
        previewMovementArea.name = "Preview Movement Area";
    }

    private void SetColorSelectorArea(float leftX, float rightX, float bottomY, float topY)
    {
        colorSelectorArea = new GameObject();
        colorSelectorArea.transform.parent = transform;
        colorSelectorArea.transform.position = new Vector3((leftX + rightX) / 2, (bottomY + topY) / 2, 0);
        colorSelectorAreaScale = new Vector3((rightX - leftX), (topY - bottomY), 0);
        colorSelectorArea.transform.localScale = previewMovementAreaScale;
        colorSelectorArea.name = "Color Selector Area";
    }

    bool ClickedInsideArea(Vector2 clickedPosition, Vector3 areaScale, GameObject targetArea)
    {
        Vector3 targetAreaPos = targetArea.transform.position;
        float areaWidht = areaScale.x;
        float areaHeight = areaScale.y;
        if (clickedPosition.x >= (targetAreaPos.x - (areaWidht / 2)) &&
            clickedPosition.x <= (targetAreaPos.x + (areaWidht / 2)) &&
            clickedPosition.y >= (targetAreaPos.y - (areaHeight / 2)) &&
            clickedPosition.y <= (targetAreaPos.y + (areaHeight / 2)))
            return true;
        else
            return false;

    }

    private void EnablePlayerSkinPreview()
    {
        Vector2 tempFingerPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 currentPlayerPos = transform.position;

        bool isClickedInsidePreview = ClickedInsideArea(tempFingerPos, previewMovementAreaScale, previewMovementArea);
        bool isClickedOnColorSelector = ClickedInsideArea(tempFingerPos, colorSelectorAreaScale, colorSelectorArea);

        if (Input.GetMouseButtonDown(0))
        {
            if (isClickedInsidePreview && !isClickedOnColorSelector)
            {
                CreateLine(tempFingerPos);
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (isClickedInsidePreview && !isClickedOnColorSelector)
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
            playerClass.playerState = PlayerState.Run;
        }
    }

    private bool DoubleClickedOnPlayer(Vector2 fingerPosition)
    {
        float currentTime = Time.time;
        Vector2 playerPosition = playerSkin.transform.position;
        bool hasClickedNearPlayer = Vector2.Distance(playerPosition, fingerPosition) <= thresholdBetweenDClickAndPlayer;
        if (currentTime - previousClickTime <= thresholdDoubleClickTime && hasClickedNearPlayer)
        {
            previousClickTime = currentTime;
            return true;
        }
        else
        {
            previousClickTime = currentTime;
            return false;
        }

    }

    float FindAngleBetweenVectors(Vector2 fVec, Vector2 sVec)
    {
        fVec = fVec - sVec;
        float angle = Mathf.Atan2(fVec.y, fVec.x) * Mathf.Rad2Deg;
        return angle;
    }

    private void ProcessShoot(Vector2 currentFingerPos, Vector2 currentPlayerPosition)
    {
        float launcherAngle = FindAngleBetweenVectors(currentPlayerPosition, currentFingerPos);
        var launcherDirection = Quaternion.Euler(new Vector3(0, 0, launcherAngle));
        playerLauncherInstance.transform.rotation = launcherDirection;
    }

    private void FinalizeShoot(Vector2 currentFingerPos, Vector2 currentPlayerPosition)
    {
        float bulletFiringAngle = FindAngleBetweenVectors(currentPlayerPosition, currentFingerPos);
        var bulletFiringDirection = Quaternion.Euler(new Vector3(0, 0, bulletFiringAngle));
        playerLauncherInstance.GetComponent<PlayerLauncher>().ShootAndSelfDestruct(bulletFiringDirection);
        playerClass.SetScale(1f);
    }

    void CreateLine(Vector2 initialFingerPos)
    {
        currentLine = Instantiate(linePrefab, initialFingerPos, Quaternion.identity) as GameObject;
        currentLine.GetComponent<SpriteRenderer>().color = playerStats.colorsData[playerStats.upgradesList[selectedUpgradeIndexForPreview].ParticlesColor.ToString()].ThirdColor;
        previousFingerPosition = currentLine;
        playerClass.SetWayPoints(previousFingerPosition, false);
    }


    void UpdateLine(Vector2 newFingerPos)
    {
        float angle = 0;
        var rotation = Quaternion.Euler(0, 0, angle);
        GameObject lineInstance = Instantiate(linePrefab, newFingerPos, rotation) as GameObject;
        lineInstance.GetComponent<SpriteRenderer>().color = playerStats.colorsData[playerStats.upgradesList[selectedUpgradeIndexForPreview].ParticlesColor.ToString()].ThirdColor;
        previousFingerPosition = lineInstance;
        playerClass.SetWayPoints(previousFingerPosition, true);
    }

    private void EnableLauncherSkinPreview()
    {
        Vector2 tempFingerPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 currentPlayerPos = playerSkin.transform.position;

        bool isClickedInsidePreview = ClickedInsideArea(tempFingerPos, previewMovementAreaScale, previewMovementArea);
        bool isClickedOnColorSelector = ClickedInsideArea(tempFingerPos, colorSelectorAreaScale, colorSelectorArea);

        if (Input.GetMouseButtonDown(0))
        {
            if (isClickedInsidePreview && !isClickedOnColorSelector)
            {
                if (DoubleClickedOnPlayer(tempFingerPos))
                {
                    isShooting = true;
                    playerLauncherInstance = Instantiate(playerLauncher, playerSkin.transform.position, playerSkin.transform.rotation);
                    playerClass.SetScale(0f);
                }
            }

        }
        else if (Input.GetMouseButton(0))
        {
            if (isShooting)
                ProcessShoot(tempFingerPos, currentPlayerPos);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (isShooting)
            {
                isShooting = false;
                FinalizeShoot(tempFingerPos, currentPlayerPos);
            }
        }
    }





    public void ShowColorsMenu()
    {
        PlayerStatistics.Upgrade currentUpgrade = playerStats.upgradesList[selectedUpgradeIndexForPreview];
        mainCanvas.SetActive(false);
        playerSkin.SetActive(false);
        secondaryCanvas.SetActive(true);
        previewHasFocus = false;
        foreach (Transform child in secondaryCanvasColors.transform)
        {
            if (child.gameObject.name == currentUpgrade.ParticlesColor.ToString())
                child.GetChild(1).gameObject.SetActive(true);
            else
                child.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void SelectColor(string colorStr)
    {
        PlayerStatistics.Upgrade currentUpgrade = playerStats.upgradesList[selectedUpgradeIndexForPreview];

        if (SkinColors.Yellow.ToString() == colorStr)
            currentUpgrade.ParticlesColor = SkinColors.Yellow;
        else if (SkinColors.Blue.ToString() == colorStr)
            currentUpgrade.ParticlesColor = SkinColors.Blue;
        else if (SkinColors.Red.ToString() == colorStr)
            currentUpgrade.ParticlesColor = SkinColors.Red;
        else if (SkinColors.Purple.ToString() == colorStr)
            currentUpgrade.ParticlesColor = SkinColors.Purple;

        PlayerStatistics.CustomColor mcolor = playerStats.colorsData[currentUpgrade.ParticlesColor.ToString()];

        colorSelector.GetComponent<Image>().color = mcolor.ThirdColor;

        playerStats.upgradesList[selectedUpgradeIndexForPreview] = currentUpgrade;

        UpdateUiData(currentUpgrade);

        if (currentUpgrade.ApplicableOn == ObjectsDescription.Player)
            playerStats.UpdateColorOfSkin(currentUpgrade, playerSkin);
        else if (currentUpgrade.ApplicableOn == ObjectsDescription.PlayerLauncher)
            Debug.Log("Under Construction...");
    }

    public void ShowMainCanvas()
    {
        secondaryCanvas.SetActive(false);
        playerSkin.SetActive(true);    
        mainCanvas.SetActive(true);
        previewHasFocus = true;
    }

    public void UpgradeClicked(int index)
    {
        selectedUpgradeIndexForPreview = index;
        PlayerStatistics.Upgrade currentUpgrade = playerStats.upgradesList[selectedUpgradeIndexForPreview];
        upgradeScrollerClass.UpgradeChosen(index);
        UpdateUiData(currentUpgrade);
        UpdateMaterial(currentUpgrade);
        if (currentUpgrade.ApplicableOn == ObjectsDescription.Player)
            playerStats.UpdateColorOfSkin(currentUpgrade, playerSkin);
        else if (currentUpgrade.ApplicableOn == ObjectsDescription.PlayerLauncher)
            Debug.Log("Under Construction...");
    }

    private void UpdateMaterial(PlayerStatistics.Upgrade currentUpgrade)
    {
        GameObject targetParticle;
        if (currentUpgrade.ApplicableOn == ObjectsDescription.Player)
            targetParticle = playerSkin.transform.GetChild(0).gameObject;
        else if(currentUpgrade.ApplicableOn == ObjectsDescription.PlayerLauncher)
            targetParticle = playerSkin.transform.GetChild(0).gameObject;
        else
            targetParticle = playerSkin.transform.GetChild(0).gameObject;

        GameObject newParticle = skinsAndMaterials[currentUpgrade.UpgradeCategory.ToString()];
        newParticle.SetActive(true);
        newParticle.transform.parent = targetParticle.transform.parent;
        newParticle.transform.position = targetParticle.transform.position;

        targetParticle.transform.parent = transform;
        targetParticle.SetActive(false);
    }

    private void UpdateUiData(PlayerStatistics.Upgrade currentUpgrade)
    {
        currentUpgrade = playerStats.upgradesList[selectedUpgradeIndexForPreview];
        if (currentUpgrade.IsUnlocked)
        {
            uiPreviewArea.GetComponent<Image>().color = playerStats.HexToRGB("#30303030");

            unlockButtonsGO.SetActive(true);
            
            if(currentUpgrade.IsActive)
            {
                if (!currentUpgrade.ColorStuff[currentUpgrade.ParticlesColor.ToString()].IsUnlocked)
                {
                    unlockButtonsGO.SetActive(false);
                    lockButtonsGO.SetActive(true);
                    moneyCostTMPro.text = currentUpgrade.ColorStuff[currentUpgrade.ParticlesColor.ToString()].MoneyCost.ToString();
                    coinCostTMPro.text = currentUpgrade.ColorStuff[currentUpgrade.ParticlesColor.ToString()].CoinCost.ToString();
                }
                else
                {
                    lockButtonsGO.SetActive(false);
                    if (currentUpgrade.ColorStuff[currentUpgrade.ParticlesColor.ToString()].IsActive)
                        unlockButtonTMPro.text = "Selected";
                    else
                        unlockButtonTMPro.text = "Select Color";
                }
                colorSelector.SetActive(true);
                colorSelector.GetComponent<Image>().color = playerStats.colorsData[currentUpgrade.ParticlesColor.ToString()].ThirdColor;
            } else
            {
                unlockButtonTMPro.text = "Select Skin";
                lockButtonsGO.SetActive(false);
                colorSelector.SetActive(false);
            }
        } else
        {
            uiPreviewArea.GetComponent<Image>().color = playerStats.HexToRGB("#30303082");
            unlockButtonsGO.SetActive(false);
            lockButtonsGO.SetActive(true);
            colorSelector.SetActive(false);
            moneyCostTMPro.text = currentUpgrade.MoneyCost.ToString();
            coinCostTMPro.text = currentUpgrade.CoinCost.ToString();
        }
    }


    public void SelectButtonClicked()
    {
        PlayerStatistics.Upgrade currentUpgrade = playerStats.upgradesList[selectedUpgradeIndexForPreview];
        if(currentUpgrade.IsActive)
        {
            PlayerStatistics.SkinColorStuff skinColorData = currentUpgrade.ColorStuff[currentUpgrade.ParticlesColor.ToString()];
            List<string> keys = new List<string>(currentUpgrade.ColorStuff.Keys);
            foreach (string key in keys)
            {
                PlayerStatistics.SkinColorStuff colorData = currentUpgrade.ColorStuff[key];
                if (key == currentUpgrade.ParticlesColor.ToString())
                    colorData.IsActive = true;
                else
                    colorData.IsActive = false;
                currentUpgrade.ColorStuff[key] = colorData;
            }
        } else
        {
            int numUpgrades = playerStats.upgradesList.Count;
            for (int i=0; i<numUpgrades; i++)
            {
                PlayerStatistics.Upgrade tempUpgrade = playerStats.upgradesList[i];
                if(tempUpgrade.ApplicableOn == currentUpgrade.ApplicableOn)
                {
                    tempUpgrade.IsActive = false;
                    playerStats.upgradesList[i] = tempUpgrade;
                }
            }
            currentUpgrade.IsActive = true;
        }
        playerStats.upgradesList[selectedUpgradeIndexForPreview] = currentUpgrade;
        UpdateUiData(currentUpgrade);
    }

    public void BuyButtonClicked(bool fromCoin)
    {
        PlayerStatistics.Upgrade currentUpgrade = playerStats.upgradesList[selectedUpgradeIndexForPreview];
        if(currentUpgrade.IsUnlocked)
        {
            if(fromCoin)
            {
                PlayerStatistics.SkinColorStuff skinColorData = currentUpgrade.ColorStuff[currentUpgrade.ParticlesColor.ToString()];
                if(playerStats.playerCoins >= skinColorData.CoinCost)
                {
                    playerStats.playerCoins -= skinColorData.CoinCost;
                    numCoinsTMPro.text = playerStats.playerCoins.ToString();
                    skinColorData.IsUnlocked = true;
                    currentUpgrade.ColorStuff[currentUpgrade.ParticlesColor.ToString()] = skinColorData;

                    taskHandlerClass.UpdateLevelTaskState(ObjectsDescription.Player, TaskTypes.UpdateSkin, TaskCategory.ImmediateActionTask, new List<string>() { skinColorData.CoinCost.ToString() });
                }
            } else
            {
                // pay from credit card.
            }
            playerStats.upgradesList[selectedUpgradeIndexForPreview] = currentUpgrade;
        } else
        {
            if(fromCoin)
            {
                if(playerStats.playerCoins >= currentUpgrade.CoinCost)
                {
                    playerStats.playerCoins -= currentUpgrade.CoinCost;
                    numCoinsTMPro.text = playerStats.playerCoins.ToString();
                    currentUpgrade.IsUnlocked = true;

                    taskHandlerClass.UpdateLevelTaskState(ObjectsDescription.Player, TaskTypes.UpdateSkin, TaskCategory.ImmediateActionTask, new List<string>() { currentUpgrade.CoinCost.ToString() });
                }
            } else
            {
                // pay from credit card.
            }
            playerStats.upgradesList[selectedUpgradeIndexForPreview] = currentUpgrade;
            upgradeScrollerClass.UpgradeUnlocked(selectedUpgradeIndexForPreview);
        }
        UpdateUiData(currentUpgrade);
    }


    private void CreateSkinsAndMaterialsDict()
    {
        var categoriesList = (SkinCategory[])System.Enum.GetValues(typeof(SkinCategory));
        int numSkins = categoriesList.Length;
        for(int i=0;i<numSkins;i++)
        {
            GameObject goInstance = Instantiate(upgradesGO[i]);
            goInstance.transform.parent = transform;
            goInstance.SetActive(false);
            goInstance.name = categoriesList[i].ToString();
            skinsAndMaterials.Add(categoriesList[i].ToString(), goInstance);
        }
    }


    void Start()
    {
        Vector3[] corners = new Vector3[4];

        uiPreviewArea.GetComponent<RectTransform>().GetWorldCorners(corners);
        for (int i = 0; i < 4; i++)
        {
            corners[i] = mainCamera.ScreenToWorldPoint(corners[i]);
        }
        SetPreviewMovementArea(corners[0].x, corners[3].x, corners[0].y, corners[1].y);

        colorSelector.GetComponent<RectTransform>().GetWorldCorners(corners);
        for (int i = 0; i < 4; i++)
        {
            corners[i] = mainCamera.ScreenToWorldPoint(corners[i]);
        }
        SetColorSelectorArea(corners[0].x, corners[3].x, corners[0].y, corners[1].y);


        playerClass = playerSkin.GetComponent<Player>();
        playerStats = FindObjectOfType<PlayerStatistics>();

        coinCostTMPro = coinTextGO.GetComponent<TextMeshProUGUI>();
        moneyCostTMPro = moneyTextGO.GetComponent<TextMeshProUGUI>();
        unlockButtonTMPro = unlockTextGO.GetComponent<TextMeshProUGUI>();
        numCoinsTMPro = numCoinsTextGO.GetComponent<TextMeshProUGUI>();
        numCoinsTMPro.text = playerStats.playerCoins.ToString();

        selectedUpgradeIndexForPreview = 0;

        taskHandlerClass = FindObjectOfType<TaskHandler>();

        upgradeScrollerClass = GetComponent<UpgradeScroller>();

        PlayerStatistics.Upgrade currentUpgrade = playerStats.upgradesList[selectedUpgradeIndexForPreview];

        CreateSkinsAndMaterialsDict();
        
        UpdateUiData(currentUpgrade);

        UpdateMaterial(currentUpgrade);

        playerStats.UpdateColorOfSkin(currentUpgrade, playerSkin);

    }

    void Update()
    {
        PlayerStatistics.Upgrade activeUpgrade = playerStats.upgradesList[selectedUpgradeIndexForPreview];
        if (activeUpgrade.ApplicableOn == ObjectsDescription.Player && previewHasFocus)
        {
            EnablePlayerSkinPreview();
        } else if(activeUpgrade.ApplicableOn == ObjectsDescription.PlayerLauncher)
        {
            EnableLauncherSkinPreview();
        }
    }

    /*
    private void OnDestroy()
    {
        int numUpgrades = playerStats.upgradesList.Count;
        for(int i=0;i<numUpgrades;i++)
        {
            PlayerStatistics.Upgrade currUpgrade = playerStats.upgradesList[i];
            if (currUpgrade.IsActive)
            {
                skinsAndMaterials[currUpgrade.UpgradeCategory.ToString()].transform.parent = playerStats.transform;
                skinsAndMaterials[currUpgrade.UpgradeCategory.ToString()].SetActive(false);
            }
        }
    }
    */
    
}
