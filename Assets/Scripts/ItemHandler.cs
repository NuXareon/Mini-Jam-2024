using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private Canvas canvas;

    private RectTransform rectTransform;
    private Vector2 initialPosition;

    private CanvasGroup canvasGroup;

    public bool onDragEndReset = true;

    public void ResetItem()
    {
        rectTransform.anchoredPosition = initialPosition;
        canvasGroup.blocksRaycasts = true;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.anchoredPosition;

        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
        onDragEndReset = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (onDragEndReset) 
        {
            ResetItem();
        }
        canvasGroup.alpha = 1f;
    }
}
