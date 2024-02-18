using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotHandlerMenu : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    public GameObject itemHeld = null;
    private bool shouldResetItemHeld = false;

    public GameObject StartButton;
    public GameObject BgButton;
    public GameObject Banana;
    public GameObject Inventory;

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
            if (item.GetComponent<SlotHandlerMenu>())
            {
                item = item.GetComponent<SlotHandlerMenu>().itemHeld;
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

    private void Update()
    {
        bool bStartGameEnabled = false;
        bool bShowBanana = false;
        bool bBgButton = false;
        bool bInventory = false;
        if (itemHeld != null)
        {
            MenuItem menuItem = itemHeld.GetComponent<MenuItem>();
            if (menuItem)
            {
                if (menuItem.Type == MenuItemType.GameStart)
                {
                    bStartGameEnabled = true;
                }
                else if (menuItem.Type == MenuItemType.Banana) 
                {
                    bShowBanana = true;
                }
                else if (menuItem.Type == MenuItemType.ChangeBg)
                {
                    bBgButton = true;
                }
                else if (menuItem.Type == MenuItemType.SeeItems)
                {
                    bInventory = true;
                }
            }
        }

        if (!StartButton.activeSelf)
        {
            if (bStartGameEnabled)
            {
                StartButton.SetActive(true);
            }
        }
        else
        { 
            if (!bStartGameEnabled)
            {
                StartButton.SetActive(false);
            }
        }
        
        if (!Banana.activeSelf)
        {
            if (bShowBanana)
            {
                Banana.SetActive(true);
            }
        }
        else
        {
            if (!bShowBanana)
            {
                Banana.SetActive(false);
            }
        }

        if (!BgButton.activeSelf)
        {
            if (bBgButton)
            {
                BgButton.SetActive(true);
            }
        }
        else
        {
            if (!bBgButton)
            {
                BgButton.SetActive(false);
            }
        }

        if (!Inventory.activeSelf)
        {
            if (bInventory)
            {
                Inventory.SetActive(true);
            }
        }
        else
        {
            if (!bInventory)
            {
                Inventory.SetActive(false);
            }
        }
    }
}
