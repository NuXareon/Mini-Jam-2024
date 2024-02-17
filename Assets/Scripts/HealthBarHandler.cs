using System.Collections;
using System.Collections.Generic;
using TMPro;
using TreeEditor;
using UnityEngine;

public class HealthBarHandler : MonoBehaviour
{
    // Speed of smoothing, 1.0 = ~1 second. Bigger means faster. 0.0 = No smoothing.
    public float SmoothingSpeed = 10.0f;
    public TextMeshProUGUI Text;

    private Transform Bar;
    private float targetSize = 1f;

    void Start()
    {
        Bar = transform.Find("Bar");
    }

    private void Update()
    {
        if (SmoothingSpeed > 0)
        {
            float newScale = Bar.localScale.x + (targetSize - Bar.localScale.x) * Time.deltaTime * SmoothingSpeed;
            Bar.localScale = new Vector3(newScale, 1f);
        }
    }

    public void SetSize(float size)
    {
        if (SmoothingSpeed > 0f)
        {
            targetSize = size;
        }
        else
        {
            Bar.localScale = new Vector3(size, 1f);
        }
    }

    public void SetText(string text)
    {
        if (Text != null)
        {
            Text.text = text;
        }
    }
}
