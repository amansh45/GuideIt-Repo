using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    [SerializeField] GameObject rippleEffectParticleSystem, explodeParticleSystem, deathParticleSystem, player;
    [SerializeField] float rippleEffectDuration = 1f, explosionEffectDuration = 1.5f, deathEffectDuration = 2f;
    [SerializeField] Camera mainCamera;

    // Note: The sequence for declaring these should be same as that of skinCategory and also the size of the list should be the size of the skinCategory(upgradeManager)...
    [SerializeField] List<GameObject> upgradesGO;
    
    ScreenRipple screenRippleClass;
    PlayerStatistics playerStats;
    Dictionary<string, GameObject> skinsAndMaterials = new Dictionary<string, GameObject>();
    CameraManager camManager;
    Color skinColor = Color.white;

    private void Start()
    {
        screenRippleClass = mainCamera.GetComponent<ScreenRipple>();
        playerStats = FindObjectOfType<PlayerStatistics>();
        camManager = FindObjectOfType<CameraManager>();
        CreateSkinsAndMaterialsDict();
        AddSkinsToLevel();
    }

    private void Update()
    {
        if(camManager == null)
            camManager = FindObjectOfType<CameraManager>();
    }


    public void PlayerDied(Vector3 position, GameObject collider, float camShakeDuration)
    {
        GameObject deathEffect = Instantiate(deathParticleSystem, position, transform.rotation);

        ParticleSystem ps = deathEffect.GetComponent<ParticleSystem>();

        var main = ps.main;
        main.startColor = skinColor;

        Destroy(deathEffect, deathEffectDuration);
        InitiateCameraShakeEffect(camShakeDuration);
        InitiateScreenRippleEffect(position);
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

    public void InitiateCameraShakeEffect(float effectDuration)
    {
        camManager.ShakeCamera(effectDuration);
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

                    PlayerStatistics.CustomColor mcolor = playerStats.colorsData[currUpgrade.ParticlesColor.ToString()];
                    skinColor = mcolor.ThirdColor;
                }
            }
        }
    }
}
