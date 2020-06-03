using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsGlow : MonoBehaviour
{
    [SerializeField] Material objectsGlowMaterial;
    [SerializeField] float fadeOneMin = 0.68f, fadeOneMax = 0.68f;
    [SerializeField] float fadeTwoMin = 0.7f, fadeTwoMax = 3f;
    [SerializeField] float blendFadeMin = 0.25f, blendFadeMax = 1f;
    [SerializeField] float glowCycleTime = 0.5f;
    float fadeOneCurrent, fadeTwoCurrent, blendFadeCurrent, fadeOneDifference, fadeTwoDifference, blendFadeDifference;
    float glowLastingTime;

    [SerializeField] bool isGlowing = true;

    private IEnumerator Start()
    {
        fadeOneCurrent = fadeOneMin;
        fadeTwoCurrent = fadeTwoMin;
        blendFadeCurrent = blendFadeMin;
        glowLastingTime = glowCycleTime / 20;
        fadeOneDifference = (fadeOneMax - fadeOneMin) / 20;
        fadeTwoDifference = (fadeTwoMax - fadeTwoMin) / 20;
        blendFadeDifference = (blendFadeMax - blendFadeMin) / 20;
        while (true)
        {
            yield return StartCoroutine(StartObjectCycle());
        }
    }

    private IEnumerator StartObjectCycle()
    {
        float timeInstance = 0f;
        while (timeInstance <= glowCycleTime)
        {
            timeInstance += glowLastingTime;
            yield return new WaitForSeconds(glowLastingTime);
            UpdateObjectGlow();
        }
        isGlowing = !isGlowing;
    }

    private void UpdateObjectGlow()
    {
        if (fadeOneDifference != 0)
            UpdateGlowProperties("_AlphaIntensity_Fade_1", ref fadeOneCurrent, fadeOneMin, fadeOneMax, fadeOneDifference);
        if (fadeTwoDifference != 0)
            UpdateGlowProperties("_AlphaIntensity_Fade_2", ref fadeTwoCurrent, fadeTwoMin, fadeTwoMax, fadeTwoDifference);
        if (blendFadeDifference != 0)
            UpdateGlowProperties("_OperationBlen_Fade_1", ref blendFadeCurrent, blendFadeMin, blendFadeMax, blendFadeDifference);
    }

    void UpdateGlowProperties(string propertyName, ref float currentIntensity, float minIntensity, float maxIntensity, float intensityDiff)
    {
        objectsGlowMaterial.SetFloat(propertyName, currentIntensity);

        if (isGlowing)
        {
            currentIntensity += intensityDiff;
        }
        else
        {
            currentIntensity -= intensityDiff;
        }
    }

}


