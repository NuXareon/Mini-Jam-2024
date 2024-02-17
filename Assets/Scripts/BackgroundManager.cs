using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public Material[] BackgroundImages;

    private Renderer[] BackgoundRenderers;
    private int CurrentBgIndex = 0;

    private void Start()
    {
        BackgoundRenderers = GetComponentsInChildren<Renderer>();
    }

    public void SetNextBackground()
    {
        CurrentBgIndex += 1;
        CurrentBgIndex %= BackgroundImages.Length;
        foreach (Renderer renderer in BackgoundRenderers)
        {
            //renderer.material.mainTextureOffset = Vector2.zero;
            //BackgroundImages[CurrentBgIndex].mainTextureScale = renderer.material.mainTextureScale;
            //Material[] mats = renderer.materials;
            //mats[0] = BackgroundImages[CurrentBgIndex];
            //renderer.materials = mats;
            //renderer.materials[0] = BackgroundImages[CurrentBgIndex];
            renderer.material = BackgroundImages[CurrentBgIndex];
        }
    }
}
