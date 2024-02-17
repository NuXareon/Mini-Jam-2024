using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    public GameObject itemHeld = null;
    private bool shouldResetItemHeld = false;

    public ItemStats GetItemStats()
    {
        if (itemHeld)
        {
            return itemHeld.GetComponent<ItemStats>();
        }
        return null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemHeld != null)
        {
            itemHeld.GetComponent<ItemHandler>().OnBeginDrag(eventData);
            shouldResetItemHeld = true;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (itemHeld != null)
        {
            itemHeld.GetComponent<ItemHandler>().OnEndDrag(eventData);
            if (shouldResetItemHeld)
            {
                itemHeld = null;
                shouldResetItemHeld = false;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (itemHeld != null)
        {
            itemHeld.GetComponent<ItemHandler>().OnDrag(eventData);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            // If the item is in a slot, we fake the item being drag, but it's the slot receiving the events, so we need to get the item from them.
            GameObject item = eventData.pointerDrag;
            if (item.GetComponent<SlotHandler>())
            {
                item = item.GetComponent<SlotHandler>().itemHeld;
            }

            if (itemHeld != null && item != itemHeld)
            {
                itemHeld.GetComponent<ItemHandler>().ResetItem();
            }
            item.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            item.GetComponent<ItemHandler>().onDragEndReset = false;
            itemHeld = item;
            shouldResetItemHeld = false;
        }
        else if (itemHeld)
        {
            // If we receiven this event and we have an item held, means that we dropped the current item out
            itemHeld = null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (itemHeld)
            {
                itemHeld.GetComponent<ItemHandler>().ResetItem();
                itemHeld = null;
            }
        }
    }
}
