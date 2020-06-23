using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum SkinCategory
{
    PlayerBasic,
    PlayerModerate,
    PlayerAdvance,
    LauncherBasic,
    LauncherModerate,
    LauncherAdvance,
    BulletBasic,
    BulletModerate,
    BulletAdvance,
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
    [SerializeField] GameObject colorSelector;
    [SerializeField] float thresholdDistanceBetweenCheckpoints = 0.3f, thresholdDoubleClickTime = 0.3f, thresholdBetweenDClickAndPlayer = 0.5f;
    [SerializeField] GameObject mainCanvas, secondaryCanvas;

    int selectedUpgradeIndexForPreview;
    TextMeshProUGUI coinCostTMPro, moneyCostTMPro, unlockButtonTMPro;
    GameObject previewMovementArea, currentLine, previousFingerPosition, playerLauncherInstance;
    Vector3 previewMovementAreaScale;
    Player playerClass;
    float previousClickTime = int.MinValue;
    PlayerStatistics playerStats;
    UpgradeScroller upgradeScrollerClass;
    
    bool isShooting = false;

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


    private void EnableLauncherSkinPreview()
    {
        Vector2 tempFingerPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 currentPlayerPos = playerSkin.transform.position;
        
        if (Input.GetMouseButtonDown(0))
        {
            if(ClickedInsidePreviewArea(tempFingerPos))
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
        mainCanvas.SetActive(false);
        secondaryCanvas.SetActive(true);
        playerSkin.SetActive(false);
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
        UpdateColorOfSkin(currentUpgrade);
    }

    public void ShowMainCanvas()
    {
        secondaryCanvas.SetActive(false);
        playerSkin.SetActive(true);
        mainCanvas.SetActive(true);
    }

    void CreateLine(Vector2 initialFingerPos)
    {
        currentLine = Instantiate(linePrefab, initialFingerPos, Quaternion.identity) as GameObject;
        previousFingerPosition = currentLine;
        playerClass.SetWayPoints(previousFingerPosition, false);
    }


    void UpdateLine(Vector2 newFingerPos)
    {
        float angle = 0;
        var rotation = Quaternion.Euler(0, 0, angle);
        GameObject lineInstance = Instantiate(linePrefab, newFingerPos, rotation) as GameObject;
        previousFingerPosition = lineInstance;
        playerClass.SetWayPoints(previousFingerPosition, true);
    }

    public void UpgradeClicked(int index)
    {
        selectedUpgradeIndexForPreview = index;
        PlayerStatistics.Upgrade currentUpgrade = playerStats.upgradesList[selectedUpgradeIndexForPreview];
        upgradeScrollerClass.UpgradeChosen(index);
        UpdateUiData(currentUpgrade);
        UpdateMaterial(currentUpgrade);
        UpdateColorOfSkin(currentUpgrade);
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

        GameObject newParticle = Instantiate(currentUpgrade.UpgradeParticle, targetParticle.transform.position, targetParticle.transform.rotation);
        newParticle.transform.parent = targetParticle.transform.parent;
        newParticle.transform.name = currentUpgrade.UpgradeCategory.ToString();
        Destroy(targetParticle);
    }

    private void UpdateUiData(PlayerStatistics.Upgrade currentUpgrade)
    {
        currentUpgrade = playerStats.upgradesList[selectedUpgradeIndexForPreview];
        if (currentUpgrade.IsUnlocked)
        {
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
            unlockButtonsGO.SetActive(false);
            lockButtonsGO.SetActive(true);
            colorSelector.SetActive(false);
            moneyCostTMPro.text = currentUpgrade.MoneyCost.ToString();
            coinCostTMPro.text = currentUpgrade.CoinCost.ToString();
        }
    }


    /*
     * Updating the color of the skin
     * 
     */
    
    private void UpdatePlayerBasicSkinColor(GameObject mat, PlayerStatistics.Upgrade currUpgrade)
    {
        PlayerStatistics.CustomColor mcolor = playerStats.colorsData[currUpgrade.ParticlesColor.ToString()];
        playerSkin.GetComponent<SpriteRenderer>().color = mcolor.ThirdColor;
        foreach (Transform child in mat.transform)
        {
            ParticleSystem ps = child.GetComponent<ParticleSystem>();
            if (child.transform.name == "Player Trail" || child.transform.name == "Player Particles")
            {
                var col = ps.colorOverLifetime;
                col.enabled = true;
                Gradient grad = new Gradient();                
                grad.SetKeys(new GradientColorKey[] { new GradientColorKey(mcolor.FirstColor, 0.0f), new GradientColorKey(mcolor.SecondColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });                
                col.color = grad;
            } else if(child.transform.name == "Player Glow")
            {
                ParticleSystem.MinMaxGradient grad = new ParticleSystem.MinMaxGradient(mcolor.ThirdColor, mcolor.FourthColor);
                var main = ps.main;
                main.startColor = grad;
            }
        }
    }

    private void UpdateColorOfSkin(PlayerStatistics.Upgrade currentUpgrade)
    {

        if (currentUpgrade.ApplicableOn == ObjectsDescription.Player)
        {
            if(currentUpgrade.UpgradeCategory == SkinCategory.PlayerBasic)
            {
                GameObject mat = playerSkin.transform.GetChild(0).gameObject;
                UpdatePlayerBasicSkinColor(mat, currentUpgrade);
            } else if(currentUpgrade.UpgradeCategory == SkinCategory.PlayerModerate)
            {
                GameObject mat = playerSkin.transform.GetChild(0).gameObject;
                UpdatePlayerBasicSkinColor(mat, currentUpgrade);
            } else if(currentUpgrade.UpgradeCategory == SkinCategory.PlayerAdvance)
            {

            }
        } else if(currentUpgrade.ApplicableOn == ObjectsDescription.PlayerLauncher)
        {

        } else if(currentUpgrade.ApplicableOn == ObjectsDescription.PlayerProjectile)
        {

        }
    }

    /*
     * End
     */

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
                playerStats.playerCoins -= skinColorData.CoinCost;
                skinColorData.IsUnlocked = true;
                currentUpgrade.ColorStuff[currentUpgrade.ParticlesColor.ToString()] = skinColorData;
            } else
            {
                // pay from credit card.
            }
            playerStats.upgradesList[selectedUpgradeIndexForPreview] = currentUpgrade;
        } else
        {
            if(fromCoin)
            {
                playerStats.playerCoins -= currentUpgrade.CoinCost;
                currentUpgrade.IsUnlocked = true;
            } else
            {
                // pay from credit card.
            }
            playerStats.upgradesList[selectedUpgradeIndexForPreview] = currentUpgrade;
            upgradeScrollerClass.UpgradeUnlocked(selectedUpgradeIndexForPreview);
        }
        UpdateUiData(currentUpgrade);
    }
    void Start()
    {
        Vector3[] corners = new Vector3[4];
        uiPreviewArea.GetComponent<RectTransform>().GetWorldCorners(corners);
        for (int i = 0; i < 4; i++)
        {
            corners[i] = mainCamera.ScreenToWorldPoint(corners[i]);
        }
        playerClass = playerSkin.GetComponent<Player>();

        playerStats = FindObjectOfType<PlayerStatistics>();

        selectedUpgradeIndexForPreview = 0;

        SetPreviewMovementArea(corners[0].x, corners[3].x, corners[0].y, corners[1].y);

        coinCostTMPro = coinTextGO.GetComponent<TextMeshProUGUI>();
        moneyCostTMPro = moneyTextGO.GetComponent<TextMeshProUGUI>();
        unlockButtonTMPro = unlockTextGO.GetComponent<TextMeshProUGUI>();

        upgradeScrollerClass = GetComponent<UpgradeScroller>();

        PlayerStatistics.Upgrade currentUpgrade = playerStats.upgradesList[selectedUpgradeIndexForPreview];

        UpdateUiData(currentUpgrade);

        UpdateColorOfSkin(currentUpgrade);

    }

    void Update()
    {
        PlayerStatistics.Upgrade activeUpgrade = playerStats.upgradesList[selectedUpgradeIndexForPreview];
        if (activeUpgrade.ApplicableOn == ObjectsDescription.Player)
        {
            EnablePlayerSkinPreview();
        } else if(activeUpgrade.ApplicableOn == ObjectsDescription.PlayerLauncher)
        {
            EnableLauncherSkinPreview();
        }
    }
}
