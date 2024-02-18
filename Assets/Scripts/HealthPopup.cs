using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthPopup : MonoBehaviour
{
    public TextMeshProUGUI text;

    public static HealthPopup Create(Transform prefab, Vector3 position, int healthAmount)
    {
        Transform newPopup = Instantiate(prefab, position, Quaternion.identity);

        HealthPopup healtPopup = newPopup.GetComponent<HealthPopup>();
        healtPopup.Setup(healthAmount);

        return healtPopup;
    }

    private void Setup(int HealthAmount)
    {
        text.text = HealthAmount.ToString();
    }

    // Start is called before the first frame update
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
