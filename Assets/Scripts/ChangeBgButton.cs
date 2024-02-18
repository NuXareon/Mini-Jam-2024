using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ChangeBgButton : MonoBehaviour, IPointerClickHandler
{
    public BackgroundManager bgManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        bgManager.SetNextBackground();
    }
}
