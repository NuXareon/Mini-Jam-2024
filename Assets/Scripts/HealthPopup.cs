using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

public class HealthPopup : MonoBehaviour
{
    public TextMeshProUGUI text;

    private const float LifeTimeMax = 1.0f;
    private float MoveSpeedX = 0.5f;
    private float MoveSpeedY = 1.5f;
    private Vector3 MoveDirection;
    private float FadeSpeed = 3.0f;
    private float LifeTime = LifeTimeMax;
    private UnityEngine.Color color;

    public static void Create(Transform prefab, Vector3 position, int healthAmount)
    {
        if (healthAmount == 0)
        {
            return;
        }
        GameObject ParentSpawner = GameObject.Find("UI");
        Transform newPopup = Instantiate(prefab, prefab.position + position, Quaternion.identity, ParentSpawner.transform);

        HealthPopup healtPopup = newPopup.GetComponent<HealthPopup>();
        healtPopup.Setup(healthAmount);
    }

    private void Setup(int HealthAmount)
    {
        if (HealthAmount < 0)
        {
            color = UnityEngine.Color.red;
        }
        else
        {
            color = UnityEngine.Color.green;
        }
        text.color = color;
        HealthAmount = Mathf.Abs(HealthAmount);

        float popupRatio = ((((float)HealthAmount - 1f) * (2f - 1f))/(15f - 1f)) + 1f;
        popupRatio = Mathf.Min(popupRatio, 2f);
        Debug.Log("popup ratio:" + popupRatio);
        text.fontSize *= popupRatio;

        text.text = HealthAmount.ToString();
        MoveDirection = new Vector3(MoveSpeedX, MoveSpeedY, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += MoveDirection * Time.deltaTime;
        MoveDirection.x -= 0.1f * Time.deltaTime;
        MoveDirection.y -= 2.0f * Time.deltaTime;

        float increaseSpeed = 0.001f;
        if (LifeTime > LifeTimeMax*0.5f)
        {
            transform.localScale += new Vector3(increaseSpeed, increaseSpeed) * Time.deltaTime;
        }
        else
        {
            transform.localScale -= new Vector3(increaseSpeed, increaseSpeed) * Time.deltaTime;
        }

        LifeTime -= Time.deltaTime;
        if (LifeTime < 0)
        {
            color.a -= FadeSpeed * Time.deltaTime;
            text.color = color;

            if (text.color.a < 0)
            {
                Destroy(gameObject);
            }
        }

    }
}
