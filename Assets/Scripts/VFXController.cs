using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    [SerializeField] GameObject rippleEffectParticleSystem, explodeParticleSystem, player;
    [SerializeField] float rippleEffectDuration = 1f, explosionEffectDuration = 1.5f, cameraShakeDuration = 0.25f;
    [SerializeField] Camera mainCamera;

    // Note: The sequence for declaring these should be same as that of skinCategory and also the size of the list should be the size of the skinCategory(upgradeManager)...
    [SerializeField] List<GameObject> upgradesGO;
    
    ScreenRipple screenRippleClass;
    PlayerStatistics playerStats;
    Dictionary<string, GameObject> skinsAndMaterials = new Dictionary<string, GameObject>();
    
    private void Start()
    {
        screenRippleClass = mainCamera.GetComponent<ScreenRipple>();
        playerStats = FindObjectOfType<PlayerStatistics>();
        CreateSkinsAndMaterialsDict();
        AddSkinsToLevel();
    }

    
    public void InitiateRippleEffect(Vector3 position)
    {
        GameObject ripple = Instantiate(rippleEffectParticleSystem, position, transform.rotation);
        Destroy(ripple, rippleEffectDuration);
    }

    public void InitiateExplodeEffect(Vector3 position)
    {
        GameObject explosion = Instantiate(explodeParticleSystem, position, transform.rotation);
        Destroy(explosion, explosionEffectDuration);
    }

    public void InitiateCameraShakeEffect()
    {
        CameraManager camManager = FindObjectOfType<CameraManager>().GetComponent<CameraManager>();
        camManager.ShakeCamera(cameraShakeDuration);
    }

    public void InitiateScreenRippleEffect(Vector3 position)
    {
        screenRippleClass.ScreenRippleEffect(transform.position);
    }

    private void CreateSkinsAndMaterialsDict()
    {
        var categoriesList = (SkinCategory[])System.Enum.GetValues(typeof(SkinCategory));
        int numSkins = categoriesList.Length;
        for (int i = 0; i < numSkins; i++)
        {
            GameObject goInstance = Instantiate(upgradesGO[i]);
            goInstance.transform.parent = transform;
            goInstance.SetActive(false);
            goInstance.name = categoriesList[i].ToString();
            skinsAndMaterials.Add(categoriesList[i].ToString(), goInstance);
        }
    }

    private void AddSkinsToLevel()
    {
        int numUpgrades = playerStats.upgradesList.Count;
        for (int i = 0; i < numUpgrades; i++)
        {
            PlayerStatistics.Upgrade currUpgrade = playerStats.upgradesList[i];
            if (currUpgrade.IsActive)
            {
                if (currUpgrade.ApplicableOn == ObjectsDescription.Player)
                {
                    skinsAndMaterials[currUpgrade.UpgradeCategory.ToString()].transform.parent = player.transform;
                    skinsAndMaterials[currUpgrade.UpgradeCategory.ToString()].transform.position = player.transform.position;
                    skinsAndMaterials[currUpgrade.UpgradeCategory.ToString()].SetActive(true);
                    playerStats.UpdateColorOfSkin(currUpgrade, player);
                }
            }
        }
    }
}
