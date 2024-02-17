using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAnimation : MonoBehaviour
{
    public float ScrollSpeedX = 0f;
    public float ScrollSpeedY = 0f;

    Renderer backgroundRenderer;

    // Start is called before the first frame update
    void Start()
    {
        backgroundRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 offset = new Vector2(backgroundRenderer.material.mainTextureOffset.x + ScrollSpeedX * Time.unscaledDeltaTime, backgroundRenderer.material.mainTextureOffset.y + ScrollSpeedY * Time.unscaledDeltaTime);
        backgroundRenderer.material.mainTextureOffset = offset;
    }
}
