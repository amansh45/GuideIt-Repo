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

        UpdateUiData();
        UpdateColorOfSkin();
    }

    public void ShowMainCanvas()
    {
        secondaryCanvas.SetActive(false);
        playerSkin.SetActive(true);
        mainCanvas.SetActive(true);
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
        selectedUpgradeIndexForPreview = index;
        UpdateUiData();
        //UpdateMaterial();
        UpdateColorOfSkin();
    }

    private void UpdateUiData()
    {
        PlayerStatistics.Upgrade activeUpgrade = playerStats.upgradesList[selectedUpgradeIndexForPreview];
        if (activeUpgrade.IsUnlocked)
        {
            unlockButtonsGO.SetActive(true);
            
            if(activeUpgrade.IsActive)
            {
                if (!activeUpgrade.ColorStuff[activeUpgrade.ParticlesColor.ToString()].IsUnlocked)
                {
                    unlockButtonsGO.SetActive(false);
                    lockButtonsGO.SetActive(true);
                    moneyCostTMPro.text = activeUpgrade.ColorStuff[activeUpgrade.ParticlesColor.ToString()].MoneyCost.ToString();
                    coinCostTMPro.text = activeUpgrade.ColorStuff[activeUpgrade.ParticlesColor.ToString()].CoinCost.ToString();
                }
                else
                {
                    lockButtonsGO.SetActive(false);
                    if (activeUpgrade.ColorStuff[activeUpgrade.ParticlesColor.ToString()].IsActive)
                        unlockButtonTMPro.text = "Selected";
                    else
                        unlockButtonTMPro.text = "Select Color";
                }
                colorSelector.SetActive(true);
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
            moneyCostTMPro.text = activeUpgrade.MoneyCost.ToString();
            coinCostTMPro.text = activeUpgrade.CoinCost.ToString();
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
        playerClass = playerSkin.GetComponent<Player>();

        playerStats = FindObjectOfType<PlayerStatistics>();

        selectedUpgradeIndexForPreview = 0;

        SetPreviewMovementArea(corners[0].x, corners[3].x, corners[0].y, corners[1].y);

        coinCostTMPro = coinTextGO.GetComponent<TextMeshProUGUI>();
        moneyCostTMPro = moneyTextGO.GetComponent<TextMeshProUGUI>();
        unlockButtonTMPro = unlockTextGO.GetComponent<TextMeshProUGUI>();

        UpdateUiData();

        UpdateColorOfSkin();

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

    private void UpdateColorOfSkin()
    {
        PlayerStatistics.Upgrade currentUpgrade = playerStats.upgradesList[selectedUpgradeIndexForPreview];

        if (currentUpgrade.ApplicableOn == ObjectsDescription.Player)
        {
            if(currentUpgrade.UpgradeCategory == SkinCategory.PlayerBasic)
            {
                GameObject mat = playerSkin.transform.GetChild(0).gameObject;
                UpdatePlayerBasicSkinColor(mat, currentUpgrade);
            } else if(currentUpgrade.UpgradeCategory == SkinCategory.PlayerModerate)
            {

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
