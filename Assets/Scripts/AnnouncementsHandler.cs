using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnnouncementsHandler : MonoBehaviour
{
    public float AnnouncementTime = 1f;
    [SerializeField] private TextMeshProUGUI Text;

    private float RemainingAnnouncementTime = 0f;

    private void Start()
    {
        RemainingAnnouncementTime = AnnouncementTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (RemainingAnnouncementTime > 0f)
        {
            RemainingAnnouncementTime -= Time.deltaTime;
            if (RemainingAnnouncementTime < 0f)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void Announce(string NewMessage)
    {
        Text.text = NewMessage;
        RemainingAnnouncementTime = AnnouncementTime;
        gameObject.SetActive(true);
    }
}
