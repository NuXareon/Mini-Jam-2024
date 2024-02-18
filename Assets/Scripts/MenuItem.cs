using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuItemType
{
    GameStart,
    Banana,
    ChangeBg,
    SeeItems
}

public class MenuItem : MonoBehaviour
{
    public MenuItemType Type;
}
