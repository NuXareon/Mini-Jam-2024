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
            renderer.material = BackgroundImages[CurrentBgIndex];
        }
    }
}
