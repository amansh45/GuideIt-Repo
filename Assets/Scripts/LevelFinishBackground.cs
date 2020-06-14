using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFinishBackground : MonoBehaviour
{
    [SerializeField] GameObject quadPrefab, backgroundPrefab;
    [SerializeField] Camera mainCamera;
    [SerializeField] float borderScreenOffset = 1f, borderWidth = 0.025f, finishBackgroundZAxis = -1.3f;
    [SerializeField] Color color = Color.white;
    [SerializeField] Material currentBackgroundMaterial;
    [SerializeField] GameObject backgroundParticleSystem;

    Vector3 bottomLeft, topRight, bottomRight, topLeft;

    
    public void RenderLine(Vector3 fpoint, Vector3 spoint)
    {
        GameObject borderLineGO = new GameObject();
        borderLineGO.transform.parent = transform;
        borderLineGO.transform.position = transform.position;
        LineRenderer borderLineRenderer = borderLineGO.AddComponent<LineRenderer>();
        borderLineRenderer.material = new Material(Shader.Find("Mobile/Particles/Additive"));
        borderLineRenderer.startColor = color;
        borderLineRenderer.endColor = color;
        borderLineRenderer.startWidth = borderWidth;
        borderLineRenderer.endWidth = borderWidth;
        borderLineRenderer.SetPosition(0, fpoint);
        borderLineRenderer.SetPosition(1, spoint);
        borderLineRenderer.useWorldSpace = false;
    }
    
    public void CreateBorders()
    {
        Vector3 bLeft, bRight, tLeft, tRight;
        bLeft = new Vector3(bottomLeft.x + borderScreenOffset, bottomLeft.y + borderScreenOffset, finishBackgroundZAxis);
        bRight = new Vector3(bottomRight.x - borderScreenOffset, bottomRight.y + borderScreenOffset, finishBackgroundZAxis);
        tLeft = new Vector3(topLeft.x + borderScreenOffset, topLeft.y - borderScreenOffset, finishBackgroundZAxis);
        tRight = new Vector3(topRight.x - borderScreenOffset, topRight.y - borderScreenOffset, finishBackgroundZAxis);
        RenderLine(bLeft, bRight);
        RenderLine(tLeft, tRight);
        RenderLine(bLeft, tLeft);
        RenderLine(bRight, tRight);
    }

    void Start()
    {

    }

    private void OnEnable()
    {
        bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        bottomRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0));
        topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));
        topLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, 0));
        float cameraWidth = bottomRight.x - bottomLeft.x;
        float cameraHeight = topRight.y - bottomLeft.y;

        GameObject finishBackgroundInstance = Instantiate(quadPrefab, transform.position, transform.rotation);
        finishBackgroundInstance.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, finishBackgroundZAxis);
        finishBackgroundInstance.transform.localScale = new Vector3(cameraWidth, cameraHeight, 0);
        finishBackgroundInstance.GetComponent<MeshRenderer>().material = currentBackgroundMaterial;
        finishBackgroundInstance.transform.parent = transform;
        finishBackgroundInstance.name = "Finish Background";

        GameObject backgroundParticle = Instantiate(backgroundParticleSystem, transform.position, transform.rotation);
        backgroundParticle.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y + (cameraHeight/2.0f), finishBackgroundZAxis);
        backgroundParticle.transform.Rotate(90f, 0, 0, Space.World);
        backgroundParticle.transform.parent = transform;
        backgroundParticle.name = "Background Particle";

        CreateBorders();
    }

}
